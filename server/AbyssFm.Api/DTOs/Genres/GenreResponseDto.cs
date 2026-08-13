namespace AbyssFm.Api.DTOs.Genres;
//simply defines: A genre returned by our API has these three fields.
public class GenreResponseDto
{
    public int GenreId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;
}
