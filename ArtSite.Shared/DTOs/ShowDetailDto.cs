namespace ArtSite.Shared.DTOs;

public class ShowDetailDto
{
  public int ShowId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string? Dates { get; set; }
  public string? ShowInfo { get; set; }
  public string? ShowType { get; set; }
  public string? ArtistStatement { get; set; }
  public LocationDto? Location { get; set; }
  public List<ArtworkDto>? Artworks { get; set; }
  public List<ShowImageDto>? Images { get; set; }
}
