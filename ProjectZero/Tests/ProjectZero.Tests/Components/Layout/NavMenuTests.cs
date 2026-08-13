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

        var links = cut.FindAll("nav a").Select(l => l.GetAttribute("href"));

        Assert.Contains("", links);
        Assert.Contains("razor-syntax", links);
        Assert.Contains("razor-components", links);
        Assert.Contains("bootstrap-comparison", links);
        Assert.Contains("bootstrap-components", links);
        Assert.Contains("playground", links);
        Assert.Contains("counter", links);
        Assert.Contains("weather", links);
    }

    [Fact]
    public void NavMenu_PlaygroundHasExpandableSubLinks()
    {
        var cut = _context.Render<NavMenu>();

        var toggler = cut.Find("button.nav-sublink-toggle");
        var target = toggler.GetAttribute("data-bs-target");
        Assert.Equal("playgroundSubs", target?.TrimStart('#'));
        Assert.NotNull(cut.Find("#playgroundSubs"));
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