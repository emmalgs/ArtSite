using ArtSite.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtSite.Api.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<ArtWork> ArtWorks { get; set; }
    public DbSet<ArtworkImage> ArtworkImages { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<ShowArtwork> ShowArtworks { get; set; }
    public DbSet<ShowImage> ShowImages { get; set; }
    public DbSet<ArtistInfo> ArtistInfos { get; set; }
    public DbSet<ArtistInfoVersion> ArtistInfoVersions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Configure ArtWork entity
      modelBuilder.Entity<ArtWork>(entity =>
      {
        entity.HasKey(a => a.ArtWorkId);

        entity.Property(a => a.Title)
          .IsRequired()
          .HasMaxLength(200);

        entity.Property(a => a.Medium)
          .HasMaxLength(100);

        entity.Property(a => a.Category)
          .HasMaxLength(100);

        entity.Property(a => a.Dimensions)
          .HasMaxLength(100);

        entity.Property(a => a.Price)
          .HasPrecision(18, 2);

        // Configure relationship with Location
        entity.HasOne<Location>()
          .WithMany()
          .HasForeignKey(a => a.LocationId)
          .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship with ArtworkImages
        entity.HasMany(a => a.ArtworkImages)
          .WithOne(ai => ai.ArtWork)
          .HasForeignKey(ai => ai.ArtWorkId)
          .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure ArtworkImage entity
      modelBuilder.Entity<ArtworkImage>(entity =>
      {
        entity.HasKey(ai => ai.ArtworkImageId);

        entity.Property(ai => ai.BucketPath)
          .IsRequired()
          .HasMaxLength(500);

        entity.Property(ai => ai.AltText)
          .HasMaxLength(200);
      });

      // Configure Location entity
      modelBuilder.Entity<Location>(entity =>
      {
        entity.HasKey(l => l.LocationId);

        entity.Property(l => l.Name)
          .HasMaxLength(200);

        entity.Property(l => l.Url)
          .HasMaxLength(500);

        entity.Property(l => l.Email)
          .HasMaxLength(100);

        entity.Property(l => l.Phone)
          .HasMaxLength(20);

        entity.Property(l => l.City)
          .HasMaxLength(100);

        entity.Property(l => l.State)
          .HasMaxLength(100);

        entity.Property(l => l.Country)
          .HasMaxLength(100);

        entity.Property(l => l.CollectionType)
          .HasMaxLength(100);
      });

      // Configure Show entity
      modelBuilder.Entity<Show>(entity =>
      {
        entity.HasKey(s => s.ShowId);

        entity.Property(s => s.Title)
          .IsRequired()
          .HasMaxLength(300);

        entity.Property(s => s.Dates)
          .HasMaxLength(200);

        entity.Property(s => s.ShowType)
          .HasMaxLength(100);

        // Configure relationship with Location
        entity.HasOne(s => s.Location)
          .WithMany()
          .HasForeignKey(s => s.LocationId)
          .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship with ShowArtworks
        entity.HasMany(s => s.ShowArtworks)
          .WithOne(sa => sa.Show)
          .HasForeignKey(sa => sa.ShowId)
          .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with ShowImages
        entity.HasMany(s => s.ShowImages)
          .WithOne(si => si.Show)
          .HasForeignKey(si => si.ShowId)
          .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure ShowArtwork entity (junction table)
      modelBuilder.Entity<ShowArtwork>(entity =>
      {
        entity.HasKey(sa => sa.ShowArtworkId);

        entity.HasOne(sa => sa.Show)
          .WithMany(s => s.ShowArtworks)
          .HasForeignKey(sa => sa.ShowId)
          .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(sa => sa.ArtWork)
          .WithMany()
          .HasForeignKey(sa => sa.ArtWorkId)
          .OnDelete(DeleteBehavior.Cascade);

        // Ensure no duplicate artwork in the same show
        entity.HasIndex(sa => new { sa.ShowId, sa.ArtWorkId })
          .IsUnique();
      });

      // Configure ShowImage entity
      modelBuilder.Entity<ShowImage>(entity =>
      {
        entity.HasKey(si => si.ShowImageId);

        entity.Property(si => si.BucketPath)
          .IsRequired()
          .HasMaxLength(500);

        entity.Property(si => si.AltText)
          .HasMaxLength(200);

        entity.Property(si => si.ImageType)
          .HasMaxLength(100);
      });

      // Configure ArtistInfo entity
      modelBuilder.Entity<ArtistInfo>(entity =>
      {
        entity.HasKey(ai => ai.ArtistInfoId);

        entity.Property(ai => ai.CreatedAt)
          .IsRequired();

        entity.Property(ai => ai.UpdatedAt)
          .IsRequired();

        // Configure relationship with ArtistInfoVersions
        entity.HasMany(ai => ai.Versions)
          .WithOne(aiv => aiv.ArtistInfo)
          .HasForeignKey(aiv => aiv.ArtistInfoId)
          .OnDelete(DeleteBehavior.Cascade);
      });

      // Configure ArtistInfoVersion entity
      modelBuilder.Entity<ArtistInfoVersion>(entity =>
      {
        entity.HasKey(aiv => aiv.ArtistInfoVersionId);

        entity.Property(aiv => aiv.VersionCreatedAt)
          .IsRequired();

        entity.Property(aiv => aiv.ChangeDescription)
          .HasMaxLength(500);
      });
    }
  }
}