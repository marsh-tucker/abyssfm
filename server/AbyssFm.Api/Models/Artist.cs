namespace AbyssFm.Api.Models;

public class Artist
{
    public int ArtistId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Album> Albums { get; set; } = new List<Album>();

    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
