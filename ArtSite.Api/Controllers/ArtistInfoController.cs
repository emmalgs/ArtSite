using ArtSite.Api.Services;
using ArtSite.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistInfoController : ControllerBase
{
  private readonly IArtistInfoService _artistInfoService;

  public ArtistInfoController(IArtistInfoService artistInfoService)
  {
    _artistInfoService = artistInfoService;
  }

  [HttpGet]
  public async Task<ActionResult<ArtistInfoDto>> GetCurrent()
  {
    var artistInfo = await _artistInfoService.GetCurrentAsync();

    if (artistInfo == null)
      return NotFound(new { message = "No artist info found" });

    return Ok(artistInfo);
  }

  [HttpGet("versions")]
  public async Task<ActionResult<List<ArtistInfoVersionDto>>> GetVersionHistory()
  {
    var versions = await _artistInfoService.GetVersionHistoryAsync();
    return Ok(versions);
  }

  [HttpGet("versions/{versionId}")]
  public async Task<ActionResult<ArtistInfoVersionDto>> GetVersion(int versionId)
  {
    var version = await _artistInfoService.GetVersionByIdAsync(versionId);

    if (version == null)
      return NotFound(new { message = $"Version with ID {versionId} not found" });

    return Ok(version);
  }

  [HttpPut]
  [Authorize]
  public async Task<ActionResult<ArtistInfoDto>> CreateOrUpdate([FromBody] UpdateArtistInfoDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var artistInfo = await _artistInfoService.CreateOrUpdateAsync(dto);
    return Ok(artistInfo);
  }

  [HttpPost("restore/{versionId}")]
  [Authorize]
  public async Task<ActionResult<ArtistInfoDto>> RestoreVersion(int versionId)
  {
    var artistInfo = await _artistInfoService.RestoreVersionAsync(versionId);

    if (artistInfo == null)
      return NotFound(new { message = $"Version with ID {versionId} not found" });

    return Ok(artistInfo);
  }
}
