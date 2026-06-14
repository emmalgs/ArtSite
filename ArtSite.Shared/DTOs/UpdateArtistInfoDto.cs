namespace ArtSite.Shared.DTOs;

public class UpdateArtistInfoDto
{
  public string? CV { get; set; }
  public string? Bio { get; set; }
  public string? ArtistStatement { get; set; }
  public string ChangeDescription { get; set; } = string.Empty;
}
