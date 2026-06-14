namespace ArtSite.Shared.Models;

public class ShowImage
{
  public int ShowImageId { get; set; }
  public int ShowId { get; set; }
  public string BucketPath { get; set; } = string.Empty;
  public string? AltText { get; set; }
  public string? ImageType { get; set; } // e.g., "postcard", "installation", "event photo"

  // Navigation property
  public Show Show { get; set; } = null!;
}
