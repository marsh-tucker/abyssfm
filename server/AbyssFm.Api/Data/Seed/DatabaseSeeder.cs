using System.Text.Json;
using System.Text.RegularExpressions;
using AbyssFm.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AbyssFm.Api.Data.Seed;

public class DatabaseSeeder
{
    private readonly AbyssFmDbContext _dbContext;
    private readonly ILogger<DatabaseSeeder> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DatabaseSeeder(
        AbyssFmDbContext dbContext,
        ILogger<DatabaseSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var seedFilePath = Path.Combine(
            AppContext.BaseDirectory,
            "seed-data",
            "songs.seed.json");

        if (!File.Exists(seedFilePath))
        {
            throw new FileNotFoundException(
                $"The song seed file was not found at: {seedFilePath}");
        }

        var json = await File.ReadAllTextAsync(
            seedFilePath,
            cancellationToken);

        var seedSongs = JsonSerializer.Deserialize<List<SongSeedDto>>(
            json,
            JsonOptions);

        if (seedSongs is null || seedSongs.Count == 0)
        {
            _logger.LogWarning(
                "The song seed file contained no song records.");

            return;
        }

        ValidateSeedSongs(seedSongs);

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var artistsByName = (await _dbContext.Artists
                    .ToListAsync(cancellationToken))
                .ToDictionary(
                    artist => NormalizeKey(artist.Name),
                    StringComparer.OrdinalIgnoreCase);

            var genresByName = (await _dbContext.Genres
                    .ToListAsync(cancellationToken))
                .ToDictionary(
                    genre => NormalizeKey(genre.Name),
                    StringComparer.OrdinalIgnoreCase);

            var moodTagsByName = (await _dbContext.MoodTags
                    .ToListAsync(cancellationToken))
                .ToDictionary(
                    mood => NormalizeKey(mood.Name),
                    StringComparer.OrdinalIgnoreCase);

            var vibeTagsByName = (await _dbContext.VibeTags
                    .ToListAsync(cancellationToken))
                .ToDictionary(
                    vibe => NormalizeKey(vibe.Name),
                    StringComparer.OrdinalIgnoreCase);

            var albumsByArtistAndTitle = (await _dbContext.Albums
                    .Include(album => album.Artist)
                    .ToListAsync(cancellationToken))
                .ToDictionary(
                    album => CreateAlbumKey(
                        album.Artist?.Name ?? string.Empty,
                        album.Title),
                    StringComparer.OrdinalIgnoreCase);

            var songsByIdentity = (await _dbContext.Songs
                    .Include(song => song.PrimaryArtist)
                    .Include(song => song.Album)
                    .ToListAsync(cancellationToken))
                .ToDictionary(
                    song => CreateSongKey(
                        song.PrimaryArtist?.Name ?? string.Empty,
                        song.Title,
                        song.Album?.Title),
                    StringComparer.OrdinalIgnoreCase);

            var insertedSongCount = 0;
            var skippedSongCount = 0;

            foreach (var seedSong in seedSongs)
            {
                var artist = GetOrCreateArtist(
                    seedSong.Artist,
                    artistsByName);

                var album = GetOrCreateAlbum(
                    seedSong,
                    artist,
                    albumsByArtistAndTitle);

                var primaryGenre = GetOrCreateGenre(
                    seedSong.PrimaryGenre,
                    genresByName);

                var songKey = CreateSongKey(
                    seedSong.Artist,
                    seedSong.Title,
                    seedSong.Album);

                if (songsByIdentity.ContainsKey(songKey))
                {
                    skippedSongCount++;
                    continue;
                }

                var song = new Song
                {
                    Title = seedSong.Title.Trim(),
                    PrimaryArtist = artist,
                    Album = album,
                    PrimaryGenre = primaryGenre,
                    DurationSeconds = seedSong.DurationSeconds,
                    TempoBpm = seedSong.TempoBpm,
                    Energy = seedSong.Energy,
                    Darkness = seedSong.Darkness,
                    Danceability = seedSong.Danceability,
                    Valence = seedSong.Valence,
                    Instrumentalness = seedSong.Instrumentalness,
                    Description = seedSong.Description?.Trim(),
                    CoverTheme = seedSong.CoverTheme?.Trim()
                };

                foreach (var weightedGenre in seedSong.Genres)
                {
                    var genre = GetOrCreateGenre(
                        weightedGenre.Name,
                        genresByName);

                    song.SongGenres.Add(new SongGenre
                    {
                        Song = song,
                        Genre = genre,
                        Weight = weightedGenre.Weight
                    });
                }

                foreach (var weightedMood in seedSong.MoodTags)
                {
                    var moodTag = GetOrCreateMoodTag(
                        weightedMood.Name,
                        moodTagsByName);

                    song.SongMoodTags.Add(new SongMoodTag
                    {
                        Song = song,
                        MoodTag = moodTag,
                        Weight = weightedMood.Weight
                    });
                }

                foreach (var weightedVibe in seedSong.VibeTags)
                {
                    var vibeTag = GetOrCreateVibeTag(
                        weightedVibe.Name,
                        vibeTagsByName);

                    song.SongVibeTags.Add(new SongVibeTag
                    {
                        Song = song,
                        VibeTag = vibeTag,
                        Weight = weightedVibe.Weight
                    });
                }

                _dbContext.Songs.Add(song);
                songsByIdentity.Add(songKey, song);

                insertedSongCount++;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Database seeding finished. Inserted {InsertedCount} songs and skipped {SkippedCount} existing songs.",
                insertedSongCount,
                skippedSongCount);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private Artist GetOrCreateArtist(
        string artistName,
        IDictionary<string, Artist> artistsByName)
    {
        var cleanName = artistName.Trim();
        var key = NormalizeKey(cleanName);

        if (artistsByName.TryGetValue(key, out var existingArtist))
        {
            return existingArtist;
        }

        var artist = new Artist
        {
            Name = cleanName,
            Slug = CreateSlug(cleanName)
        };

        _dbContext.Artists.Add(artist);
        artistsByName.Add(key, artist);

        return artist;
    }

    private Album? GetOrCreateAlbum(
        SongSeedDto seedSong,
        Artist artist,
        IDictionary<string, Album> albumsByArtistAndTitle)
    {
        if (string.IsNullOrWhiteSpace(seedSong.Album))
        {
            return null;
        }

        var cleanTitle = seedSong.Album.Trim();

        var key = CreateAlbumKey(
            artist.Name,
            cleanTitle);

        if (albumsByArtistAndTitle.TryGetValue(key, out var existingAlbum))
        {
            existingAlbum.ReleaseYear ??= seedSong.ReleaseYear;
            existingAlbum.CoverTheme ??= seedSong.CoverTheme?.Trim();

            return existingAlbum;
        }

        var album = new Album
        {
            Title = cleanTitle,
            Artist = artist,
            ReleaseYear = seedSong.ReleaseYear,
            CoverTheme = seedSong.CoverTheme?.Trim()
        };

        _dbContext.Albums.Add(album);
        albumsByArtistAndTitle.Add(key, album);

        return album;
    }

    private Genre GetOrCreateGenre(
        string genreName,
        IDictionary<string, Genre> genresByName)
    {
        var cleanName = genreName.Trim();
        var key = NormalizeKey(cleanName);

        if (genresByName.TryGetValue(key, out var existingGenre))
        {
            return existingGenre;
        }

        var genre = new Genre
        {
            Name = cleanName,
            Slug = CreateSlug(cleanName)
        };

        _dbContext.Genres.Add(genre);
        genresByName.Add(key, genre);

        return genre;
    }

    private MoodTag GetOrCreateMoodTag(
        string moodName,
        IDictionary<string, MoodTag> moodTagsByName)
    {
        var cleanName = moodName.Trim().ToLowerInvariant();
        var key = NormalizeKey(cleanName);

        if (moodTagsByName.TryGetValue(key, out var existingMood))
        {
            return existingMood;
        }

        var moodTag = new MoodTag
        {
            Name = cleanName,
            Slug = CreateSlug(cleanName)
        };

        _dbContext.MoodTags.Add(moodTag);
        moodTagsByName.Add(key, moodTag);

        return moodTag;
    }

    private VibeTag GetOrCreateVibeTag(
        string vibeName,
        IDictionary<string, VibeTag> vibeTagsByName)
    {
        var cleanName = vibeName.Trim().ToLowerInvariant();
        var key = NormalizeKey(cleanName);

        if (vibeTagsByName.TryGetValue(key, out var existingVibe))
        {
            return existingVibe;
        }

        var vibeTag = new VibeTag
        {
            Name = cleanName,
            Slug = CreateSlug(cleanName)
        };

        _dbContext.VibeTags.Add(vibeTag);
        vibeTagsByName.Add(key, vibeTag);

        return vibeTag;
    }

    private static void ValidateSeedSongs(
        IReadOnlyCollection<SongSeedDto> seedSongs)
    {
        var seenSongs = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var song in seedSongs)
        {
            if (string.IsNullOrWhiteSpace(song.Title))
            {
                throw new InvalidDataException(
                    "Every seed song must have a title.");
            }

            if (string.IsNullOrWhiteSpace(song.Artist))
            {
                throw new InvalidDataException(
                    $"The seed song '{song.Title}' must have an artist.");
            }

            if (string.IsNullOrWhiteSpace(song.PrimaryGenre))
            {
                throw new InvalidDataException(
                    $"The seed song '{song.Title}' must have a primary genre.");
            }

            ValidateRange(song.Energy, nameof(song.Energy), song.Title);
            ValidateRange(song.Darkness, nameof(song.Darkness), song.Title);
            ValidateRange(song.Danceability, nameof(song.Danceability), song.Title);
            ValidateRange(song.Valence, nameof(song.Valence), song.Title);
            ValidateRange(
                song.Instrumentalness,
                nameof(song.Instrumentalness),
                song.Title);

            if (song.DurationSeconds is <= 0)
            {
                throw new InvalidDataException(
                    $"The duration for '{song.Title}' must be greater than zero.");
            }

            if (song.TempoBpm is <= 0)
            {
                throw new InvalidDataException(
                    $"The tempo for '{song.Title}' must be greater than zero when supplied.");
            }

            ValidateWeightedTags(
                song.Genres,
                "genre",
                song.Title);

            ValidateWeightedTags(
                song.MoodTags,
                "mood",
                song.Title);

            ValidateWeightedTags(
                song.VibeTags,
                "vibe",
                song.Title);

            var containsPrimaryGenre = song.Genres.Any(
                genre => string.Equals(
                    genre.Name.Trim(),
                    song.PrimaryGenre.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (!containsPrimaryGenre)
            {
                throw new InvalidDataException(
                    $"The primary genre '{song.PrimaryGenre}' for '{song.Title}' must also appear in its weighted genres.");
            }

            var songKey = CreateSongKey(
                song.Artist,
                song.Title,
                song.Album);

            if (!seenSongs.Add(songKey))
            {
                throw new InvalidDataException(
                    $"The seed file contains a duplicate song: '{song.Title}' by '{song.Artist}'.");
            }
        }
    }

    private static void ValidateWeightedTags(
        IReadOnlyCollection<WeightedTagSeedDto>? tags,
        string category,
        string songTitle)
    {
        if (tags is null || tags.Count == 0)
        {
            throw new InvalidDataException(
                $"The seed song '{songTitle}' must have at least one {category}.");
        }

        var names = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag.Name))
            {
                throw new InvalidDataException(
                    $"The seed song '{songTitle}' contains a {category} without a name.");
            }

            ValidateRange(
                tag.Weight,
                $"{category} weight",
                songTitle);

            if (!names.Add(tag.Name.Trim()))
            {
                throw new InvalidDataException(
                    $"The seed song '{songTitle}' contains duplicate {category} '{tag.Name}'.");
            }
        }
    }

    private static void ValidateRange(
        short value,
        string fieldName,
        string songTitle)
    {
        if (value is < 1 or > 10)
        {
            throw new InvalidDataException(
                $"{fieldName} for '{songTitle}' must be between 1 and 10.");
        }
    }

    private static string CreateAlbumKey(
        string artistName,
        string albumTitle)
    {
        return $"{NormalizeKey(artistName)}|{NormalizeKey(albumTitle)}";
    }

    private static string CreateSongKey(
        string artistName,
        string songTitle,
        string? albumTitle)
    {
        return
            $"{NormalizeKey(artistName)}|" +
            $"{NormalizeKey(songTitle)}|" +
            $"{NormalizeKey(albumTitle ?? string.Empty)}";
    }

    private static string NormalizeKey(string value)
    {
        return value.Trim();
    }

    private static string CreateSlug(string value)
    {
        var normalized = value
            .Trim()
            .ToLowerInvariant()
            .Replace("&", " and ");

        normalized = Regex.Replace(
            normalized,
            @"[^a-z0-9]+",
            "-");

        return normalized.Trim('-');
    }
}