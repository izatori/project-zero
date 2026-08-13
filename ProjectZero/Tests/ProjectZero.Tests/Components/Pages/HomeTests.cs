using ProjectZero.Components.Pages;
using Bunit;

namespace ProjectZero.Tests.Components.Pages;

public class HomeTests : IDisposable
{
    private readonly BunitContext _context = new();

    [Fact]
    public void Home_RendersHeadingAndLead()
    {
        var cut = _context.Render<Home>();

        Assert.Equal("ProjectZero", cut.Find("h1").TextContent);
        Assert.Contains("learn", cut.Find("p.lead").TextContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Home_LinksToLearningPages()
    {
        var cut = _context.Render<Home>();

        Assert.Contains(cut.FindAll("a[href]"), l => l.GetAttribute("href") == "/razor-syntax");
        Assert.Contains(cut.FindAll("a[href]"), l => l.GetAttribute("href") == "/playground");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}