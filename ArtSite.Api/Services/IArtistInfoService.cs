using ArtSite.Shared.DTOs;

namespace ArtSite.Api.Services;

public interface IArtistInfoService
{
  Task<ArtistInfoDto?> GetCurrentAsync();
  Task<List<ArtistInfoVersionDto>> GetVersionHistoryAsync();
  Task<ArtistInfoDto> CreateOrUpdateAsync(UpdateArtistInfoDto dto);
  Task<ArtistInfoVersionDto?> GetVersionByIdAsync(int versionId);
  Task<ArtistInfoDto?> RestoreVersionAsync(int versionId);
}
