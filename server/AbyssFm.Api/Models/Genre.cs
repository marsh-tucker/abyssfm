namespace AbyssFm.Api.Models;

public class Genre
{
    public int GenreId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Song> PrimaryGenreSongs { get; set; } = new List<Song>();

    public ICollection<SongGenre> SongGenres { get; set; } = new List<SongGenre>();
}
