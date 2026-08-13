using ProjectZero.Components.Pages;
using Bunit;

namespace ProjectZero.Tests.Components.Pages;

public class CounterTests : IDisposable
{
    private readonly BunitContext _context = new();

    [Fact]
    public void Counter_InitiallyRendersZero()
    {
        var cut = _context.Render<Counter>();

        Assert.Equal("Current count: 0", cut.Find("p[role=status]").TextContent);
    }

    [Fact]
    public void Counter_ClickingButton_IncrementsCount()
    {
        var cut = _context.Render<Counter>();

        cut.Find("button").Click();

        Assert.Equal("Current count: 1", cut.Find("p[role=status]").TextContent);
    }

    [Fact]
    public void Counter_ClickingButtonMultipleTimes_IncrementsEachTime()
    {
        var cut = _context.Render<Counter>();

        var button = cut.Find("button");
        button.Click();
        button.Click();
        button.Click();

        Assert.Equal("Current count: 3", cut.Find("p[role=status]").TextContent);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}