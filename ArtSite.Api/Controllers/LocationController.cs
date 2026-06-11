using ArtSite.Shared.DTOs;
using ArtSite.Shared.Models;
using ArtSite.Api.Services;
using Microsoft.AspNetCore.Mvc;
using ArtSite.Api.Data;

namespace ArtSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
  private readonly ILocationService _locationService;

  public LocationController(ILocationService locationService)
  {
    _locationService = locationService;
  }

  [HttpGet]
  public async Task<ActionResult<List<LocationDto>>> GetAll()
  {
    var locations = await _locationService.GetAllAsync();
    return Ok(locations);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<LocationDto>> GetById(int id)
  {
    var location = await _locationService.GetByIdAsync(id);

    if (location == null)
      return NotFound(new { message = $"Location with ID {id} not found" });

    return Ok(location);
  }

  [HttpPost]
  public async Task<ActionResult<LocationDto>> Create([FromBody] LocationDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var location = await _locationService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = location.LocationId }, location);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<LocationDto>> Update(int id, [FromBody] LocationDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var location = await _locationService.UpdateAsync(id, dto);

    if (location == null)
      return NotFound(new { message = $"Location with ID {id} not found" });

    return Ok(location);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    var success = await _locationService.DeleteAsync(id);

    if (!success)
      return NotFound(new { message = $"Location with ID {id} not found" });

    return NoContent();
  }
}