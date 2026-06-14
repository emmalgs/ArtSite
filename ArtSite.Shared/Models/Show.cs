namespace ArtSite.Shared.Models;

public class Show
{
  public int ShowId { get; set; }
  public string Title { get; set; } = string.Empty;
  public int? LocationId { get; set; }
  public string? Dates { get; set; }
  public string? ShowInfo { get; set; }
  public string? ShowType { get; set; }
  public string? ArtistStatement { get; set; }

  // Navigation properties
  public Location? Location { get; set; }
  public List<ShowArtwork> ShowArtworks { get; set; } = [];
  public List<ShowImage> ShowImages { get; set; } = [];
}
