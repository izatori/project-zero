using System.Text;
using System.Text.RegularExpressions;
using Core.Domain.Abstractions;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

/// <summary>
/// Glyph aggregate root representing a single Hiragana, Katakana or Kanji character
/// and everything needed to learn it.
/// </summary>
public class Glyph : AggregateRoot<Guid>
{
    private Glyph(Guid id, string character, string romaji, GlyphType type, string imageFileName, string? strokeAnimationFileName) : base(id)
    {
        Character = character;
        Romaji = romaji;
        Type = type;
        ImageFileName = imageFileName;
        StrokeAnimationFileName = strokeAnimationFileName;
        IsLearned = false;
        IsFavourite = false;
    }

    public string Character { get; private set; }

    public string Romaji { get; private set; }

    public GlyphType Type { get; private set; }

    public string ImageFileName { get; private set; }

    public string? StrokeAnimationFileName { get; private set; }

    public bool IsLearned { get; private set; }

    public bool IsFavourite { get; private set; }

    /// <summary>
    /// Factory method to create a new Glyph.
    /// Contains the business logic for glyph creation.
    /// </summary>
    public static Glyph Create(string character, string romaji, GlyphType type, string imageFileName, string? strokeAnimationFileName = null)
    {
        ValidateCharacter(character);
        ValidateRomaji(romaji);
        ValidateImageFileName(imageFileName);
        ValidateStrokeAnimationFileName(strokeAnimationFileName);

        var glyph = new Glyph(Guid.NewGuid(), character, romaji, type, imageFileName, strokeAnimationFileName);

        glyph.RaiseDomainEvent(new GlyphCreatedEvent(glyph.Id, character, romaji, type, imageFileName, strokeAnimationFileName));

        return glyph;
    }

    /// <summary>
    /// Updates the character information of the glyph.
    /// Contains the business logic for glyph updates.
    /// </summary>
    public void Update(string character, string romaji, GlyphType type, string imageFileName, string? strokeAnimationFileName)
    {
        ValidateCharacter(character);
        ValidateRomaji(romaji);
        ValidateImageFileName(imageFileName);
        ValidateStrokeAnimationFileName(strokeAnimationFileName);

        Character = character;
        Romaji = romaji;
        Type = type;
        ImageFileName = imageFileName;
        StrokeAnimationFileName = strokeAnimationFileName;
    }

    /// <summary>
    /// Marks the glyph as learned.
    /// </summary>
    public void MarkAsLearned()
    {
        if (IsLearned)
        {
            return;
        }

        IsLearned = true;

        RaiseDomainEvent(new GlyphMarkedLearnedEvent(Id));
    }

    /// <summary>
    /// Marks the glyph as not learned yet.
    /// </summary>
    public void MarkAsNotLearned()
    {
        if (!IsLearned)
        {
            return;
        }
    
        IsLearned = false;
        
        RaiseDomainEvent(new GlyphMarkedAsUnlearnedEvent(Id));
    }

    /// <summary>
    /// Marks the glyph as a favourite.
    /// </summary>
    public void MarkAsFavourite()
    {
        if (IsFavourite)
        {
            return;
        }

        IsFavourite = true;

        RaiseDomainEvent(new GlyphMarkedFavouriteEvent(Id));
    }

    /// <summary>
    /// Marks the glyph as not a favourite.
    /// </summary>
    public void MarkAsNotFavourite()
    {
        if (!IsFavourite)
        {
            return;
        }

        IsFavourite = false;

        RaiseDomainEvent(new GlyphMarkedAsUnfavouriteEvent(Id));
    }

    /// <summary>
    /// Validates that the character consists of one or more Hiragana/Katakana characters
    /// (single characters such as あ or compound syllables such as きゃ).
    /// </summary>
    private static void ValidateCharacter(string character)
    {
        if (string.IsNullOrWhiteSpace(character))
            throw new ArgumentException("Character cannot be empty", nameof(character));

        var length = Rune.DecodeFromUtf16(character, out _, out var consumed) == System.Buffers.OperationStatus.Done
            ? consumed
            : character.Length;

        if (length is < 1 or > 3)
            throw new ArgumentException("Character must contain between 1 and 3 characters", nameof(character));
    }

    /// <summary>
    /// Validates the romaji reading.
    /// </summary>
    private static void ValidateRomaji(string romaji)
    {
        if (string.IsNullOrWhiteSpace(romaji))
            throw new ArgumentException("Romaji cannot be empty", nameof(romaji));
    }

    /// <summary>
    /// Validates the image file name.
    /// </summary>
    private static void ValidateImageFileName(string imageFileName)
    {
        if (string.IsNullOrWhiteSpace(imageFileName))
            throw new ArgumentException("Image file name cannot be empty", nameof(imageFileName));

        if (imageFileName.Contains(' '))
            throw new ArgumentException("Image file name cannot contain spaces", nameof(imageFileName));

        if (!Regex.IsMatch(imageFileName, @"^[a-zA-Z0-9_-]+\.(jpe?g|png|webp)$", RegexOptions.IgnoreCase))
            throw new ArgumentException("Image file name must only contain letters, numbers, - and _ followed by .jpg, .jpeg, .png or .webp", nameof(imageFileName));
    }

    /// <summary>
    /// Validates the stroke animation file name. Optional; when provided must be a .gif file.
    /// </summary>
    private static void ValidateStrokeAnimationFileName(string? strokeAnimationFileName)
    {
        if (string.IsNullOrWhiteSpace(strokeAnimationFileName))
            return;

        if (strokeAnimationFileName.Contains(' '))
            throw new ArgumentException("Stroke animation file name cannot contain spaces", nameof(strokeAnimationFileName));

        if (!Regex.IsMatch(strokeAnimationFileName, @"^[a-zA-Z0-9_-]+\.gif$", RegexOptions.IgnoreCase))
            throw new ArgumentException("Stroke animation file name must only contain letters, numbers, - and _ followed by .gif", nameof(strokeAnimationFileName));
    }
}

/// <summary>
/// Domain event raised when a glyph is created.
/// </summary>
public class GlyphCreatedEvent : DomainEvent
{
    public GlyphCreatedEvent(Guid id, string character, string romaji, GlyphType type, string imageFileName, string? strokeAnimationFileName)
    {
        GlyphId = id;
        Character = character;
        Romaji = romaji;
        Type = type;
        ImageFileName = imageFileName;
        StrokeAnimationFileName = strokeAnimationFileName;
    }

    public Guid GlyphId { get; private set; }
    public string Character { get; private set; }
    public string Romaji { get; private set; }
    public GlyphType Type { get; private set; }
    public string ImageFileName { get; private set; }
    public string? StrokeAnimationFileName { get; private set; }
}

/// <summary>
/// Domain event raised when a glyph is marked as learned.
/// </summary>
public class GlyphMarkedLearnedEvent : DomainEvent
{
    public GlyphMarkedLearnedEvent(Guid id)
    {
        GlyphId = id;
    }

    public Guid GlyphId { get; private set; }
}

/// <summary>
/// Domain event raised when a glyph is marked as unlearned.
/// </summary>
public class GlyphMarkedAsUnlearnedEvent : DomainEvent
{
    public GlyphMarkedAsUnlearnedEvent(Guid id)
    {
        GlyphId = id;
    }

    public Guid GlyphId { get; private set; }
}

/// <summary>
/// Domain event raised when a glyph is marked as a favourite.
/// </summary>
public class GlyphMarkedFavouriteEvent : DomainEvent
{
    public GlyphMarkedFavouriteEvent(Guid id)
    {
        GlyphId = id;
    }

    public Guid GlyphId { get; private set; }
}

/// <summary>
/// Domain event raised when a glyph is marked as not a favourite.
/// </summary>
public class GlyphMarkedAsUnfavouriteEvent : DomainEvent
{
    public GlyphMarkedAsUnfavouriteEvent(Guid id)
    {
        GlyphId = id;
    }

    public Guid GlyphId { get; private set; }
}