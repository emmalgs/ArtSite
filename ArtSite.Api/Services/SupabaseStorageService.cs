using Microsoft.Extensions.Options;
using Supabase;
using SkiaSharp;

namespace ArtSite.Api.Services;

public class SupabaseStorageService : IStorageService
{
  private readonly Supabase.Client _supabaseClient;
  private readonly string _bucketName;
  private readonly string _supabaseUrl;
  private const int MaxDimension = 2000;

  public SupabaseStorageService(Supabase.Client supabaseClient, IOptions<Configuration.SupabaseOptions> options)
  {
    _supabaseClient = supabaseClient;
    _bucketName = options.Value.StorageBucket;
    _supabaseUrl = options.Value.Url;
  }

  public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
  {
    // Load image using SkiaSharp
    using var inputStream = new SKManagedStream(fileStream);
    using var original = SKBitmap.Decode(inputStream);

    if (original == null)
      throw new InvalidOperationException("Failed to decode image");

    // Calculate new dimensions maintaining aspect ratio
    var (newWidth, newHeight) = CalculateResizeDimensions(original.Width, original.Height, MaxDimension);

    // Resize if needed
    SKBitmap resized;
    if (newWidth != original.Width || newHeight != original.Height)
    {
      var imageInfo = new SKImageInfo(newWidth, newHeight);
      resized = original.Resize(imageInfo, SKSamplingOptions.Default);
    }
    else
    {
      resized = original;
    }

    // Convert to JPEG format
    var jpegFileName = Path.ChangeExtension(fileName, ".jpg");
    using var outputStream = new MemoryStream();
    using var image = SKImage.FromBitmap(resized);
    using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
    data.SaveTo(outputStream);
    outputStream.Position = 0;

    // Clean up resized bitmap if different from original
    if (resized != original)
      resized.Dispose();

    // Upload to Supabase Storage
    var bytes = outputStream.ToArray();
    await _supabaseClient.Storage
      .From(_bucketName)
      .Upload(bytes, jpegFileName, new Supabase.Storage.FileOptions
      {
        ContentType = "image/jpeg",
        Upsert = true
      });

    // Return the public URL
    return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{jpegFileName}";
  }

  private static (int width, int height) CalculateResizeDimensions(int originalWidth, int originalHeight, int maxDimension)
  {
    if (originalWidth <= maxDimension && originalHeight <= maxDimension)
      return (originalWidth, originalHeight);

    var ratio = (double)originalWidth / originalHeight;

    if (originalWidth > originalHeight)
      return (maxDimension, (int)(maxDimension / ratio));
    else
      return ((int)(maxDimension * ratio), maxDimension);
  }

  public async Task<string> GetImageUrlAsync(string fileName)
  {
    // Get public URL for the file
    var publicUrl = _supabaseClient.Storage
      .From(_bucketName)
      .GetPublicUrl(fileName);

    return publicUrl;
  }

  public async Task DeleteAsync(string fileName)
  {
    await _supabaseClient.Storage
      .From(_bucketName)
      .Remove(new List<string> { fileName });
  }
}