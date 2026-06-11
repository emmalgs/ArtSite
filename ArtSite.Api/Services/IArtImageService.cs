using ArtSite.Shared.DTOs;

namespace ArtSite.Api.Services;
public interface IArtImageService
{
    Task<List<ArtworkImageDto>> GetAllAsync();
    Task<ArtworkImageDto?> GetByIdAsync(int id);
    Task<ArtworkImageDto> CreateAsync(ArtworkImageDto artImageDto);
    Task<ArtworkImageDto?> UpdateAsync(ArtworkImageDto artImageDto);
    Task<bool> DeleteAsync(int id);
}