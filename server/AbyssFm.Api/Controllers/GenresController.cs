
using AbyssFm.Api.Data;
using AbyssFm.Api.DTOs.Genres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbyssFm.Api.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController : ControllerBase
{
    private readonly AbyssFmDbContext _dbContext;

    public GenresController(AbyssFmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<GenreResponseDto>>> GetGenres()
    {
        var genres = await _dbContext.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .Select(genre => new GenreResponseDto
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                Slug = genre.Slug
            })
            .ToListAsync();

        return Ok(genres);
    }
}