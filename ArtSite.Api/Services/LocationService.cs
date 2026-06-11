using ArtSite.Api.Data;
using ArtSite.Shared.DTOs;
using ArtSite.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtSite.Api.Services;

public class LocationService : ILocationService
{
  private readonly AppDbContext _context;

  public LocationService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<LocationDto>> GetAllAsync()
  {
    return await _context.Locations
      .Select(l => new LocationDto
      {
        LocationId = l.LocationId,
        Name = l.Name,
        City = l.City,
        State = l.State,
        Country = l.Country,
        Url = l.Url,
        Email = l.Email,
        Phone = l.Phone,
        CollectionType = l.CollectionType
      })
      .ToListAsync();
  }

  public async Task<LocationDto?> GetByIdAsync(int id)
  {
    var location = await _context.Locations.FindAsync(id);

    if (location == null)
      return null;

    return new LocationDto
    {
      LocationId = location.LocationId,
      Name = location.Name,
      City = location.City,
      State = location.State,
      Country = location.Country,
      Url = location.Url,
      Email = location.Email,
      Phone = location.Phone,
      CollectionType = location.CollectionType
    };
  }

  public async Task<LocationDto> CreateAsync(LocationDto dto)
  {
    var location = new Location
    {
      Name = dto.Name,
      City = dto.City,
      State = dto.State,
      Country = dto.Country,
      Email = dto.Email,
      Url = dto.Url,
      Phone = dto.Phone,
      CollectionType = dto.CollectionType
    };

    _context.Locations.Add(location);
    await _context.SaveChangesAsync();

    return (await GetByIdAsync(location.LocationId))!;
  }

  public async Task<LocationDto?> UpdateAsync(int id, LocationDto dto)
  {
    var location = await _context.Locations.FindAsync(id);

    if (location == null)
      return null;

    location.Name = dto.Name;
    location.City = dto.City;
    location.Country = dto.Country;
    location.State = dto.State;
    location.Email = dto.Email;
    location.Phone = dto.Phone;
    location.CollectionType = dto.CollectionType;
    location.Url = dto.Url;

    await _context.SaveChangesAsync();

    return await GetByIdAsync(id);
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var location = await _context.Locations.FindAsync(id);

    if (location == null)
      return false;

    _context.Locations.Remove(location);
    await _context.SaveChangesAsync();

    return true;
  }
}