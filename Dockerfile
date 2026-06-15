FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Install wasm-tools workload for Blazor WebAssembly
RUN dotnet workload install wasm-tools

# Copy csproj files and restore dependencies
COPY ["ArtSite.Api/ArtSite.Api.csproj", "ArtSite.Api/"]
COPY ["ArtSite.Client/ArtSite.Client.csproj", "ArtSite.Client/"]
COPY ["ArtSite.Shared/ArtSite.Shared.csproj", "ArtSite.Shared/"]
RUN dotnet restore "ArtSite.Api/ArtSite.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ArtSite.Api"
RUN dotnet build "ArtSite.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArtSite.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Railway uses PORT environment variable
ENV ASPNETCORE_URLS=http://+:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ArtSite.Api.dll"]
