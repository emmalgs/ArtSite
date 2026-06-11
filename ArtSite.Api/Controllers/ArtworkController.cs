using ArtSite.Shared.DTOs;
using ArtSite.Shared.Models;
using ArtSite.Api.Services;
using Microsoft.AspNetCore.Mvc;
using ArtSite.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ArtSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtworkController : ControllerBase
{
  private readonly IArtworkService _artworkService;
  private readonly IStorageService _storageService;
  private readonly AppDbContext _context;
  public ArtworkController(
    IArtworkService artworkService,
    IStorageService storageService,
    AppDbContext context)
  {
    _artworkService = artworkService;
    _storageService = storageService;
    _context = context;
  }
  [HttpGet]
  public async Task<ActionResult<List<ArtworkDto>>> GetAll()
  {
    var artworks = await _artworkService.GetAllAsync();
    return Ok(artworks);
  }
  [HttpGet("{id}")]
  public async Task<ActionResult<ArtworkDetailDto>> GetById(int id)
  {
    var artwork = await _artworkService.GetByIdAsync(id);
    if (artwork == null)
      return NotFound(new { message = $"Artwork with ID {id} not found" });
    return Ok(artwork);
  }

  [HttpPost]
  public async Task<ActionResult<ArtworkDetailDto>> Create(
    [FromBody] CreateArtworkDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var artwork = await _artworkService.CreateAsync(dto);

    return CreatedAtAction(
      nameof(GetById),
      new { id = artwork.ArtWorkId },
      artwork
    );
  }

  [HttpPost("{artworkId}/images")]
  public async Task<ActionResult<ArtworkImageDto>> UploadImage(
    int artworkId,
    [FromForm] IFormFile image,
    [FromForm] string? altText = null,
    [FromForm] bool isPrimary = false)
  {
    // Validate artwork exists
    var artwork = await _artworkService.GetByIdAsync(artworkId);
    if (artwork == null)
      return NotFound(new { message = $"Artwork with ID {artworkId} not found" });

    // Validate file
    if (image == null || image.Length == 0)
      return BadRequest(new { message = "No file uploaded" });

    // Validate file type
    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    if (!allowedTypes.Contains(image.ContentType.ToLower()))
      return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, and WebP are allowed" });

    // Upload to Supabase and get URL
    using var stream = image.OpenReadStream();
    var imageUrl = await _storageService.UploadAsync(
      stream,
      $"{artworkId}_{Guid.NewGuid()}_{image.FileName}",
      image.ContentType);

    // Save image record to database
    var artworkImage = new ArtworkImage
    {
      ArtWorkId = artworkId,
      BucketPath = imageUrl,
      AltText = altText ?? artwork.Title,
      IsPrimary = isPrimary
    };

    _context.ArtworkImages.Add(artworkImage);
    await _context.SaveChangesAsync();

    return Ok(new ArtworkImageDto
    {
      ArtworkImageId = artworkImage.ArtworkImageId,
      ImageUrl = artworkImage.BucketPath,
      AltText = artworkImage.AltText,
      IsPrimary = artworkImage.IsPrimary
    });
  }


  [HttpDelete("{artworkId}/images/{imageId}")]
  public async Task<IActionResult> DeleteImage(int artworkId, int imageId)
  {
    // Verify artwork exists
    var artwork = await _artworkService.GetByIdAsync(artworkId);
    if (artwork == null)
      return NotFound(new { message = $"Artwork with ID {artworkId} not found" });

    // Find the image
    var image = await _context.ArtworkImages
      .FirstOrDefaultAsync(img => img.ArtworkImageId == imageId && img.ArtWorkId == artworkId);

    if (image == null)
      return NotFound(new { message = $"Image with ID {imageId} not found for artwork {artworkId}" });

    // Delete from Supabase storage
    try
    {
      // Extract filename from BucketPath URL
      var fileName = image.BucketPath.Split('/').Last();
      await _storageService.DeleteAsync(fileName);
    }
    catch (Exception ex)
    {
      // Log the error but continue with database deletion
      Console.WriteLine($"Failed to delete image from storage: {ex.Message}");
    }

    // Delete from database
    _context.ArtworkImages.Remove(image);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  [HttpPut("{artworkId}/images/{imageId}/primary")]
  public async Task<IActionResult> SetPrimaryImage(int artworkId, int imageId)
  {
    // Verify artwork exists
    var artwork = await _artworkService.GetByIdAsync(artworkId);
    if (artwork == null)
      return NotFound(new { message = $"Artwork with ID {artworkId} not found" });

    // Find the image
    var image = await _context.ArtworkImages
      .FirstOrDefaultAsync(img => img.ArtworkImageId == imageId && img.ArtWorkId == artworkId);

    if (image == null)
      return NotFound(new { message = $"Image with ID {imageId} not found for artwork {artworkId}" });

    // Unset all other primary images for this artwork
    var otherImages = await _context.ArtworkImages
      .Where(img => img.ArtWorkId == artworkId && img.ArtworkImageId != imageId)
      .ToListAsync();

    foreach (var otherImage in otherImages)
    {
      otherImage.IsPrimary = false;
    }

    // Set this image as primary
    image.IsPrimary = true;

    await _context.SaveChangesAsync();

    return Ok(new { message = "Primary image updated successfully" });
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ArtworkDetailDto>> Update(
    int id,
    [FromBody] UpdateArtworkDto dto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var artwork = await _artworkService.UpdateAsync(id, dto);

    if (artwork == null)
      return NotFound(new { message = $"Artwork with ID {id} not found" });

    return Ok(artwork);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    // Get artwork with images before deleting
    var artwork = await _artworkService.GetByIdAsync(id);

    if (artwork == null)
      return NotFound(new { message = $"Artwork with ID {id} not found" });

    // Delete all images from Supabase Storage
    if (artwork.Images != null && artwork.Images.Any())
    {
      foreach (var image in artwork.Images)
      {
        try
        {
          // Extract filename from URL
          var fileName = image.ImageUrl.Split('/').Last();
          await _storageService.DeleteAsync(fileName);
        }
        catch (Exception ex)
        {
          // Log error but continue with deletion
          Console.WriteLine($"Failed to delete image {image.ArtworkImageId} from storage: {ex.Message}");
        }
      }
    }

    // Delete artwork (images cascade delete from database)
    var success = await _artworkService.DeleteAsync(id);

    if (!success)
      return NotFound(new { message = $"Artwork with ID {id} not found" });

    return NoContent();
  }
}