namespace AbyssFm.Api.Models;

public class MoodTag
{
    public int MoodTagId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? DisplayIcon { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SongMoodTag> SongMoodTags { get; set; } = new List<SongMoodTag>();
}
