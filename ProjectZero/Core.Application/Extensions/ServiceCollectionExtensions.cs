using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Extensions;

/// <summary>
/// Extension methods for setting up application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application layer services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR with handlers from this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        // Register the application mediator
        services.AddScoped<Abstractions.IMediator, Mediator>();

        return services;
    }
}