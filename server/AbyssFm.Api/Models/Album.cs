namespace AbyssFm.Api.Models;

public class Album
{
    public int AlbumId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int ArtistId { get; set; }

    public Artist? Artist { get; set; }

    public int? ReleaseYear { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? CoverTheme { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
