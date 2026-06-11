using ArtSite.Shared.DTOs;

namespace ArtSite.Api.Services;
public interface ILocationService
{
    Task<List<LocationDto>> GetAllAsync();
    Task<LocationDto?> GetByIdAsync(int id);
    Task<LocationDto> CreateAsync(LocationDto location);
    Task<LocationDto?> UpdateAsync(int id, LocationDto location);
    Task<bool> DeleteAsync(int id);
}