namespace ArtSite.Shared.DTOs;

public class ShowImageDto
{
  public int ShowImageId { get; set; }
  public string ImageUrl { get; set; } = string.Empty;
  public string? AltText { get; set; }
  public string? ImageType { get; set; }
}
