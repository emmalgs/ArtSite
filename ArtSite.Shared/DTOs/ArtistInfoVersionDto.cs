namespace ArtSite.Shared.DTOs;

public class ArtistInfoVersionDto
{
  public int ArtistInfoVersionId { get; set; }
  public string? CV { get; set; }
  public string? Bio { get; set; }
  public string? ArtistStatement { get; set; }
  public DateTime VersionCreatedAt { get; set; }
  public string ChangeDescription { get; set; } = string.Empty;
}
