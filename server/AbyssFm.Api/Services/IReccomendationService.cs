using AbyssFm.Api.DTOs.Recommendations;

namespace AbyssFm.Api.Services;

public interface IRecommendationService
{
    Task<List<RecommendationResultDto>> GetRecommendationsAsync(
        RecommendationRequestDto request);
}