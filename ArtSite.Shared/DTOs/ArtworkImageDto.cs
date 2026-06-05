namespace ArtSite.Shared.DTOs;

public class ArtworkImageDto
{
  public int ArtworkImageId { get; set; }
  public string ImageUrl { get; set; } = string.Empty;
  public string? AltText { get; set; }
  public bool IsPrimary { get; set; }
}