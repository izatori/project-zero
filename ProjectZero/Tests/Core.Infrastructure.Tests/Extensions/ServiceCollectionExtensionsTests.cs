using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using Core.Domain.Services;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructureServices_RegistersDbContextAndServices()
    {
        var services = new ServiceCollection();

        services.AddInfrastructureServices();

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<ApplicationDbContext>());
        Assert.NotNull(provider.GetService<IUserRepository>());
        Assert.NotNull(provider.GetService<IUnitOfWork>());
        Assert.NotNull(provider.GetService<IDeleteMeUserDomainService>());
        Assert.NotNull(provider.GetService<IDeleteMeEmailDomainService>());
    }

    [Fact]
    public void AddInfrastructureServices_WithConnectionString_RegistersServices()
    {
        var services = new ServiceCollection();

        services.AddInfrastructureServices("Server=.;Database=ProjectZero;Trusted_Connection=true");

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<ApplicationDbContext>());
        Assert.NotNull(provider.GetService<IUserRepository>());
        Assert.NotNull(provider.GetService<IUnitOfWork>());
    }
}