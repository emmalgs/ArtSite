using ArtSite.Api.Data;
using ArtSite.Shared.DTOs;
using ArtSite.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtSite.Api.Services;

public class ArtworkService : IArtworkService
{
  private readonly AppDbContext _context;

  public ArtworkService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<ArtworkDto>> GetAllAsync()
  {
    return await _context.ArtWorks
      .Include(a => a.ArtworkImages)
      .Select(a => new ArtworkDto
      {
        ArtWorkId = a.ArtWorkId,
        Title = a.Title,
        Medium = a.Medium,
        Year = a.Year,
        Available = a.Available,
        PrimaryImageUrl = a.ArtworkImages
          .Where(img => img.IsPrimary)
          .Select(img => img.BucketPath)
          .FirstOrDefault()
      })
      .ToListAsync();
  }

  public async Task<ArtworkDetailDto?> GetByIdAsync(int id)
  {
    var artwork = await _context.ArtWorks
      .Include(a => a.ArtworkImages)
      .FirstOrDefaultAsync(a => a.ArtWorkId == id);

    if (artwork == null)
      return null;

    var location = await _context.Locations
      .FirstOrDefaultAsync(l => l.LocationId == artwork.LocationId);

    return new ArtworkDetailDto
    {
      ArtWorkId = artwork.ArtWorkId,
      Title = artwork.Title,
      Medium = artwork.Medium,
      Category = artwork.Category,
      Dimensions = artwork.Dimensions,
      Year = artwork.Year,
      Available = artwork.Available,
      Location = location == null ? null : new LocationDto
      {
        LocationId = location.LocationId,
        Name = location.Name,
        City = location.City,
        Url = location.Url,
        Email = location.Email,
        Phone = location.Phone,
        CollectionType = location.CollectionType
      },
      Images = [.. artwork.ArtworkImages.Select(img => new
ArtworkImageDto
      {
        ArtworkImageId = img.ArtworkImageId,
        ImageUrl = img.BucketPath,
        AltText = img.AltText,
        IsPrimary = img.IsPrimary
      })]
    };
  }

  public async Task<ArtworkDetailDto> CreateAsync(CreateArtworkDto dto)
  {
    var artwork = new ArtWork
    {
      Title = dto.Title,
      Medium = dto.Medium,
      Category = dto.Category,
      Dimensions = dto.Dimensions,
      Year = dto.Year,
      Available = dto.Available,
      LocationId = dto.LocationId
    };

    _context.ArtWorks.Add(artwork);
    await _context.SaveChangesAsync();

    return (await GetByIdAsync(artwork.ArtWorkId))!;
  }

  public async Task<ArtworkDetailDto?> UpdateAsync(int id, UpdateArtworkDto dto)
  {
    var artwork = await _context.ArtWorks.FindAsync(id);

    if (artwork == null)
      return null;

    artwork.Title = dto.Title;
    artwork.Medium = dto.Medium;
    artwork.Category = dto.Category;
    artwork.Dimensions = dto.Dimensions;
    artwork.Year = dto.Year;
    artwork.Available = dto.Available;
    artwork.LocationId = dto.LocationId;

    await _context.SaveChangesAsync();

    return await GetByIdAsync(id);
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var artwork = await _context.ArtWorks.FindAsync(id);

    if (artwork == null)
      return false;

    _context.ArtWorks.Remove(artwork);
    await _context.SaveChangesAsync();

    return true;
  }
}