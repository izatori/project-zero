using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using Core.Domain.Services;
using Core.Infrastructure.Persistence;
using Core.Infrastructure.Persistence.Repositories;
using Core.Infrastructure.Services;
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
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register domain services
        services.AddScoped<IDeleteMeUserDomainService, DeleteMeUserDomainService>();
        services.AddScoped<IDeleteMeEmailDomainService, DeleteMeEmailDomainService>();

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
            // Uncomment when Microsoft.EntityFrameworkCore.SqlServer is added
            // options.UseSqlServer(connectionString);

            // For now, use InMemory
            options.UseInMemoryDatabase("ProjectZero");
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register domain services
        services.AddScoped<IDeleteMeUserDomainService, DeleteMeUserDomainService>();
        services.AddScoped<IDeleteMeEmailDomainService, DeleteMeEmailDomainService>();

        return services;
    }
}