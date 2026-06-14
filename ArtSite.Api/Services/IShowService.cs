using ArtSite.Shared.DTOs;

namespace ArtSite.Api.Services;

public interface IShowService
{
  Task<List<ShowDto>> GetAllAsync();
  Task<ShowDetailDto?> GetByIdAsync(int id);
  Task<ShowDetailDto> CreateAsync(CreateShowDto dto);
  Task<ShowDetailDto?> UpdateAsync(int id, UpdateShowDto dto);
  Task<bool> DeleteAsync(int id);
}
