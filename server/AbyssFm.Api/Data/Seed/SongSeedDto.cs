namespace AbyssFm.Api.Data.Seed;

public class SongSeedDto
{
    public string Title { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    public string? Album { get; set; }

    public int? ReleaseYear { get; set; }

    public string PrimaryGenre { get; set; } = string.Empty;

    public List<WeightedTagSeedDto> Genres { get; set; } = new();

    public int? DurationSeconds { get; set; }

    public int? TempoBpm { get; set; }

    public short Energy { get; set; }

    public short Darkness { get; set; }

    public short Danceability { get; set; }

    public short Valence { get; set; }

    public short Instrumentalness { get; set; }

    public string? Description { get; set; }

    public string? CoverTheme { get; set; }

    public List<WeightedTagSeedDto> MoodTags { get; set; } = new();

    public List<WeightedTagSeedDto> VibeTags { get; set; } = new();
}