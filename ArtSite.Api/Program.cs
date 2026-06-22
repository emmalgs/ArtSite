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

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowBlazorClient");

app.MapStaticAssets(); 

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<ArtSite.Client.App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode();

// Ensure the fallback route is at the very end
app.MapFallbackToFile("index.html"); 

// Configure static file options to handle extensionless files
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".webassembly"] = "application/wasm";
provider.Mappings[".js"] = "application/javascript";
provider.Mappings[".dat"] = "application/octet-stream";
provider.Mappings[".dll"] = "application/octet-stream";
provider.Mappings[".wasm"] = "application/wasm";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx =>
    {
        // Handle files without extensions (like blazor.webassembly)
        if (string.IsNullOrEmpty(Path.GetExtension(ctx.File.Name)))
        {
            ctx.Context.Response.Headers.ContentType = "application/javascript";
        }
    }
});

app.MapControllers();

// Fallback to index.html for client-side routing (but not for API routes)
app.MapFallbackToFile("index.html");

app.Run();
