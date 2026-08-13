using ProjectZero.Components.Pages;
using Bunit;

namespace ProjectZero.Tests.Components.Pages;

public class HomeTests : IDisposable
{
    private readonly BunitContext _context = new();

    [Fact]
    public void Home_RendersHeadingAndWelcomeMessage()
    {
        var cut = _context.Render<Home>();

        Assert.Equal("Hello, world!", cut.Find("h1").TextContent);
        Assert.Contains("Welcome to your new app.", cut.Markup);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}