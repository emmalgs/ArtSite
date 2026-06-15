using ArtSite.Api.Data;
using ArtSite.Shared.DTOs;
using ArtSite.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtSite.Api.Services;

public class ShowService : IShowService
{
  private readonly AppDbContext _context;

  public ShowService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<ShowDto>> GetAllAsync()
  {
    return await _context.Shows
      .Include(s => s.Location)
      .Select(s => new ShowDto
      {
        ShowId = s.ShowId,
        Title = s.Title,
        Dates = s.Dates,
        ShowType = s.ShowType,
        LocationId = s.LocationId,
        LocationName = s.Location != null ? s.Location.Name : null
      })
      .ToListAsync();
  }

  public async Task<ShowDetailDto?> GetByIdAsync(int id)
  {
    var show = await _context.Shows
      .Include(s => s.Location)
      .Include(s => s.ShowArtworks)
        .ThenInclude(sa => sa.ArtWork)
      .Include(s => s.ShowImages)
      .FirstOrDefaultAsync(s => s.ShowId == id);

    if (show == null)
      return null;

    return new ShowDetailDto
    {
      ShowId = show.ShowId,
      Title = show.Title,
      Dates = show.Dates,
      ShowInfo = show.ShowInfo,
      ShowType = show.ShowType,
      ArtistStatement = show.ArtistStatement,
      Location = show.Location != null ? new LocationDto
      {
        LocationId = show.Location.LocationId,
        Name = show.Location.Name,
        City = show.Location.City,
        State = show.Location.State,
        Country = show.Location.Country,
        Url = show.Location.Url,
        Email = show.Location.Email,
        Phone = show.Location.Phone,
        CollectionType = show.Location.CollectionType
      } : null,
      Artworks = show.ShowArtworks.Select(sa => new ArtworkDto
      {
        ArtWorkId = sa.ArtWork.ArtWorkId,
        Title = sa.ArtWork.Title,
        Medium = sa.ArtWork.Medium,
        Category = sa.ArtWork.Category,
        Year = sa.ArtWork.Year,
        Price = sa.ArtWork.Price,
        Available = sa.ArtWork.Available,
        PrimaryImageUrl = _context.ArtworkImages
          .Where(ai => ai.ArtWorkId == sa.ArtWork.ArtWorkId && ai.IsPrimary)
          .Select(ai => ai.BucketPath)
          .FirstOrDefault()
      }).ToList(),
      Images = show.ShowImages.Select(si => new ShowImageDto
      {
        ShowImageId = si.ShowImageId,
        ImageUrl = si.BucketPath,
        AltText = si.AltText,
        ImageType = si.ImageType
      }).ToList()
    };
  }

  public async Task<ShowDetailDto> CreateAsync(CreateShowDto dto)
  {
    var show = new Show
    {
      Title = dto.Title,
      Dates = dto.Dates,
      ShowInfo = dto.ShowInfo,
      ShowType = dto.ShowType,
      ArtistStatement = dto.ArtistStatement,
      LocationId = dto.LocationId
    };

    _context.Shows.Add(show);
    await _context.SaveChangesAsync();

    // Add artworks to show if provided
    if (dto.ArtworkIds != null && dto.ArtworkIds.Any())
    {
      var showArtworks = dto.ArtworkIds.Select(artworkId => new ShowArtwork
      {
        ShowId = show.ShowId,
        ArtWorkId = artworkId
      }).ToList();

      _context.ShowArtworks.AddRange(showArtworks);
      await _context.SaveChangesAsync();
    }

    return (await GetByIdAsync(show.ShowId))!;
  }

  public async Task<ShowDetailDto?> UpdateAsync(int id, UpdateShowDto dto)
  {
    var show = await _context.Shows
      .Include(s => s.ShowArtworks)
      .FirstOrDefaultAsync(s => s.ShowId == id);

    if (show == null)
      return null;

    show.Title = dto.Title;
    show.Dates = dto.Dates;
    show.ShowInfo = dto.ShowInfo;
    show.ShowType = dto.ShowType;
    show.ArtistStatement = dto.ArtistStatement;
    show.LocationId = dto.LocationId;

    // Update artworks - remove all existing and add new ones
    if (dto.ArtworkIds != null)
    {
      // Remove existing artworks
      _context.ShowArtworks.RemoveRange(show.ShowArtworks);

      // Add new artworks
      var newShowArtworks = dto.ArtworkIds.Select(artworkId => new ShowArtwork
      {
        ShowId = show.ShowId,
        ArtWorkId = artworkId
      }).ToList();

      _context.ShowArtworks.AddRange(newShowArtworks);
    }

    await _context.SaveChangesAsync();

    return await GetByIdAsync(id);
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var show = await _context.Shows.FindAsync(id);

    if (show == null)
      return false;

    _context.Shows.Remove(show);
    await _context.SaveChangesAsync();

    return true;
  }
}
