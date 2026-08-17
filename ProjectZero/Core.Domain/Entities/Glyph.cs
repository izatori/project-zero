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
    private readonly List<GlyphTranslation> _translations = [];

    private Glyph(Guid id, string character, string romaji, GlyphType type, string imageFileName, string? strokeAnimationFileName) : base(id)
    {
        Character = character;
        Romaji = romaji;
        Type = type;
        ImageFileName = imageFileName;
        StrokeAnimationFileName = strokeAnimationFileName;
        IsLearned = false;
    }

    public string Character { get; private set; }

    public string Romaji { get; private set; }

    public GlyphType Type { get; private set; }

    public string ImageFileName { get; private set; }

    public string? StrokeAnimationFileName { get; private set; }

    public bool IsLearned { get; private set; }

    public IReadOnlyCollection<GlyphTranslation> Translations => _translations.AsReadOnly();

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
    /// Adds a translation example to the glyph.
    /// </summary>
    public void AddTranslation(string japaneseWriting, string romajiWriting, string translation, string? imageFileName = null)
    {
        if (string.IsNullOrWhiteSpace(japaneseWriting))
            throw new ArgumentException("Japanese writing cannot be empty", nameof(japaneseWriting));

        if (string.IsNullOrWhiteSpace(romajiWriting))
            throw new ArgumentException("Romaji writing cannot be empty", nameof(romajiWriting));

        if (string.IsNullOrWhiteSpace(translation))
            throw new ArgumentException("Translation cannot be empty", nameof(translation));

        _translations.Add(new GlyphTranslation(japaneseWriting, romajiWriting, translation, imageFileName));
    }

    /// <summary>
    /// Removes a translation example from the glyph that matches the given values.
    /// </summary>
    public void RemoveTranslation(string japaneseWriting, string romajiWriting, string translation)
    {
        var match = _translations.FirstOrDefault(t =>
            t.JapaneseWriting == japaneseWriting &&
            t.RomajiWriting == romajiWriting &&
            t.Translation == translation);

        if (match is not null)
        {
            _translations.Remove(match);
        }
    }

    /// <summary>
    /// Sets (or clears) the image of a translation that matches the given values.
    /// </summary>
    public void SetTranslationImage(string japaneseWriting, string romajiWriting, string translation, string? imageFileName)
    {
        var match = _translations.FirstOrDefault(t =>
            t.JapaneseWriting == japaneseWriting &&
            t.RomajiWriting == romajiWriting &&
            t.Translation == translation);

        if (match is null)
        {
            throw new KeyNotFoundException("Translation was not found.");
        }

        match.SetImageFileName(imageFileName);
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
        IsLearned = false;
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
/// Value object representing a translation example for a glyph.
/// </summary>
public class GlyphTranslation : ValueObject
{
    public GlyphTranslation(string japaneseWriting, string romajiWriting, string translation, string? imageFileName = null)
    {
        JapaneseWriting = japaneseWriting;
        RomajiWriting = romajiWriting;
        Translation = translation;
        ImageFileName = imageFileName;
    }

    public string JapaneseWriting { get; }

    public string RomajiWriting { get; }

    public string Translation { get; }

    public string? ImageFileName { get; private set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return JapaneseWriting;
        yield return RomajiWriting;
        yield return Translation;
    }

    /// <summary>
    /// Sets or clears the image for this translation.
    /// </summary>
    public void SetImageFileName(string? imageFileName)
    {
        ImageFileName = imageFileName;
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