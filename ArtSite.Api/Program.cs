using System.Text;
using ArtSite.Api.Configuration;
using ArtSite.Api.Data;
using ArtSite.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Supabase;
using SupabaseOptions = Supabase.SupabaseOptions;


var builder = WebApplication.CreateBuilder(args);

// Configure port from environment (for Railway/Render)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Configure Supabase options
builder.Services.Configure<ArtSite.Api.Configuration.SupabaseOptions>(
    builder.Configuration.GetSection("Supabase"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddOpenApi();

builder.Services.AddControllers();

// Add CORS - Allow any origin for now (you can restrict this later to your deployment URL)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(connectionString)
);

// Register Supabase client as singleton
var supabaseUrl = builder.Configuration["Supabase:Url"]
    ?? throw new InvalidOperationException("Supabase:Url configuration is missing");
var supabaseKey = builder.Configuration["Supabase:Key"]
    ?? throw new InvalidOperationException("Supabase:Key configuration is missing");

builder.Services.AddSingleton(provider =>
{
    var options = new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = false,
    };

    return new Client(supabaseUrl, supabaseKey, options);
});

// Jwt token
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt")
);

// Register services
builder.Services.AddScoped<IStorageService, SupabaseStorageService>();
builder.Services.AddScoped<IArtworkService, ArtworkService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IArtistInfoService, ArtistInfoService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt Key is missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Only use HTTPS redirection in development
// Render handles HTTPS at the load balancer level
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowBlazorClient");

// Configure static file content type provider BEFORE using static files
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".webassembly"] = "application/wasm";
provider.Mappings[".js"] = "application/javascript";
provider.Mappings[".dat"] = "application/octet-stream";
provider.Mappings[".dll"] = "application/octet-stream";
provider.Mappings[".wasm"] = "application/wasm";
provider.Mappings[".json"] = "application/json";
provider.Mappings[".br"] = "application/octet-stream";
provider.Mappings[".gz"] = "application/octet-stream";

// Serve Blazor WebAssembly framework files from _framework directory
app.UseBlazorFrameworkFiles();

// Serve static files (CSS, JS, images, etc.) with proper MIME types
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx =>
    {
        // Handle files without extensions
        if (string.IsNullOrEmpty(Path.GetExtension(ctx.File.Name)))
        {
            ctx.Context.Response.Headers.ContentType = "application/javascript";
        }

        // Add caching headers for framework files
        if (ctx.Context.Request.Path.StartsWithSegments("/_framework"))
        {
            ctx.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
        }
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map API controllers - these will be matched before the fallback
app.MapControllers();

// Fallback to index.html for client-side routing (SPA)
// This MUST be last so API routes take precedence
app.MapFallbackToFile("index.html");

app.Run();
