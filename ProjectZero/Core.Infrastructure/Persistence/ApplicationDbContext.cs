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

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Name).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UpdatedAt);
            builder.Property(u => u.IsActive).IsRequired();

            // Unique constraint on email
            builder.HasIndex(u => u.Email).IsUnique();
        });
        
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
    }
}