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
    }
  }
}