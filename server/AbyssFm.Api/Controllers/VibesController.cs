using AbyssFm.Api.Data;
using AbyssFm.Api.DTOs.Vibes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbyssFm.Api.Controllers;

[ApiController]
[Route("api/vibes")]
public class VibesController : ControllerBase
{
    private readonly AbyssFmDbContext _dbContext;

    public VibesController(AbyssFmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<VibeTagResponseDto>>> GetVibes()
    {
        var vibes = await _dbContext.VibeTags
            .AsNoTracking()
            .OrderBy(vibe => vibe.Name)
            .Select(vibe => new VibeTagResponseDto
            {
                VibeTagId = vibe.VibeTagId,
                Name = vibe.Name,
                Slug = vibe.Slug,
                Description = vibe.Description,
                DisplayIcon = vibe.DisplayIcon
            })
            .ToListAsync();

        return Ok(vibes);
    }
}