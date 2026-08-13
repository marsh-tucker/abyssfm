using AbyssFm.Api.Data;
using AbyssFm.Api.DTOs.Songs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbyssFm.Api.Controllers;

[ApiController]
[Route("api/songs")]
public class SongsController : ControllerBase
{
    private readonly AbyssFmDbContext _dbContext;

    public SongsController(AbyssFmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<SongResponseDto>>> GetSongs()
    {
        var songs = await _dbContext.Songs
            .AsNoTracking()
            .OrderBy(song => song.Title)
            .Select(song => new SongResponseDto
            {
                SongId = song.SongId,
                Title = song.Title,

                Artist = song.PrimaryArtist!.Name,

                Album = song.Album != null
                    ? song.Album.Title
                    : null,

                PrimaryGenre = song.PrimaryGenre!.Name,

                DurationSeconds = song.DurationSeconds,
                TempoBpm = song.TempoBpm,

                Energy = song.Energy,
                Darkness = song.Darkness,
                Danceability = song.Danceability,
                Valence = song.Valence,
                Instrumentalness = song.Instrumentalness,

                Description = song.Description,
                CoverTheme = song.CoverTheme,

                Genres = song.SongGenres
                    .OrderByDescending(songGenre => songGenre.Weight)
                    .Select(songGenre => new WeightedTagResponseDto
                    {
                        Name = songGenre.Genre!.Name,
                        Weight = songGenre.Weight
                    })
                    .ToList(),

                MoodTags = song.SongMoodTags
                    .OrderByDescending(songMood => songMood.Weight)
                    .Select(songMood => new WeightedTagResponseDto
                    {
                        Name = songMood.MoodTag!.Name,
                        Weight = songMood.Weight
                    })
                    .ToList(),

                VibeTags = song.SongVibeTags
                    .OrderByDescending(songVibe => songVibe.Weight)
                    .Select(songVibe => new WeightedTagResponseDto
                    {
                        Name = songVibe.VibeTag!.Name,
                        Weight = songVibe.Weight
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(songs);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SongResponseDto>> GetSongById(int id)
    {
        var song = await _dbContext.Songs
            .AsNoTracking()
            .Where(song => song.SongId == id)
            .Select(song => new SongResponseDto
            {
                SongId = song.SongId,
                Title = song.Title,

                Artist = song.PrimaryArtist!.Name,

                Album = song.Album != null
                    ? song.Album.Title
                    : null,

                PrimaryGenre = song.PrimaryGenre!.Name,

                DurationSeconds = song.DurationSeconds,
                TempoBpm = song.TempoBpm,

                Energy = song.Energy,
                Darkness = song.Darkness,
                Danceability = song.Danceability,
                Valence = song.Valence,
                Instrumentalness = song.Instrumentalness,

                Description = song.Description,
                CoverTheme = song.CoverTheme,

                Genres = song.SongGenres
                    .OrderByDescending(songGenre => songGenre.Weight)
                    .Select(songGenre => new WeightedTagResponseDto
                    {
                        Name = songGenre.Genre!.Name,
                        Weight = songGenre.Weight
                    })
                    .ToList(),

                MoodTags = song.SongMoodTags
                    .OrderByDescending(songMood => songMood.Weight)
                    .Select(songMood => new WeightedTagResponseDto
                    {
                        Name = songMood.MoodTag!.Name,
                        Weight = songMood.Weight
                    })
                    .ToList(),

                VibeTags = song.SongVibeTags
                    .OrderByDescending(songVibe => songVibe.Weight)
                    .Select(songVibe => new WeightedTagResponseDto
                    {
                        Name = songVibe.VibeTag!.Name,
                        Weight = songVibe.Weight
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (song is null)
        {
            return NotFound();
        }

        return Ok(song);
    }
}