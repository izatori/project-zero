using Core.Application.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_RegistersMediatR()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddApplicationServices();

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IMediator>());
    }
}