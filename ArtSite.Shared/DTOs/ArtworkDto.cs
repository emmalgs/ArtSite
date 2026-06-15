namespace ArtSite.Shared.DTOs;

public class ArtworkDto
{
  public int ArtWorkId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string? Medium { get; set; }
  public string? Category { get; set; }
  public int Year { get; set; }
  public decimal? Price { get; set; }
  public bool Available { get; set; }
  public string? PrimaryImageUrl { get; set; }
}