namespace ArtSite.Api.Services;

public interface IStorageService
{
  Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
  Task<string> GetImageUrlAsync(string fileName);
  Task DeleteAsync(string fileName);
}