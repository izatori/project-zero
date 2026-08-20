using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;

namespace Core.Infrastructure.Persistence;

/// <summary>
/// Application DbContext for Entity Framework Core.
/// Manages database connections and entity mappings.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Glyph> Glyphs { get; set; } = null!;
    public DbSet<Translation> Translations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
            builder.Property(p => p.FileName).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Price).IsRequired();
            builder.Property(p => p.Description).IsRequired().HasMaxLength(1023);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt);
            builder.Property(p => p.IsActive).IsRequired();
        });

        // Glyph configuration
        modelBuilder.Entity<Glyph>(builder =>
        {
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Character).IsRequired().HasMaxLength(4);
            builder.Property(g => g.Romaji).IsRequired().HasMaxLength(255);
            builder.Property(g => g.Type).IsRequired();
            builder.Property(g => g.ImageFileName).IsRequired().HasMaxLength(255);
            builder.Property(g => g.StrokeAnimationFileName).HasMaxLength(255);
            builder.Property(g => g.IsLearned).IsRequired();
            builder.Property(g => g.IsFavourite).IsRequired();
        });

        // Translation configuration
        modelBuilder.Entity<Translation>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.GlyphId);
            builder.Property(t => t.JapaneseWriting).IsRequired().HasMaxLength(255);
            builder.Property(t => t.RomajiWriting).IsRequired().HasMaxLength(255);
            builder.Property(t => t.English).IsRequired().HasMaxLength(1023).HasColumnName("Translation");
            builder.Property(t => t.ImageFileName).HasMaxLength(255);
            builder.Property(t => t.IsLearned).IsRequired();
            builder.Property(t => t.IsFavourite).IsRequired();

            builder.HasOne<Glyph>()
                .WithMany()
                .HasForeignKey(t => t.GlyphId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}