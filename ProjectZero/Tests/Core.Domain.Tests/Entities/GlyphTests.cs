using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Domain.Tests.Entities;

public class GlyphTests
{
    private static Glyph CreateGlyph() => Glyph.Create("あ", "a", GlyphType.Hiragana, "a.jpg", "a.gif");

    [Fact]
    public void Create_InitializesLearnedAndFavouriteToFalse()
    {
        var glyph = CreateGlyph();

        Assert.False(glyph.IsLearned);
        Assert.False(glyph.IsFavourite);
    }

    [Fact]
    public void MarkAsLearned_SetsLearnedAndRaisesEvent()
    {
        var glyph = CreateGlyph();
        glyph.ClearDomainEvents();

        glyph.MarkAsLearned();

        Assert.True(glyph.IsLearned);

        var domainEvent = Assert.Single(glyph.GetDomainEvents());
        var learned = Assert.IsType<GlyphMarkedLearnedEvent>(domainEvent);
        Assert.Equal(glyph.Id, learned.GlyphId);
    }

    [Fact]
    public void MarkAsLearned_WhenAlreadyLearned_DoesNotRaiseEvent()
    {
        var glyph = CreateGlyph();
        glyph.MarkAsLearned();
        glyph.ClearDomainEvents();

        glyph.MarkAsLearned();

        Assert.Empty(glyph.GetDomainEvents());
    }

    [Fact]
    public void MarkAsNotLearned_SetsUnlearnedAndRaisesEvent()
    {
        var glyph = CreateGlyph();
        glyph.MarkAsLearned();
        glyph.ClearDomainEvents();

        glyph.MarkAsNotLearned();

        Assert.False(glyph.IsLearned);

        var domainEvent = Assert.Single(glyph.GetDomainEvents());
        var unlearned = Assert.IsType<GlyphMarkedAsUnlearnedEvent>(domainEvent);
        Assert.Equal(glyph.Id, unlearned.GlyphId);
    }

    [Fact]
    public void MarkAsFavourite_SetsFavouriteAndRaisesEvent()
    {
        var glyph = CreateGlyph();
        glyph.ClearDomainEvents();

        glyph.MarkAsFavourite();

        Assert.True(glyph.IsFavourite);

        var domainEvent = Assert.Single(glyph.GetDomainEvents());
        var favourite = Assert.IsType<GlyphMarkedFavouriteEvent>(domainEvent);
        Assert.Equal(glyph.Id, favourite.GlyphId);
    }

    [Fact]
    public void MarkAsFavourite_WhenAlreadyFavourite_DoesNotRaiseEvent()
    {
        var glyph = CreateGlyph();
        glyph.MarkAsFavourite();
        glyph.ClearDomainEvents();

        glyph.MarkAsFavourite();

        Assert.Empty(glyph.GetDomainEvents());
    }

    [Fact]
    public void MarkAsNotFavourite_SetsUnfavouriteAndRaisesEvent()
    {
        var glyph = CreateGlyph();
        glyph.MarkAsFavourite();
        glyph.ClearDomainEvents();

        glyph.MarkAsNotFavourite();

        Assert.False(glyph.IsFavourite);

        var domainEvent = Assert.Single(glyph.GetDomainEvents());
        var unfavourite = Assert.IsType<GlyphMarkedAsUnfavouriteEvent>(domainEvent);
        Assert.Equal(glyph.Id, unfavourite.GlyphId);
    }
}