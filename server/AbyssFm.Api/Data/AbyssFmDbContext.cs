using AbyssFm.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AbyssFm.Api.Data;

public class AbyssFmDbContext : DbContext
{
    public AbyssFmDbContext(DbContextOptions<AbyssFmDbContext> options)
        : base(options)
    {
    }

    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<SongGenre> SongGenres => Set<SongGenre>();
    public DbSet<MoodTag> MoodTags => Set<MoodTag>();
    public DbSet<VibeTag> VibeTags => Set<VibeTag>();
    public DbSet<SongMoodTag> SongMoodTags => Set<SongMoodTag>();
    public DbSet<SongVibeTag> SongVibeTags => Set<SongVibeTag>();
    public DbSet<RecommendationSession> RecommendationSessions => Set<RecommendationSession>();
    public DbSet<RecommendationResult> RecommendationResults => Set<RecommendationResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureArtists(modelBuilder);
        ConfigureAlbums(modelBuilder);
        ConfigureGenres(modelBuilder);
        ConfigureSongs(modelBuilder);
        ConfigureSongGenres(modelBuilder);
        ConfigureMoodTags(modelBuilder);
        ConfigureVibeTags(modelBuilder);
        ConfigureSongMoodTags(modelBuilder);
        ConfigureSongVibeTags(modelBuilder);
        ConfigureRecommendationSessions(modelBuilder);
        ConfigureRecommendationResults(modelBuilder);
    }

    private static void ConfigureArtists(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Artist>();

        entity.ToTable("artists");

        entity.HasKey(artist => artist.ArtistId);

        entity.Property(artist => artist.ArtistId).HasColumnName("artist_id");
        entity.Property(artist => artist.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        entity.Property(artist => artist.Slug).HasColumnName("slug").HasMaxLength(180).IsRequired();
        entity.Property(artist => artist.ImageUrl).HasColumnName("image_url");
        entity.Property(artist => artist.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        entity.Property(artist => artist.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz");

        entity.HasIndex(artist => artist.Name).IsUnique();
        entity.HasIndex(artist => artist.Slug).IsUnique();
    }

    private static void ConfigureAlbums(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Album>();

        entity.ToTable("albums");

        entity.HasKey(album => album.AlbumId);

        entity.Property(album => album.AlbumId).HasColumnName("album_id");
        entity.Property(album => album.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        entity.Property(album => album.ArtistId).HasColumnName("artist_id");
        entity.Property(album => album.ReleaseYear).HasColumnName("release_year");
        entity.Property(album => album.CoverImageUrl).HasColumnName("cover_image_url");
        entity.Property(album => album.CoverTheme).HasColumnName("cover_theme").HasMaxLength(100);
        entity.Property(album => album.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        entity.Property(album => album.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz");

        entity.HasOne(album => album.Artist)
            .WithMany(artist => artist.Albums)
            .HasForeignKey(album => album.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(album => album.ArtistId);
        entity.HasIndex(album => new { album.Title, album.ArtistId }).IsUnique();
    }

    private static void ConfigureGenres(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Genre>();

        entity.ToTable("genres");

        entity.HasKey(genre => genre.GenreId);

        entity.Property(genre => genre.GenreId).HasColumnName("genre_id");
        entity.Property(genre => genre.Name).HasColumnName("name").HasMaxLength(80).IsRequired();
        entity.Property(genre => genre.Slug).HasColumnName("slug").HasMaxLength(100).IsRequired();
        entity.Property(genre => genre.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        entity.HasIndex(genre => genre.Name).IsUnique();
        entity.HasIndex(genre => genre.Slug).IsUnique();
    }

    private static void ConfigureSongs(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Song>();

        entity.ToTable("songs", table =>
        {
            table.HasCheckConstraint("ck_songs_energy_range", "energy BETWEEN 1 AND 10");
            table.HasCheckConstraint("ck_songs_darkness_range", "darkness BETWEEN 1 AND 10");
            table.HasCheckConstraint("ck_songs_danceability_range", "danceability BETWEEN 1 AND 10");
            table.HasCheckConstraint("ck_songs_valence_range", "valence BETWEEN 1 AND 10");
            table.HasCheckConstraint("ck_songs_instrumentalness_range", "instrumentalness BETWEEN 1 AND 10");
        });

        entity.HasKey(song => song.SongId);

        entity.Property(song => song.SongId).HasColumnName("song_id");
        entity.Property(song => song.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        entity.Property(song => song.PrimaryArtistId).HasColumnName("primary_artist_id");
        entity.Property(song => song.AlbumId).HasColumnName("album_id");
        entity.Property(song => song.PrimaryGenreId).HasColumnName("primary_genre_id");
        entity.Property(song => song.DurationSeconds).HasColumnName("duration_seconds");
        entity.Property(song => song.TempoBpm).HasColumnName("tempo_bpm");
        entity.Property(song => song.Energy).HasColumnName("energy").IsRequired();
        entity.Property(song => song.Darkness).HasColumnName("darkness").IsRequired();
        entity.Property(song => song.Danceability).HasColumnName("danceability").IsRequired();
        entity.Property(song => song.Valence).HasColumnName("valence").IsRequired();
        entity.Property(song => song.Instrumentalness).HasColumnName("instrumentalness").IsRequired();
        entity.Property(song => song.Description).HasColumnName("description");
        entity.Property(song => song.PreviewUrl).HasColumnName("preview_url");
        entity.Property(song => song.ExternalUrl).HasColumnName("external_url");
        entity.Property(song => song.CoverTheme).HasColumnName("cover_theme").HasMaxLength(100);
        entity.Property(song => song.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
        entity.Property(song => song.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz");

        entity.HasOne(song => song.PrimaryArtist)
            .WithMany(artist => artist.Songs)
            .HasForeignKey(song => song.PrimaryArtistId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(song => song.Album)
            .WithMany(album => album.Songs)
            .HasForeignKey(song => song.AlbumId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(song => song.PrimaryGenre)
            .WithMany(genre => genre.PrimaryGenreSongs)
            .HasForeignKey(song => song.PrimaryGenreId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(song => song.PrimaryArtistId);
        entity.HasIndex(song => song.AlbumId);
        entity.HasIndex(song => song.PrimaryGenreId);
        entity.HasIndex(song => song.Energy);
        entity.HasIndex(song => song.Darkness);
        entity.HasIndex(song => song.Danceability);
        entity.HasIndex(song => song.Valence);
        entity.HasIndex(song => song.Instrumentalness);
        entity.HasIndex(song => new { song.Title, song.PrimaryArtistId, song.AlbumId }).IsUnique();
    }

    private static void ConfigureSongGenres(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<SongGenre>();

        entity.ToTable("song_genres", table =>
        {
            table.HasCheckConstraint("ck_song_genres_weight_range", "weight BETWEEN 1 AND 10");
        });

        entity.HasKey(songGenre => new { songGenre.SongId, songGenre.GenreId });

        entity.Property(songGenre => songGenre.SongId).HasColumnName("song_id");
        entity.Property(songGenre => songGenre.GenreId).HasColumnName("genre_id");
        entity.Property(songGenre => songGenre.Weight).HasColumnName("weight").IsRequired();

        entity.HasOne(songGenre => songGenre.Song)
            .WithMany(song => song.SongGenres)
            .HasForeignKey(songGenre => songGenre.SongId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(songGenre => songGenre.Genre)
            .WithMany(genre => genre.SongGenres)
            .HasForeignKey(songGenre => songGenre.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(songGenre => songGenre.GenreId);
    }

    private static void ConfigureMoodTags(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<MoodTag>();

        entity.ToTable("mood_tags");

        entity.HasKey(moodTag => moodTag.MoodTagId);

        entity.Property(moodTag => moodTag.MoodTagId).HasColumnName("mood_tag_id");
        entity.Property(moodTag => moodTag.Name).HasColumnName("name").HasMaxLength(80).IsRequired();
        entity.Property(moodTag => moodTag.Slug).HasColumnName("slug").HasMaxLength(100).IsRequired();
        entity.Property(moodTag => moodTag.Description).HasColumnName("description");
        entity.Property(moodTag => moodTag.DisplayIcon).HasColumnName("display_icon").HasMaxLength(30);
        entity.Property(moodTag => moodTag.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        entity.HasIndex(moodTag => moodTag.Name).IsUnique();
        entity.HasIndex(moodTag => moodTag.Slug).IsUnique();
    }

    private static void ConfigureVibeTags(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<VibeTag>();

        entity.ToTable("vibe_tags");

        entity.HasKey(vibeTag => vibeTag.VibeTagId);

        entity.Property(vibeTag => vibeTag.VibeTagId).HasColumnName("vibe_tag_id");
        entity.Property(vibeTag => vibeTag.Name).HasColumnName("name").HasMaxLength(80).IsRequired();
        entity.Property(vibeTag => vibeTag.Slug).HasColumnName("slug").HasMaxLength(100).IsRequired();
        entity.Property(vibeTag => vibeTag.Description).HasColumnName("description");
        entity.Property(vibeTag => vibeTag.DisplayIcon).HasColumnName("display_icon").HasMaxLength(30);
        entity.Property(vibeTag => vibeTag.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        entity.HasIndex(vibeTag => vibeTag.Name).IsUnique();
        entity.HasIndex(vibeTag => vibeTag.Slug).IsUnique();
    }

    private static void ConfigureSongMoodTags(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<SongMoodTag>();

        entity.ToTable("song_mood_tags", table =>
        {
            table.HasCheckConstraint("ck_song_mood_tags_weight_range", "weight BETWEEN 1 AND 10");
        });

        entity.HasKey(songMoodTag => new { songMoodTag.SongId, songMoodTag.MoodTagId });

        entity.Property(songMoodTag => songMoodTag.SongId).HasColumnName("song_id");
        entity.Property(songMoodTag => songMoodTag.MoodTagId).HasColumnName("mood_tag_id");
        entity.Property(songMoodTag => songMoodTag.Weight).HasColumnName("weight").IsRequired();

        entity.HasOne(songMoodTag => songMoodTag.Song)
            .WithMany(song => song.SongMoodTags)
            .HasForeignKey(songMoodTag => songMoodTag.SongId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(songMoodTag => songMoodTag.MoodTag)
            .WithMany(moodTag => moodTag.SongMoodTags)
            .HasForeignKey(songMoodTag => songMoodTag.MoodTagId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(songMoodTag => songMoodTag.MoodTagId);
    }

    private static void ConfigureSongVibeTags(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<SongVibeTag>();

        entity.ToTable("song_vibe_tags", table =>
        {
            table.HasCheckConstraint("ck_song_vibe_tags_weight_range", "weight BETWEEN 1 AND 10");
        });

        entity.HasKey(songVibeTag => new { songVibeTag.SongId, songVibeTag.VibeTagId });

        entity.Property(songVibeTag => songVibeTag.SongId).HasColumnName("song_id");
        entity.Property(songVibeTag => songVibeTag.VibeTagId).HasColumnName("vibe_tag_id");
        entity.Property(songVibeTag => songVibeTag.Weight).HasColumnName("weight").IsRequired();

        entity.HasOne(songVibeTag => songVibeTag.Song)
            .WithMany(song => song.SongVibeTags)
            .HasForeignKey(songVibeTag => songVibeTag.SongId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(songVibeTag => songVibeTag.VibeTag)
            .WithMany(vibeTag => vibeTag.SongVibeTags)
            .HasForeignKey(songVibeTag => songVibeTag.VibeTagId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(songVibeTag => songVibeTag.VibeTagId);
    }

    private static void ConfigureRecommendationSessions(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<RecommendationSession>();

        entity.ToTable("recommendation_sessions", table =>
        {
            table.HasCheckConstraint("ck_recommendation_sessions_target_energy_range", "target_energy IS NULL OR target_energy BETWEEN 1 AND 10");
            table.HasCheckConstraint("ck_recommendation_sessions_target_darkness_range", "target_darkness IS NULL OR target_darkness BETWEEN 1 AND 10");
            table.HasCheckConstraint("ck_recommendation_sessions_target_danceability_range", "target_danceability IS NULL OR target_danceability BETWEEN 1 AND 10");
            table.HasCheckConstraint("ck_recommendation_sessions_target_valence_range", "target_valence IS NULL OR target_valence BETWEEN 1 AND 10");
            table.HasCheckConstraint("ck_recommendation_sessions_target_instrumentalness_range", "target_instrumentalness IS NULL OR target_instrumentalness BETWEEN 1 AND 10");
        });

        entity.HasKey(session => session.RecommendationSessionId);

        entity.Property(session => session.RecommendationSessionId).HasColumnName("recommendation_session_id").HasColumnType("uuid");
        entity.Property(session => session.SelectedMoodTagIdsJson).HasColumnName("selected_mood_tag_ids_json");
        entity.Property(session => session.SelectedGenreIdsJson).HasColumnName("selected_genre_ids_json");
        entity.Property(session => session.SelectedVibeTagIdsJson).HasColumnName("selected_vibe_tag_ids_json");
        entity.Property(session => session.TargetEnergy).HasColumnName("target_energy");
        entity.Property(session => session.TargetDarkness).HasColumnName("target_darkness");
        entity.Property(session => session.TargetDanceability).HasColumnName("target_danceability");
        entity.Property(session => session.TargetValence).HasColumnName("target_valence");
        entity.Property(session => session.TargetInstrumentalness).HasColumnName("target_instrumentalness");
        entity.Property(session => session.Source).HasColumnName("source").HasMaxLength(50).IsRequired();
        entity.Property(session => session.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();
    }

    private static void ConfigureRecommendationResults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<RecommendationResult>();

        entity.ToTable("recommendation_results", table =>
        {
            table.HasCheckConstraint("ck_recommendation_results_rank_positive", "rank > 0");
            table.HasCheckConstraint("ck_recommendation_results_score_non_negative", "score >= 0");
        });

        entity.HasKey(result => result.RecommendationResultId);

        entity.Property(result => result.RecommendationResultId).HasColumnName("recommendation_result_id");
        entity.Property(result => result.RecommendationSessionId).HasColumnName("recommendation_session_id").HasColumnType("uuid");
        entity.Property(result => result.SongId).HasColumnName("song_id");
        entity.Property(result => result.Rank).HasColumnName("rank").IsRequired();
        entity.Property(result => result.Score).HasColumnName("score").HasColumnType("double precision").IsRequired();
        entity.Property(result => result.ReasonsJson).HasColumnName("reasons_json").IsRequired();
        entity.Property(result => result.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        entity.HasOne(result => result.RecommendationSession)
            .WithMany(session => session.RecommendationResults)
            .HasForeignKey(result => result.RecommendationSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(result => result.Song)
            .WithMany(song => song.RecommendationResults)
            .HasForeignKey(result => result.SongId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(result => result.RecommendationSessionId);
        entity.HasIndex(result => result.SongId);
        entity.HasIndex(result => new { result.RecommendationSessionId, result.SongId }).IsUnique();
        entity.HasIndex(result => new { result.RecommendationSessionId, result.Rank }).IsUnique();
    }
}