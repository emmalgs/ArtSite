namespace ArtSite.Shared.DTOs;

public class ShowDto
{
  public int ShowId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string? Dates { get; set; }
  public string? ShowType { get; set; }
  public int? LocationId { get; set; }
  public string? LocationName { get; set; }
}
