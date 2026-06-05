using ArtSite.Shared.DTOs;

namespace ArtSite.Api.Services;
public interface ILocationService
{
    Task<List<LocationDto>> GetAllAsync();
    Task<LocationDto?> GetByIdAsync(Guid id);
    Task<LocationDto> CreateAsync(LocationDto location);
    Task<LocationDto?> UpdateAsync(LocationDto location);
    Task<bool> DeleteAsync(Guid id);
}