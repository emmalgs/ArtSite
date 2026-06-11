using System.Net.Http.Json;
using ArtSite.Shared.DTOs;
using Microsoft.JSInterop;

namespace ArtSite.Client.Services;

public interface IAuthService
{
  Task<bool> LoginAsync(string username, string password);
  Task LogoutAsync();
  Task<bool> IsAuthenticatedAsync();
  Task<string?> GetTokenAsync();
}

public class AuthService : IAuthService
{
  private readonly HttpClient _httpClient;
  private readonly IJSRuntime _jSRuntime;

  public AuthService(HttpClient httpClient, IJSRuntime jSRuntime)
  {
    _httpClient = httpClient;
    _jSRuntime = jSRuntime;
  }

  public async Task<bool> LoginAsync(string username, string password)
  {
    try
    {
      var request = new LoginRequest
      {
        Username = username,
        Password = password
      };

      var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

      if (response.IsSuccessStatusCode)
      {
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (loginResponse != null)
        {
          await _jSRuntime.InvokeVoidAsync("authHelper.setToken",
            loginResponse.Token,
            loginResponse.ExpiresAt.ToString("o"));
          return true;
        }
      }
      return false;
    }

    catch (Exception ex)
    {
      Console.WriteLine($"Login error: {ex.Message}");
      return false;
    }
  }

  public async Task LogoutAsync()
  {
    await _jSRuntime.InvokeVoidAsync("authHelper.removeToken");
  }

  public async Task<bool> IsAuthenticatedAsync()
  {
    try
    {
      var isExpired = await _jSRuntime.InvokeAsync<bool>("authHelper.isTokenExpired");
      return !isExpired;
    }
    catch
    {
      return false;
    }
  }

  public async Task<string?> GetToken()
  {
    try
    {
      return await _jSRuntime.InvokeAsync<string>("authHelper.getToken");

    }
    catch
    {
      return null;
    }
  }
}