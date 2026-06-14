namespace ArtSite.Shared.DTOs;

public class CreateArtworkDto
{
  public string Title { get; set; } = string.Empty;
  public string? Medium { get; set; }
  public string? Category { get; set; }
  public string? Dimensions { get; set; }
  public int Year { get; set; }
  public decimal? Price { get; set; }
  public bool Available { get; set; }
  public int? LocationId { get; set; }
}