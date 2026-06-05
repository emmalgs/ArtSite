using Microsoft.Extensions.Options;
using Supabase;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ArtSite.Api.Services;

public class SupabaseStorageService : IStorageService
{
  private readonly Client _supabaseClient;
  private readonly string _bucketName;
  private readonly string _supabaseUrl;
  private const int MaxDimension = 2000;

  public SupabaseStorageService(Client supabaseClient, IOptions<Configuration.SupabaseOptions> options)
  {
    _supabaseClient = supabaseClient;
    _bucketName = options.Value.StorageBucket;
    _supabaseUrl = options.Value.Url;
  }

  public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
  {
    // Load and resize image using SixLabors.ImageSharp
    using var image = await Image.LoadAsync(fileStream);

    if (image.Width > MaxDimension || image.Height > MaxDimension)
    {
      image.Mutate(x => x.Resize(new ResizeOptions
      {
        Size = new Size(MaxDimension, MaxDimension),
        Mode = ResizeMode.Max
      }));
    }

    // Convert to WebP format
    var webpFileName = Path.ChangeExtension(fileName, ".webp");
    using var outputStream = new MemoryStream();
    await image.SaveAsync(outputStream, new WebpEncoder { Quality = 85 });
    outputStream.Position = 0;

    // Upload to Supabase Storage
    var bytes = outputStream.ToArray();
    await _supabaseClient.Storage
      .From(_bucketName)
      .Upload(bytes, webpFileName, new Supabase.Storage.FileOptions
      {
        ContentType = "image/webp",
        Upsert = true
      });

    // Return the public URL
    return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{webpFileName}";
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