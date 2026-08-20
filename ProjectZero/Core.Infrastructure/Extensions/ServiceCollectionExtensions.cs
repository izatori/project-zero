using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using Core.Infrastructure.Persistence;
using Core.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// Extension methods for setting up infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure layer services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register DbContext with in-memory database
        services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("ProjectZero"); });

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IGlyphRepository, GlyphRepository>();
        services.AddScoped<ITranslationRepository, TranslationRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Adds infrastructure layer services with SQL Server configuration.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string connectionString)
    {
        // Register DbContext with SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IGlyphRepository, GlyphRepository>();
        services.AddScoped<ITranslationRepository, TranslationRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}