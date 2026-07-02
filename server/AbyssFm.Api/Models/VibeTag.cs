namespace AbyssFm.Api.Models;

public class VibeTag
{
    public int VibeTagId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? DisplayIcon { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SongVibeTag> SongVibeTags { get; set; } = new List<SongVibeTag>();
}
