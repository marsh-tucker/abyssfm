using AbyssFm.Api.Data;
using AbyssFm.Api.DTOs.Recommendations;
using AbyssFm.Api.DTOs.Songs;
using AbyssFm.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AbyssFm.Api.Services;

public class RecommendationService : IRecommendationService
{
    private readonly AbyssFmDbContext _dbContext;

    public RecommendationService(AbyssFmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<RecommendationResultDto>> GetRecommendationsAsync(
        RecommendationRequestDto request)
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .Include(song => song.PrimaryArtist)
            .Include(song => song.Album)
            .Include(song => song.PrimaryGenre)
            .Include(song => song.SongGenres)
                .ThenInclude(songGenre => songGenre.Genre)
            .Include(song => song.SongMoodTags)
                .ThenInclude(songMood => songMood.MoodTag)
            .Include(song => song.SongVibeTags)
                .ThenInclude(songVibe => songVibe.VibeTag)
            .AsSplitQuery()
            .ToListAsync();

        var hasVibes = request.VibeTagIds.Count > 0;

        var hasNumericProfile =
            request.Energy.HasValue &&
            request.Darkness.HasValue &&
            request.Danceability.HasValue &&
            request.Valence.HasValue &&
            request.Instrumentalness.HasValue;

        var weights = GetCategoryWeights(hasVibes, hasNumericProfile);

        var results = new List<RecommendationResultDto>();

        foreach (var song in songs)
        {
            var genreScore = CalculateWeightedMatch(
                song.SongGenres.Select(songGenre =>
                    (songGenre.GenreId, songGenre.Weight)),
                request.GenreIds,
                weights.Genre);

            var moodScore = CalculateWeightedMatch(
                song.SongMoodTags.Select(songMood =>
                    (songMood.MoodTagId, songMood.Weight)),
                request.MoodTagIds,
                weights.Mood);

            var vibeScore = CalculateWeightedMatch(
                song.SongVibeTags.Select(songVibe =>
                    (songVibe.VibeTagId, songVibe.Weight)),
                request.VibeTagIds,
                weights.Vibe);

            var numericScore = hasNumericProfile
                ? CalculateNumericScore(song, request, weights.Numeric)
                : 0;

            var totalScore =
                genreScore +
                moodScore +
                vibeScore +
                numericScore;

            results.Add(new RecommendationResultDto
            {
                Song = MapSongToDto(song),
                Score = Math.Round(totalScore, 1),
                Reasons = BuildReasons(song, request)
            });
        }

        return results
            .OrderByDescending(result => result.Score)
            .Take(10)
            .ToList();
    }

    private static double CalculateWeightedMatch(
        IEnumerable<(int Id, short Weight)> songTags,
        IReadOnlyCollection<int> selectedIds,
        double maxPoints)
    {
        if (selectedIds.Count == 0 || maxPoints == 0)
        {
            return 0;
        }

        var weightsById = songTags.ToDictionary(
            tag => tag.Id,
            tag => tag.Weight);

        var matches = selectedIds.Select(selectedId =>
            weightsById.TryGetValue(selectedId, out var weight)
                ? weight / 10.0
                : 0.0);

        return matches.Average() * maxPoints;
    }

    private static double CalculateNumericScore(
        Song song,
        RecommendationRequestDto request,
        double maxPoints)
    {
        var closenessScores = new[]
        {
            CalculateCloseness(song.Energy, request.Energy!.Value),
            CalculateCloseness(song.Darkness, request.Darkness!.Value),
            CalculateCloseness(song.Danceability, request.Danceability!.Value),
            CalculateCloseness(song.Valence, request.Valence!.Value),
            CalculateCloseness(
                song.Instrumentalness,
                request.Instrumentalness!.Value)
        };

        return closenessScores.Average() * maxPoints;
    }

    private static double CalculateCloseness(short songValue, short targetValue)
    {
        return 1.0 - Math.Abs(songValue - targetValue) / 9.0;
    }

    private static (
        double Genre,
        double Mood,
        double Vibe,
        double Numeric)
        GetCategoryWeights(bool hasVibes, bool hasNumericProfile)
    {
        return (hasVibes, hasNumericProfile) switch
        {
            (false, false) => (40, 60, 0, 0),
            (true, false)  => (35, 45, 20, 0),
            (false, true)  => (25, 35, 0, 40),
            (true, true)   => (20, 30, 15, 35)
        };
    }

    private static List<string> BuildReasons(
        Song song,
        RecommendationRequestDto request)
    {
        var reasons = new List<string>();

        var genreMatch = song.SongGenres
            .Where(songGenre =>
                request.GenreIds.Contains(songGenre.GenreId))
            .OrderByDescending(songGenre => songGenre.Weight)
            .FirstOrDefault();

        if (genreMatch is not null)
        {
            reasons.Add(
                $"Matches your {genreMatch.Genre!.Name} preference");
        }

        var moodMatch = song.SongMoodTags
            .Where(songMood =>
                request.MoodTagIds.Contains(songMood.MoodTagId))
            .OrderByDescending(songMood => songMood.Weight)
            .FirstOrDefault();

        if (moodMatch is not null)
        {
            reasons.Add(
                $"Matches your {moodMatch.MoodTag!.Name} mood");
        }

        var vibeMatch = song.SongVibeTags
            .Where(songVibe =>
                request.VibeTagIds.Contains(songVibe.VibeTagId))
            .OrderByDescending(songVibe => songVibe.Weight)
            .FirstOrDefault();

        if (vibeMatch is not null)
        {
            reasons.Add(
                $"Fits the {vibeMatch.VibeTag!.Name} vibe");
        }

        return reasons;
    }

    private static SongResponseDto MapSongToDto(Song song)
    {
        return new SongResponseDto
        {
            SongId = song.SongId,
            Title = song.Title,
            Artist = song.PrimaryArtist!.Name,
            Album = song.Album?.Title,
            PrimaryGenre = song.PrimaryGenre!.Name,
            DurationSeconds = song.DurationSeconds,
            TempoBpm = song.TempoBpm,
            Energy = song.Energy,
            Darkness = song.Darkness,
            Danceability = song.Danceability,
            Valence = song.Valence,
            Instrumentalness = song.Instrumentalness,
            Description = song.Description,
            CoverTheme = song.CoverTheme,

            Genres = song.SongGenres
                .OrderByDescending(songGenre => songGenre.Weight)
                .Select(songGenre => new WeightedTagResponseDto
                {
                    Name = songGenre.Genre!.Name,
                    Weight = songGenre.Weight
                })
                .ToList(),

            MoodTags = song.SongMoodTags
                .OrderByDescending(songMood => songMood.Weight)
                .Select(songMood => new WeightedTagResponseDto
                {
                    Name = songMood.MoodTag!.Name,
                    Weight = songMood.Weight
                })
                .ToList(),

            VibeTags = song.SongVibeTags
                .OrderByDescending(songVibe => songVibe.Weight)
                .Select(songVibe => new WeightedTagResponseDto
                {
                    Name = songVibe.VibeTag!.Name,
                    Weight = songVibe.Weight
                })
                .ToList()
        };
    }
}