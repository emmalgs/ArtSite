using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace ArtSite.Client.Services;

public interface IApiService
{
  Task<HttpClient> GetAuthenticatedHttpClientAsync();
}

public class ApiService : IApiService
{
  private readonly HttpClient _httpClient;
  private readonly IJSRuntime _jsRuntime;

  public ApiService(HttpClient httpClient, IJSRuntime jsRuntime)
  {
    _httpClient = httpClient;
    _jsRuntime = jsRuntime;
  }

  public async Task<HttpClient> GetAuthenticatedHttpClientAsync()
  {
    try
    {
      var token = await _jsRuntime.InvokeAsync<string>("authHelper.getToken");

      if (!string.IsNullOrEmpty(token))
      {
        _httpClient.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", token);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error getting token: {ex.Message}");
    }

    return _httpClient;
  }
}
