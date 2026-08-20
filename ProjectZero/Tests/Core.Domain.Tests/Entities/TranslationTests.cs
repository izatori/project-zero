using Core.Domain.Entities;

namespace Core.Domain.Tests.Entities;

public class TranslationTests
{
    private static Translation CreateTranslation() => Translation.Create(Guid.NewGuid(), "あおい", "aoi", "blue");

    [Fact]
    public void Create_InitializesLearnedAndFavouriteToFalse()
    {
        var translation = CreateTranslation();

        Assert.False(translation.IsLearned);
        Assert.False(translation.IsFavourite);
        Assert.NotNull(translation.GlyphId);
    }

    [Fact]
    public void Create_WithoutGlyph_AllowsNullGlyphId()
    {
        var translation = Translation.Create(null, "あおい", "aoi", "blue");

        Assert.Null(translation.GlyphId);
    }

    [Fact]
    public void Create_RaisesCreatedEvent()
    {
        var translation = CreateTranslation();

        var domainEvent = Assert.Single(translation.GetDomainEvents());
        var created = Assert.IsType<TranslationCreatedEvent>(domainEvent);
        Assert.Equal(translation.Id, created.TranslationId);
        Assert.Equal(translation.GlyphId, created.GlyphId);
        Assert.Equal("blue", created.Translation);
    }

    [Fact]
    public void Create_WithEmptyJapaneseWriting_Throws()
    {
        Assert.Throws<ArgumentException>(() => Translation.Create(Guid.NewGuid(), " ", "aoi", "blue"));
    }

    [Fact]
    public void SetImageFileName_SetsAndRaisesEvent()
    {
        var translation = CreateTranslation();
        translation.ClearDomainEvents();

        translation.SetImageFileName("aoi.png");

        Assert.Equal("aoi.png", translation.ImageFileName);

        var domainEvent = Assert.Single(translation.GetDomainEvents());
        var updated = Assert.IsType<TranslationImageUpdatedEvent>(domainEvent);
        Assert.Equal(translation.Id, updated.TranslationId);
        Assert.Equal("aoi.png", updated.ImageFileName);
    }

    [Fact]
    public void SetImageFileName_WhenUnchanged_DoesNotRaiseEvent()
    {
        var translation = CreateTranslation();
        translation.SetImageFileName("aoi.png");
        translation.ClearDomainEvents();

        translation.SetImageFileName("aoi.png");

        Assert.Empty(translation.GetDomainEvents());
    }

    [Fact]
    public void MarkAsLearned_SetsLearnedAndRaisesEvent()
    {
        var translation = CreateTranslation();
        translation.ClearDomainEvents();

        translation.MarkAsLearned();

        Assert.True(translation.IsLearned);

        var domainEvent = Assert.Single(translation.GetDomainEvents());
        var learned = Assert.IsType<TranslationMarkedLearnedEvent>(domainEvent);
        Assert.Equal(translation.Id, learned.TranslationId);
    }

    [Fact]
    public void MarkAsLearned_WhenAlreadyLearned_DoesNotRaiseEvent()
    {
        var translation = CreateTranslation();
        translation.MarkAsLearned();
        translation.ClearDomainEvents();

        translation.MarkAsLearned();

        Assert.Empty(translation.GetDomainEvents());
    }

    [Fact]
    public void MarkAsNotLearned_SetsUnlearnedAndRaisesEvent()
    {
        var translation = CreateTranslation();
        translation.MarkAsLearned();
        translation.ClearDomainEvents();

        translation.MarkAsNotLearned();

        Assert.False(translation.IsLearned);

        var domainEvent = Assert.Single(translation.GetDomainEvents());
        var unlearned = Assert.IsType<TranslationMarkedAsUnlearnedEvent>(domainEvent);
        Assert.Equal(translation.Id, unlearned.TranslationId);
    }

    [Fact]
    public void MarkAsFavourite_SetsFavouriteAndRaisesEvent()
    {
        var translation = CreateTranslation();
        translation.ClearDomainEvents();

        translation.MarkAsFavourite();

        Assert.True(translation.IsFavourite);

        var domainEvent = Assert.Single(translation.GetDomainEvents());
        var favourite = Assert.IsType<TranslationMarkedFavouriteEvent>(domainEvent);
        Assert.Equal(translation.Id, favourite.TranslationId);
    }

    [Fact]
    public void MarkAsFavourite_WhenAlreadyFavourite_DoesNotRaiseEvent()
    {
        var translation = CreateTranslation();
        translation.MarkAsFavourite();
        translation.ClearDomainEvents();

        translation.MarkAsFavourite();

        Assert.Empty(translation.GetDomainEvents());
    }

    [Fact]
    public void MarkAsNotFavourite_SetsUnfavouriteAndRaisesEvent()
    {
        var translation = CreateTranslation();
        translation.MarkAsFavourite();
        translation.ClearDomainEvents();

        translation.MarkAsNotFavourite();

        Assert.False(translation.IsFavourite);

        var domainEvent = Assert.Single(translation.GetDomainEvents());
        var unfavourite = Assert.IsType<TranslationMarkedAsUnfavouriteEvent>(domainEvent);
        Assert.Equal(translation.Id, unfavourite.TranslationId);
    }

    [Fact]
    public void LinkAndUnlinkGlyph_UpdatesGlyphId()
    {
        var translation = Translation.Create(null, "あおい", "aoi", "blue");

        var glyphId = Guid.NewGuid();
        translation.LinkToGlyph(glyphId);
        Assert.Equal(glyphId, translation.GlyphId);

        translation.UnlinkFromGlyph();
        Assert.Null(translation.GlyphId);
    }
}