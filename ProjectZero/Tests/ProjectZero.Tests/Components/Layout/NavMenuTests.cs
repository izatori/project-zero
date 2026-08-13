using ProjectZero.Components.Layout;
using Bunit;

namespace ProjectZero.Tests.Components.Layout;

public class NavMenuTests : IDisposable
{
    private readonly BunitContext _context = new();

    [Fact]
    public void NavMenu_ContainsNavigationLinksForMainPages()
    {
        var cut = _context.Render<NavMenu>();

        var links = cut.FindAll("nav a");

        Assert.Equal(3, links.Count);
        Assert.Contains(links, l => l.GetAttribute("href") == "");
        Assert.Contains(links, l => l.GetAttribute("href") == "counter");
        Assert.Contains(links, l => l.GetAttribute("href") == "weather");
    }

    [Fact]
    public void NavMenu_DisplaysHomeLink()
    {
        var cut = _context.Render<NavMenu>();

        Assert.Contains("Home", cut.FindAll("nav a")[0].TextContent);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}