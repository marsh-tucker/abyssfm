namespace AbyssFm.Api.Models;

public class Song
{
    public int SongId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int PrimaryArtistId { get; set; }

    public Artist? PrimaryArtist { get; set; }

    public int? AlbumId { get; set; }

    public Album? Album { get; set; }

    public int PrimaryGenreId { get; set; }

    public Genre? PrimaryGenre { get; set; }

    public int? DurationSeconds { get; set; }

    public int? TempoBpm { get; set; }

    public short Energy { get; set; }

    public short Darkness { get; set; }

    public short Danceability { get; set; }

    public short Valence { get; set; }

    public short Instrumentalness { get; set; }

    public string? Description { get; set; }

    public string? PreviewUrl { get; set; }

    public string? ExternalUrl { get; set; }

    public string? CoverTheme { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<SongGenre> SongGenres { get; set; } = new List<SongGenre>();

    public ICollection<SongMoodTag> SongMoodTags { get; set; } = new List<SongMoodTag>();

    public ICollection<SongVibeTag> SongVibeTags { get; set; } = new List<SongVibeTag>();

    public ICollection<RecommendationResult> RecommendationResults { get; set; } = new List<RecommendationResult>();
}
