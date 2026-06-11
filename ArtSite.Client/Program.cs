using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ArtSite.Client;
using ArtSite.Client.Services;
using ArtSite.Client.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(sp =>
{
  var handler = sp.GetRequiredService<AuthHeaderHandler>();
  handler.InnerHandler = new HttpClientHandler();

  var httpClient = new HttpClient(handler)
  {
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress
  };
});

await builder.Build().RunAsync();
