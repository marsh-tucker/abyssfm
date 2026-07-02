namespace AbyssFm.Api.Models;

public class SongMoodTag
{
    public int SongId { get; set; }

    public Song? Song { get; set; }

    public int MoodTagId { get; set; }

    public MoodTag? MoodTag { get; set; }

    public short Weight { get; set; }
}
