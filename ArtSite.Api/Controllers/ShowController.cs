using ArtSite.Api.Services;
using ArtSite.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowController : ControllerBase
{
  private readonly IShowService _showService;
  private readonly IStorageService _storageService;

  public ShowController(IShowService showService, IStorageService storageService)
  {
    _showService = showService;
    _storageService = storageService;
  }

  [HttpGet]
  public async Task<ActionResult<List<ShowDto>>> GetAll()
  {
    var shows = await _showService.GetAllAsync();
    return Ok(shows);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ShowDetailDto>> GetById(int id)
  {
    var show = await _showService.GetByIdAsync(id);

    if (show == null)
      return NotFound(new { message = $"Show with ID {id} not found" });

    return Ok(show);
  }

  [HttpPost]
  [Authorize]
  public async Task<ActionResult<ShowDetailDto>> Create([FromBody] CreateShowDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var show = await _showService.CreateAsync(dto);

    return CreatedAtAction(nameof(GetById), new { id = show.ShowId }, show);
  }

  [HttpPut("{id}")]
  [Authorize]
  public async Task<ActionResult<ShowDetailDto>> Update(int id, [FromBody] UpdateShowDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var show = await _showService.UpdateAsync(id, dto);

    if (show == null)
      return NotFound(new { message = $"Show with ID {id} not found" });

    return Ok(show);
  }

  [HttpDelete("{id}")]
  [Authorize]
  public async Task<IActionResult> Delete(int id)
  {
    var success = await _showService.DeleteAsync(id);

    if (!success)
      return NotFound(new { message = $"Show with ID {id} not found" });

    return NoContent();
  }

  [HttpPost("{showId}/images")]
  [Authorize]
  public async Task<ActionResult<ShowImageDto>> UploadImage(
    int showId,
    [FromForm] IFormFile image,
    [FromForm] string? altText = null,
    [FromForm] string? imageType = null)
  {
    // Validate show exists
    var show = await _showService.GetByIdAsync(showId);
    if (show == null)
      return NotFound(new { message = $"Show with ID {showId} not found" });

    // Validate file
    if (image == null || image.Length == 0)
      return BadRequest(new { message = "No file uploaded" });

    // Validate file type
    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    if (!allowedTypes.Contains(image.ContentType.ToLower()))
      return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, and WebP are allowed" });

    try
    {
      // Upload to storage
      using var stream = image.OpenReadStream();
      var imageUrl = await _storageService.UploadAsync(
        stream,
        $"show_{showId}_{Guid.NewGuid()}_{image.FileName}",
        image.ContentType);

      // Save to database
      var showImage = new Shared.Models.ShowImage
      {
        ShowId = showId,
        BucketPath = imageUrl,
        AltText = altText ?? show.Title,
        ImageType = imageType
      };

      using var scope = HttpContext.RequestServices.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
      context.ShowImages.Add(showImage);
      await context.SaveChangesAsync();

      return Ok(new ShowImageDto
      {
        ShowImageId = showImage.ShowImageId,
        ImageUrl = showImage.BucketPath,
        AltText = showImage.AltText,
        ImageType = showImage.ImageType
      });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = $"Error uploading image: {ex.Message}" });
    }
  }

  [HttpDelete("{showId}/images/{imageId}")]
  [Authorize]
  public async Task<IActionResult> DeleteImage(int showId, int imageId)
  {
    using var scope = HttpContext.RequestServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();

    var image = await context.ShowImages
      .FirstOrDefaultAsync(img => img.ShowImageId == imageId && img.ShowId == showId);

    if (image == null)
      return NotFound(new { message = $"Image with ID {imageId} not found for show {showId}" });

    // Delete from storage
    try
    {
      var fileName = image.BucketPath.Split('/').Last();
      await _storageService.DeleteAsync(fileName);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Failed to delete image from storage: {ex.Message}");
    }

    // Delete from database
    context.ShowImages.Remove(image);
    await context.SaveChangesAsync();

    return NoContent();
  }
}
