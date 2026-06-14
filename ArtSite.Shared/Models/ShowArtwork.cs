namespace ArtSite.Shared.Models;

public class ShowArtwork
{
  public int ShowArtworkId { get; set; }
  public int ShowId { get; set; }
  public int ArtWorkId { get; set; }

  // Navigation properties
  public Show Show { get; set; } = null!;
  public ArtWork ArtWork { get; set; } = null!;
}
