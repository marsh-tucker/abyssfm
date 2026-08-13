using AbyssFm.Api.Data;
using AbyssFm.Api.DTOs.Moods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbyssFm.Api.Controllers;

[ApiController]
[Route("api/moods")]
public class MoodsController : ControllerBase
{
    private readonly AbyssFmDbContext _dbContext;

    public MoodsController(AbyssFmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<MoodTagResponseDto>>> GetMoods()
    {
        var moods = await _dbContext.MoodTags
            .AsNoTracking()
            .OrderBy(mood => mood.Name)
            .Select(mood => new MoodTagResponseDto
            {
                MoodTagId = mood.MoodTagId,
                Name = mood.Name,
                Slug = mood.Slug,
                Description = mood.Description,
                DisplayIcon = mood.DisplayIcon
            })
            .ToListAsync();

        return Ok(moods);
    }
}