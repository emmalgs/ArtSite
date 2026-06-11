using System.Net.Http.Headers;
using ArtSite.Client.Services;

namespace ArtSite.Client.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
  private readonly IAuthService _authService;

  public AuthHeaderHandler(IAuthService authService)
  {
    _authService = authService;
  }

  protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage message,
    CancellationToken cancellationToken)
  {
    var token = await _authService.GetTokenAsync();

    if (!string.IsNullOrEmpty(token))
    {
      message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    return await base.SendAsync(message, cancellationToken);
  }
}