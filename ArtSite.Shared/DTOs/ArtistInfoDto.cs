namespace ArtSite.Shared.DTOs;

public class ArtistInfoDto
{
  public int ArtistInfoId { get; set; }
  public string? CV { get; set; }
  public string? Bio { get; set; }
  public string? ArtistStatement { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}
