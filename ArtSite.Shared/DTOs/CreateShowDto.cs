namespace ArtSite.Shared.DTOs;

public class CreateShowDto
{
  public string Title { get; set; } = string.Empty;
  public string? Dates { get; set; }
  public string? ShowInfo { get; set; }
  public string? ShowType { get; set; }
  public string? ArtistStatement { get; set; }
  public int? LocationId { get; set; }
  public List<int>? ArtworkIds { get; set; }
}
