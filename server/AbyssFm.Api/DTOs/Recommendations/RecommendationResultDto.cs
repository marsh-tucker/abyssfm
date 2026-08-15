using AbyssFm.Api.DTOs.Songs;

namespace AbyssFm.Api.DTOs.Recommendations;

public class RecommendationResultDto
{
    public SongResponseDto Song { get; set; } = new();

    public double Score { get; set; }

    public List<string> Reasons { get; set; } = new();
}