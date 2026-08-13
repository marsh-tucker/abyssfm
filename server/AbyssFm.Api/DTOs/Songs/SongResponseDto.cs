namespace AbyssFm.Api.DTOs.Songs;

public class SongResponseDto
{
    public int SongId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    public string? Album { get; set; }

    public string PrimaryGenre { get; set; } = string.Empty;

    public int? DurationSeconds { get; set; }

    public int? TempoBpm { get; set; }

    public short Energy { get; set; }

    public short Darkness { get; set; }

    public short Danceability { get; set; }

    public short Valence { get; set; }

    public short Instrumentalness { get; set; }

    public string? Description { get; set; }

    public string? CoverTheme { get; set; }

    public List<WeightedTagResponseDto> Genres { get; set; } = new();

    public List<WeightedTagResponseDto> MoodTags { get; set; } = new();

    public List<WeightedTagResponseDto> VibeTags { get; set; } = new();
}