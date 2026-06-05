namespace ArtSite.Shared.Models;

public class ArtWork
{
  public int ArtWorkId { get; set; }

  public string Title { get; set; } = string.Empty;
  public string? Medium { get; set; }
  public string? Category { get; set; }
  public string? Dimensions { get; set; }
  public int Year { get; set; }
  public bool Available { get; set; }
  public int LocationId { get; set; }
  public List<ArtworkImage> ArtworkImages { get; set; } = [];
}
