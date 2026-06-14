namespace ArtSite.Shared.Models;

public class ArtistInfoVersion
{
  public int ArtistInfoVersionId { get; set; }
  public int ArtistInfoId { get; set; }
  public string? CV { get; set; }
  public string? Bio { get; set; }
  public string? ArtistStatement { get; set; }
  public DateTime VersionCreatedAt { get; set; }
  public string ChangeDescription { get; set; } = string.Empty;

  public ArtistInfo ArtistInfo { get; set; } = null!;
}
