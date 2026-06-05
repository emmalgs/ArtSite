using ArtSite.Shared.DTOs;

namespace ArtSite.Api.Services;
public interface IArtworkService
{
    Task<List<ArtworkDto>> GetAllAsync();
    Task<ArtworkDetailDto?> GetByIdAsync(int id);
    Task<ArtworkDetailDto> CreateAsync(CreateArtworkDto dto);
    Task<ArtworkDetailDto?> UpdateAsync(int id, UpdateArtworkDto dto);
    Task<bool> DeleteAsync(int id);
}