using ArtSite.Shared.DTOs;

namespace ArtSite.Api.Services;

public interface IAuthService
{
  Task<LoginResponse?> LoginAsync(LoginRequest request);
  string GenerateJwtToken(string username);
  bool ValidateCredentials(string username, string password);
}