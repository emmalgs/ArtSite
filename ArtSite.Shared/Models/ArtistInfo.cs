namespace ArtSite.Shared.Models;

public class ArtistInfo
{
  public int ArtistInfoId { get; set; }
  public string? CV { get; set; }
  public string? Bio { get; set; }
  public string? ArtistStatement { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }

  public List<ArtistInfoVersion> Versions { get; set; } = [];
}
