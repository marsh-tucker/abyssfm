using AbyssFm.Api.DTOs.Recommendations;
using AbyssFm.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbyssFm.Api.Controllers;

[ApiController]
[Route("api/recommendations")]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationsController(
        IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpPost]
    public async Task<ActionResult<List<RecommendationResultDto>>> GetRecommendations(
        [FromBody] RecommendationRequestDto request)
    {
        if (request.GenreIds.Count == 0)
        {
            return BadRequest(new
            {
                message = "Please select at least one genre."
            });
        }

        if (request.MoodTagIds.Count == 0)
        {
            return BadRequest(new
            {
                message = "Please select at least one mood."
            });
        }

        var numericValues = new short?[]
        {
            request.Energy,
            request.Darkness,
            request.Danceability,
            request.Valence,
            request.Instrumentalness
        };

        var numericValuesProvided =
            numericValues.Count(value => value.HasValue);

        if (numericValuesProvided > 0 &&
            numericValuesProvided < numericValues.Length)
        {
            return BadRequest(new
            {
                message = "Please provide all five numeric preferences or none of them."
            });
        }

        if (numericValuesProvided == numericValues.Length &&
            numericValues.Any(value => value < 1 || value > 10))
        {
            return BadRequest(new
            {
                message = "Numeric preferences must be between 1 and 10."
            });
        }

        var results =
            await _recommendationService.GetRecommendationsAsync(request);

        return Ok(results);
    }
}