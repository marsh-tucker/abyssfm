namespace AbyssFm.Api.Models;

public class RecommendationSession
{
    public Guid RecommendationSessionId { get; set; } = Guid.NewGuid();

    public string? SelectedMoodTagIdsJson { get; set; }

    public string? SelectedGenreIdsJson { get; set; }

    public string? SelectedVibeTagIdsJson { get; set; }

    public short? TargetEnergy { get; set; }

    public short? TargetDarkness { get; set; }

    public short? TargetDanceability { get; set; }

    public short? TargetValence { get; set; }

    public short? TargetInstrumentalness { get; set; }

    public string Source { get; set; } = "frontend";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RecommendationResult> RecommendationResults { get; set; } = new List<RecommendationResult>();
}
