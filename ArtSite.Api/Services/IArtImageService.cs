using ArtSite.Shared.DTOs;

namespace ArtSite.Api.Services;
public interface IArtImageService
{
    Task<List<ArtImageDto>> GetAllAsync();
    Task<ArtImageDto?> GetByIdAsync(Guid id);
    Task<ArtImageDto> CreateAsync(ArtImageDto artImageDto);
    Task<ArtImageDto?> UpdateAsync(ArtImageDto artImageDto);
    Task<bool> DeleteAsync(Guid id);
}