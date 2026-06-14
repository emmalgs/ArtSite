using ArtSite.Api.Data;
using ArtSite.Shared.DTOs;
using ArtSite.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtSite.Api.Services;

public class ArtistInfoService : IArtistInfoService
{
  private readonly AppDbContext _context;

  public ArtistInfoService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<ArtistInfoDto?> GetCurrentAsync()
  {
    var artistInfo = await _context.ArtistInfos
      .OrderByDescending(ai => ai.UpdatedAt)
      .FirstOrDefaultAsync();

    if (artistInfo == null)
      return null;

    return new ArtistInfoDto
    {
      ArtistInfoId = artistInfo.ArtistInfoId,
      CV = artistInfo.CV,
      Bio = artistInfo.Bio,
      ArtistStatement = artistInfo.ArtistStatement,
      CreatedAt = artistInfo.CreatedAt,
      UpdatedAt = artistInfo.UpdatedAt
    };
  }

  public async Task<List<ArtistInfoVersionDto>> GetVersionHistoryAsync()
  {
    var current = await _context.ArtistInfos
      .OrderByDescending(ai => ai.UpdatedAt)
      .FirstOrDefaultAsync();

    if (current == null)
      return new List<ArtistInfoVersionDto>();

    return await _context.ArtistInfoVersions
      .Where(v => v.ArtistInfoId == current.ArtistInfoId)
      .OrderByDescending(v => v.VersionCreatedAt)
      .Select(v => new ArtistInfoVersionDto
      {
        ArtistInfoVersionId = v.ArtistInfoVersionId,
        CV = v.CV,
        Bio = v.Bio,
        ArtistStatement = v.ArtistStatement,
        VersionCreatedAt = v.VersionCreatedAt,
        ChangeDescription = v.ChangeDescription
      })
      .ToListAsync();
  }

  public async Task<ArtistInfoDto> CreateOrUpdateAsync(UpdateArtistInfoDto dto)
  {
    var current = await _context.ArtistInfos
      .OrderByDescending(ai => ai.UpdatedAt)
      .FirstOrDefaultAsync();

    var now = DateTime.UtcNow;

    if (current == null)
    {
      // Create new artist info
      var newArtistInfo = new ArtistInfo
      {
        CV = dto.CV,
        Bio = dto.Bio,
        ArtistStatement = dto.ArtistStatement,
        CreatedAt = now,
        UpdatedAt = now
      };

      _context.ArtistInfos.Add(newArtistInfo);
      await _context.SaveChangesAsync();

      // Create initial version
      var initialVersion = new ArtistInfoVersion
      {
        ArtistInfoId = newArtistInfo.ArtistInfoId,
        CV = dto.CV,
        Bio = dto.Bio,
        ArtistStatement = dto.ArtistStatement,
        VersionCreatedAt = now,
        ChangeDescription = "Initial version"
      };

      _context.ArtistInfoVersions.Add(initialVersion);
      await _context.SaveChangesAsync();

      return new ArtistInfoDto
      {
        ArtistInfoId = newArtistInfo.ArtistInfoId,
        CV = newArtistInfo.CV,
        Bio = newArtistInfo.Bio,
        ArtistStatement = newArtistInfo.ArtistStatement,
        CreatedAt = newArtistInfo.CreatedAt,
        UpdatedAt = newArtistInfo.UpdatedAt
      };
    }
    else
    {
      // Save current state as a version before updating
      var version = new ArtistInfoVersion
      {
        ArtistInfoId = current.ArtistInfoId,
        CV = dto.CV,
        Bio = dto.Bio,
        ArtistStatement = dto.ArtistStatement,
        VersionCreatedAt = now,
        ChangeDescription = string.IsNullOrEmpty(dto.ChangeDescription)
          ? "Updated artist info"
          : dto.ChangeDescription
      };

      _context.ArtistInfoVersions.Add(version);

      // Update current
      current.CV = dto.CV;
      current.Bio = dto.Bio;
      current.ArtistStatement = dto.ArtistStatement;
      current.UpdatedAt = now;

      await _context.SaveChangesAsync();

      return new ArtistInfoDto
      {
        ArtistInfoId = current.ArtistInfoId,
        CV = current.CV,
        Bio = current.Bio,
        ArtistStatement = current.ArtistStatement,
        CreatedAt = current.CreatedAt,
        UpdatedAt = current.UpdatedAt
      };
    }
  }

  public async Task<ArtistInfoVersionDto?> GetVersionByIdAsync(int versionId)
  {
    var version = await _context.ArtistInfoVersions
      .FirstOrDefaultAsync(v => v.ArtistInfoVersionId == versionId);

    if (version == null)
      return null;

    return new ArtistInfoVersionDto
    {
      ArtistInfoVersionId = version.ArtistInfoVersionId,
      CV = version.CV,
      Bio = version.Bio,
      ArtistStatement = version.ArtistStatement,
      VersionCreatedAt = version.VersionCreatedAt,
      ChangeDescription = version.ChangeDescription
    };
  }

  public async Task<ArtistInfoDto?> RestoreVersionAsync(int versionId)
  {
    var version = await _context.ArtistInfoVersions
      .FirstOrDefaultAsync(v => v.ArtistInfoVersionId == versionId);

    if (version == null)
      return null;

    var current = await _context.ArtistInfos
      .FirstOrDefaultAsync(ai => ai.ArtistInfoId == version.ArtistInfoId);

    if (current == null)
      return null;

    var now = DateTime.UtcNow;

    // Create a version of the current state before restoring
    var backupVersion = new ArtistInfoVersion
    {
      ArtistInfoId = current.ArtistInfoId,
      CV = current.CV,
      Bio = current.Bio,
      ArtistStatement = current.ArtistStatement,
      VersionCreatedAt = now,
      ChangeDescription = $"Backup before restoring version {versionId}"
    };

    _context.ArtistInfoVersions.Add(backupVersion);

    // Restore from version
    current.CV = version.CV;
    current.Bio = version.Bio;
    current.ArtistStatement = version.ArtistStatement;
    current.UpdatedAt = now;

    await _context.SaveChangesAsync();

    return new ArtistInfoDto
    {
      ArtistInfoId = current.ArtistInfoId,
      CV = current.CV,
      Bio = current.Bio,
      ArtistStatement = current.ArtistStatement,
      CreatedAt = current.CreatedAt,
      UpdatedAt = current.UpdatedAt
    };
  }
}
