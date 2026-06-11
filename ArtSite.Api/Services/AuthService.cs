using ArtSite.Api.Configuration;
using ArtSite.Shared.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ArtSite.Api.Services;

public class AuthService : IAuthService
{
  private readonly JwtOptions _jwtOptions;
  private readonly IConfiguration _configuration;
  
  public AuthService(IOptions<JwtOptions> options, IConfiguration configuration)
  {
    _jwtOptions = options.Value;
    _configuration = configuration;
  }

  public async Task<LoginResponse?> LoginAsync(LoginRequest request)
  {
    if (!ValidateCredentials(request.Username, request.Password))
      return null;

    var token = GenerateJwtToken(request.Username);
    var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryInMinutes);

    return new LoginResponse
    {
      Token = token,
      ExpiresAt = expiresAt
    };
  }

  public bool ValidateCredentials(string username, string password)
  {
    var adminUsername = _configuration["Admin:Username"];
    var adminPassword = _configuration["Admin:PasswordHash"];
    Console.WriteLine($"Username {username} and adminUsername {adminUsername}");

    if (username != adminUsername)
      return false;
    
    var hashedPassword = HashPassword(password);
    
    Console.WriteLine($"Password: {password}, HashedPassword: {hashedPassword}, Stored Password: {adminPassword}");
    return hashedPassword == adminPassword;
  }

  public string GenerateJwtToken(string username)
  {
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new []
    {
      new Claim(ClaimTypes.Name, username),
      new Claim(ClaimTypes.Role, "Admin"),
      new Claim(JwtRegisteredClaimNames.Sub, username),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
      issuer: _jwtOptions.Issuer,
      audience: _jwtOptions.Audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryInMinutes),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public string HashPassword(string password)
  {
   using var sha256 = System.Security.Cryptography.SHA256.Create();
   var bytes = Encoding.UTF8.GetBytes(password);
   var hash = sha256.ComputeHash(bytes);
   return Convert.ToBase64String(hash); 
  }
}