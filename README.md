# ArtSite

Full-stack artist portfolio management system with admin panel and API.

## Tech Stack

**Backend:**
- ASP.NET Core 10.0 Web API
- Entity Framework Core with PostgreSQL
- JWT Authentication
- Supabase for database and file storage

**Frontend:**
- Blazor WebAssembly
- Bootstrap styling

**Hosting:**
- Render.com (Free tier)
- Deployed URL: https://artsite-5uh2.onrender.com

## Project Structure

```
ArtSite/
├── ArtSite.Api/          # ASP.NET Core API
├── ArtSite.Client/       # Blazor WebAssembly admin UI
├── ArtSite.Shared/       # Shared models and DTOs
├── Dockerfile            # Docker configuration for deployment
└── API_DOCUMENTATION.md  # Complete API reference
```

## Features

- **Artwork Management** - CRUD operations with image uploads
- **Location Management** - Track galleries, museums, collections
- **Show/Exhibition Management** - Link artworks to shows with artifacts
- **Artist Info Management** - CV, Bio, Artist Statement with version history
- **Authentication** - JWT-based secure admin access
- **Image Storage** - Supabase storage integration with automatic JPEG conversion

---

## Local Development Setup

### Prerequisites

- .NET 10.0 SDK
- PostgreSQL database (via Supabase)
- Supabase account

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/ArtSite.git
cd ArtSite
```

### 2. Configure Supabase

1. Go to [supabase.com](https://supabase.com) and create a new project
2. Get your connection string from Settings → Database
3. Get your API keys from Settings → API
4. Create a storage bucket named `artsite-images` (or your preferred name)

### 3. Configure Local Settings

Create `ArtSite.Api/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-supabase-host;Database=postgres;Username=postgres;Password=your-password"
  },
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "your-supabase-key"
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
    "Issuer": "ArtSiteApi",
    "Audience": "ArtSiteClient"
  }
}
```

**Important:** Never commit `appsettings.json` to git. It's already in `.gitignore`.

### 4. Set Up User Secrets (Alternative to appsettings.json)

```bash
cd ArtSite.Api
dotnet user-secrets set "Supabase:Key" "your-service-role-key"
dotnet user-secrets set "Jwt:Key" "your-jwt-secret-key"
```

### 5. Run Migrations

```bash
cd ArtSite.Api
dotnet ef database update
```

### 6. Run Locally

```bash
# From the solution root
dotnet run --project ArtSite.Api
```

Visit:
- Admin UI: http://localhost:5023
- API: http://localhost:5023/api/artwork

---

## Database Changes & Migrations

### Making Schema Changes

1. **Modify your models** in `ArtSite.Shared/Models/`

2. **Create a migration:**
   ```bash
   cd ArtSite.Api
   dotnet ef migrations add YourMigrationName
   ```

3. **Review the migration** in `ArtSite.Api/Migrations/`

4. **Apply to local database:**
   ```bash
   dotnet ef database update
   ```

5. **Test locally** to ensure everything works

6. **Commit and push:**
   ```bash
   git add .
   git commit -m "Add YourMigrationName migration"
   git push origin main
   ```

7. **Deploy to Render** (see deployment section below)

### Common Migration Scenarios

**Add a new column:**
```csharp
// In your model
public string NewProperty { get; set; } = string.Empty;
```

**Add a new table:**
```csharp
// Create new model class
// Add DbSet to AppDbContext
public DbSet<NewEntity> NewEntities { get; set; }
```

**Modify column constraints:**
```csharp
// In AppDbContext.OnModelCreating
modelBuilder.Entity<YourEntity>(entity =>
{
    entity.Property(e => e.PropertyName)
        .HasMaxLength(100)
        .IsRequired();
});
```

---

## Deployment to Render

### Initial Setup

1. **Create Render Account**
   - Go to [render.com](https://render.com)
   - Sign up with GitHub

2. **Create New Web Service**
   - Click "New +" → "Web Service"
   - Connect your GitHub repository
   - Render auto-detects the Dockerfile

3. **Configure Service**
   - **Name:** `artsite-api`
   - **Region:** Choose closest to you
   - **Branch:** `main`
   - **Instance Type:** Free

4. **Add Environment Variables**

   Click "Environment" → "Add Environment Variable":

   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=<supabase-connection-string>
   Supabase__Url=<supabase-url>
   Supabase__Key=<supabase-service-role-key>
   Jwt__Key=<your-jwt-secret>
   Jwt__Issuer=ArtSiteApi
   Jwt__Audience=ArtSiteClient
   ```

   **Note:** Use double underscores `__` for nested config (e.g., `ConnectionStrings__DefaultConnection`)

5. **Deploy**
   - Click "Create Web Service"
   - Render will build and deploy
   - You'll get a URL like: `https://artsite-xxxx.onrender.com`

### Updating the Deployment

**Any time you push to `main` branch, Render automatically redeploys.**

```bash
# Make your changes
git add .
git commit -m "Your change description"
git push origin main
```

Render will:
1. Pull latest code
2. Build Docker image
3. Run migrations automatically (if configured)
4. Deploy new version
5. Takes ~2-3 minutes

### Manual Deployment

If auto-deploy is disabled:
1. Go to Render Dashboard
2. Select your service
3. Click "Manual Deploy" → "Deploy latest commit"

### Running Migrations on Render

**Option 1: Automatic (Recommended)**

Migrations run automatically on startup via Entity Framework's `Database.Migrate()` if configured.

**Option 2: Manual via Render Shell**

1. Go to Render Dashboard → Your Service
2. Click "Shell" tab
3. Run:
   ```bash
   dotnet ArtSite.Api.dll
   ```

**Option 3: Run Locally Against Production DB** (Not Recommended)

```bash
# Temporarily update connection string to production
dotnet ef database update
# Change back immediately
```

### Viewing Logs

1. Render Dashboard → Your Service
2. Click "Logs" tab
3. View real-time deployment and runtime logs

---

## Supabase Configuration

### Required Setup

1. **Database (PostgreSQL)**
   - Automatically created with Supabase project
   - Get connection string from Settings → Database → Connection String → Connection Pooling

2. **Storage Bucket**
   - Go to Storage → Create bucket
   - Name: `artsite-images` (or update code if different)
   - Set to Public or configure RLS policies
   - Get service_role key from Settings → API

3. **Row Level Security (RLS)**

   Option A - Disable RLS (easier, less secure):
   ```sql
   ALTER TABLE "ArtWorks" DISABLE ROW LEVEL SECURITY;
   ALTER TABLE "Locations" DISABLE ROW LEVEL SECURITY;
   -- Repeat for all tables
   ```

   Option B - Use service_role key (recommended):
   - Use the service_role key in your environment variables
   - This bypasses RLS policies

### Updating Supabase Configuration

**Change storage bucket:**
1. Update bucket name in `SupabaseStorageService.cs`
2. Commit and push

**Change database:**
1. Update connection string in Render environment variables
2. Manually redeploy or wait for next code push

---

## Troubleshooting

### Deployment Issues

**Build fails:**
- Check Render logs for specific error
- Ensure all dependencies are in `.csproj` files
- Verify Dockerfile is correct

**Migration fails:**
- Check connection string is correct
- Ensure Supabase database is accessible
- Check migration files are included in git

**App starts but returns 404:**
- Verify routes in controllers
- Check middleware order in `Program.cs`
- Look for HTTPS redirection issues

### Database Issues

**Connection timeout:**
- Verify Supabase project is active
- Check connection string format
- Ensure IP whitelist allows Render IPs (or allow all)

**Migration conflicts:**
- Delete local migrations: `rm -rf Migrations/`
- Recreate: `dotnet ef migrations add InitialCreate`
- Or manually fix conflicting migration files

### Authentication Issues

**JWT token invalid:**
- Verify `Jwt:Key` matches between environments
- Check token hasn't expired
- Ensure Authorization header format: `Bearer <token>`

---

## API Documentation

See [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) for complete API reference.

Quick endpoints:
- `GET /api/artwork` - Get all artworks
- `GET /api/location` - Get all locations
- `GET /api/show` - Get all shows
- `GET /api/artistinfo` - Get artist info
- `POST /api/auth/login` - Login (get JWT token)

---

## Project Maintenance Checklist

### Monthly
- [ ] Check Render free tier usage (750 hours/month)
- [ ] Review error logs
- [ ] Backup database (Supabase auto-backups)

### Before Making Changes
- [ ] Pull latest: `git pull origin main`
- [ ] Create feature branch: `git checkout -b feature-name`
- [ ] Test locally
- [ ] Commit and push
- [ ] Verify deployment on Render

### After Schema Changes
- [ ] Create migration
- [ ] Test locally
- [ ] Update API documentation if endpoints changed
- [ ] Commit migration files
- [ ] Push and verify Render deployment

---

## Common Tasks

### Add a New Entity

1. Create model in `ArtSite.Shared/Models/`
2. Add DbSet to `AppDbContext.cs`
3. Configure entity in `OnModelCreating`
4. Create DTOs in `ArtSite.Shared/DTOs/`
5. Create service interface and implementation
6. Create controller
7. Register service in `Program.cs`
8. Create migration and update database
9. Test and deploy

### Add Authentication to Endpoint

```csharp
[HttpPost]
[Authorize]  // Add this attribute
public async Task<ActionResult> YourEndpoint()
{
    // ...
}
```

### Update Supabase Storage Bucket

1. Update bucket name in `SupabaseStorageService.cs`
2. Update bucket permissions in Supabase dashboard
3. Test image upload
4. Commit and deploy

---

## License

MIT

## Support

For issues or questions:
1. Check logs in Render dashboard
2. Review API documentation
3. Check Supabase dashboard for database/storage issues
