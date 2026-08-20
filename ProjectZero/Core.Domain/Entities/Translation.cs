using Core.Domain.Abstractions;

namespace Core.Domain.Entities;

/// <summary>
/// Translation aggregate root representing a single example translation.
/// A translation may optionally be linked to a glyph, but can exist on its own.
/// </summary>
public class Translation : AggregateRoot<Guid>
{
    private Translation(Guid id, Guid? glyphId, string japaneseWriting, string romajiWriting, string english, string? imageFileName) : base(id)
    {
        GlyphId = glyphId;
        JapaneseWriting = japaneseWriting;
        RomajiWriting = romajiWriting;
        English = english;
        ImageFileName = imageFileName;
        IsLearned = false;
        IsFavourite = false;
    }

    /// <summary>
    /// The glyph this translation is linked to, if any.
    /// </summary>
    public Guid? GlyphId { get; private set; }

    public string JapaneseWriting { get; private set; }

    public string RomajiWriting { get; private set; }

    /// <summary>
    /// The English meaning of the translation.
    /// </summary>
    public string English { get; private set; }

    public string? ImageFileName { get; private set; }

    public bool IsLearned { get; private set; }

    public bool IsFavourite { get; private set; }

    /// <summary>
    /// Factory method to create a new Translation.
    /// Contains the business logic for translation creation.
    /// </summary>
    public static Translation Create(Guid? glyphId, string japaneseWriting, string romajiWriting, string translation, string? imageFileName = null)
    {
        ValidateJapaneseWriting(japaneseWriting);
        ValidateRomajiWriting(romajiWriting);
        ValidateTranslation(translation);

        var entity = new Translation(Guid.NewGuid(), glyphId, japaneseWriting, romajiWriting, translation, imageFileName);

        entity.RaiseDomainEvent(new TranslationCreatedEvent(entity.Id, entity.GlyphId, japaneseWriting, romajiWriting, translation, imageFileName));

        return entity;
    }

    /// <summary>
    /// Links this translation to a glyph.
    /// </summary>
    public void LinkToGlyph(Guid glyphId)
    {
        GlyphId = glyphId;
    }

    /// <summary>
    /// Unlinks this translation from its glyph, keeping the translation itself.
    /// </summary>
    public void UnlinkFromGlyph()
    {
        GlyphId = null;
    }

    /// <summary>
    /// Sets or clears the image for this translation.
    /// </summary>
    public void SetImageFileName(string? imageFileName)
    {
        if (ImageFileName == imageFileName)
        {
            return;
        }

        ImageFileName = imageFileName;

        RaiseDomainEvent(new TranslationImageUpdatedEvent(Id, ImageFileName));
    }

    /// <summary>
    /// Marks the translation as learned.
    /// </summary>
    public void MarkAsLearned()
    {
        if (IsLearned)
        {
            return;
        }

        IsLearned = true;

        RaiseDomainEvent(new TranslationMarkedLearnedEvent(Id));
    }

    /// <summary>
    /// Marks the translation as not learned yet.
    /// </summary>
    public void MarkAsNotLearned()
    {
        if (!IsLearned)
        {
            return;
        }

        IsLearned = false;

        RaiseDomainEvent(new TranslationMarkedAsUnlearnedEvent(Id));
    }

    /// <summary>
    /// Marks the translation as a favourite.
    /// </summary>
    public void MarkAsFavourite()
    {
        if (IsFavourite)
        {
            return;
        }

        IsFavourite = true;

        RaiseDomainEvent(new TranslationMarkedFavouriteEvent(Id));
    }

    /// <summary>
    /// Marks the translation as not a favourite.
    /// </summary>
    public void MarkAsNotFavourite()
    {
        if (!IsFavourite)
        {
            return;
        }

        IsFavourite = false;

        RaiseDomainEvent(new TranslationMarkedAsUnfavouriteEvent(Id));
    }

    private static void ValidateJapaneseWriting(string japaneseWriting)
    {
        if (string.IsNullOrWhiteSpace(japaneseWriting))
            throw new ArgumentException("Japanese writing cannot be empty", nameof(japaneseWriting));
    }

    private static void ValidateRomajiWriting(string romajiWriting)
    {
        if (string.IsNullOrWhiteSpace(romajiWriting))
            throw new ArgumentException("Romaji writing cannot be empty", nameof(romajiWriting));
    }

    private static void ValidateTranslation(string translation)
    {
        if (string.IsNullOrWhiteSpace(translation))
            throw new ArgumentException("Translation cannot be empty", nameof(translation));
    }
}

/// <summary>
/// Domain event raised when a translation is created.
/// </summary>
public class TranslationCreatedEvent : DomainEvent
{
    public TranslationCreatedEvent(Guid translationId, Guid? glyphId, string japaneseWriting, string romajiWriting, string translation, string? imageFileName)
    {
        TranslationId = translationId;
        GlyphId = glyphId;
        JapaneseWriting = japaneseWriting;
        RomajiWriting = romajiWriting;
        Translation = translation;
        ImageFileName = imageFileName;
    }

    public Guid TranslationId { get; private set; }
    public Guid? GlyphId { get; private set; }
    public string JapaneseWriting { get; private set; }
    public string RomajiWriting { get; private set; }
    public string Translation { get; private set; }
    public string? ImageFileName { get; private set; }
}

/// <summary>
/// Domain event raised when a translation's image is updated.
/// </summary>
public class TranslationImageUpdatedEvent : DomainEvent
{
    public TranslationImageUpdatedEvent(Guid translationId, string? imageFileName)
    {
        TranslationId = translationId;
        ImageFileName = imageFileName;
    }

    public Guid TranslationId { get; private set; }
    public string? ImageFileName { get; private set; }
}

/// <summary>
/// Domain event raised when a translation is marked as learned.
/// </summary>
public class TranslationMarkedLearnedEvent : DomainEvent
{
    public TranslationMarkedLearnedEvent(Guid translationId)
    {
        TranslationId = translationId;
    }

    public Guid TranslationId { get; private set; }
}

/// <summary>
/// Domain event raised when a translation is marked as unlearned.
/// </summary>
public class TranslationMarkedAsUnlearnedEvent : DomainEvent
{
    public TranslationMarkedAsUnlearnedEvent(Guid translationId)
    {
        TranslationId = translationId;
    }

    public Guid TranslationId { get; private set; }
}

/// <summary>
/// Domain event raised when a translation is marked as a favourite.
/// </summary>
public class TranslationMarkedFavouriteEvent : DomainEvent
{
    public TranslationMarkedFavouriteEvent(Guid translationId)
    {
        TranslationId = translationId;
    }

    public Guid TranslationId { get; private set; }
}

/// <summary>
/// Domain event raised when a translation is marked as not a favourite.
/// </summary>
public class TranslationMarkedAsUnfavouriteEvent : DomainEvent
{
    public TranslationMarkedAsUnfavouriteEvent(Guid translationId)
    {
        TranslationId = translationId;
    }

    public Guid TranslationId { get; private set; }
}