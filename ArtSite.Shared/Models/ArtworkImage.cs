namespace ArtSite.Shared.Models;

public class ArtworkImage
{
  public int ArtworkImageId { get; set; }
  public int ArtWorkId { get; set; }
  public ArtWork? ArtWork { get; set; }
  public string BucketPath { get; set; } = string.Empty;
  public string? AltText { get; set; }
  public bool IsPrimary { get; set; }
}