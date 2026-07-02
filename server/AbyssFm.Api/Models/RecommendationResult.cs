namespace AbyssFm.Api.Models;

public class RecommendationResult
{
    public int RecommendationResultId { get; set; }

    public Guid RecommendationSessionId { get; set; }

    public RecommendationSession? RecommendationSession { get; set; }

    public int SongId { get; set; }

    public Song? Song { get; set; }

    public int Rank { get; set; }

    public double Score { get; set; }

    public string ReasonsJson { get; set; } = "[]";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
