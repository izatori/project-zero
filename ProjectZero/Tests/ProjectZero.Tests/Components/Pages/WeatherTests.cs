using ProjectZero.Components.Pages;
using Bunit;

namespace ProjectZero.Tests.Components.Pages;

public class WeatherTests : IDisposable
{
    private readonly BunitContext _context = new();

    [Fact]
    public void Weather_InitiallyShowsLoadingState()
    {
        var cut = _context.Render<Weather>();

        Assert.Contains("Loading...", cut.Markup);
    }

    [Fact]
    public void Weather_AfterLoad_ShowsForecastTable()
    {
        var cut = _context.Render<Weather>();

        cut.WaitForAssertion(() => Assert.Equal(5, cut.FindAll("tbody tr").Count));

        Assert.Contains("Temp. (C)", cut.Markup);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}