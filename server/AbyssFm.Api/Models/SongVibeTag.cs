namespace AbyssFm.Api.Models;

public class SongVibeTag
{
    public int SongId { get; set; }

    public Song? Song { get; set; }

    public int VibeTagId { get; set; }

    public VibeTag? VibeTag { get; set; }

    public short Weight { get; set; }
}
