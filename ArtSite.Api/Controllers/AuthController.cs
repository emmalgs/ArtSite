using ArtSite.Api.Services;
using ArtSite.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ArtSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("login")]
  public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);
    
    var response = await _authService.LoginAsync(loginRequest);

    if (response == null)
      return Unauthorized(new { message = "Invalid username or password"});
    
    return Ok(response);
  }
}
