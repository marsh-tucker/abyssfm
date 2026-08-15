namespace AbyssFm.Api.DTOs.Recommendations;

public class RecommendationRequestDto
{
    public List<int> GenreIds { get; set; } = new();

    public List<int> MoodTagIds { get; set; } = new();

    public List<int> VibeTagIds { get; set; } = new();

    public short? Energy { get; set; }

    public short? Darkness { get; set; }

    public short? Danceability { get; set; }

    public short? Valence { get; set; }

    public short? Instrumentalness { get; set; }
}