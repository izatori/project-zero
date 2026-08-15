using System.Text;
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

    private Glyph(Guid id, string character, string romaji, GlyphType type) : base(id)
    {
        Character = character;
        Romaji = romaji;
        Type = type;
        IsLearned = false;
    }

    public string Character { get; private set; }

    public string Romaji { get; private set; }

    public GlyphType Type { get; private set; }

    public bool IsLearned { get; private set; }

    public IReadOnlyCollection<GlyphTranslation> Translations => _translations.AsReadOnly();

    /// <summary>
    /// Factory method to create a new Glyph.
    /// Contains the business logic for glyph creation.
    /// </summary>
    public static Glyph Create(string character, string romaji, GlyphType type)
    {
        ValidateCharacter(character);
        ValidateRomaji(romaji);

        var glyph = new Glyph(Guid.NewGuid(), character, romaji, type);

        glyph.RaiseDomainEvent(new GlyphCreatedEvent(glyph.Id, character, romaji, type));

        return glyph;
    }

    /// <summary>
    /// Adds a translation example to the glyph.
    /// </summary>
    public void AddTranslation(string japaneseWriting, string romajiWriting, string translation)
    {
        if (string.IsNullOrWhiteSpace(japaneseWriting))
            throw new ArgumentException("Japanese writing cannot be empty", nameof(japaneseWriting));

        if (string.IsNullOrWhiteSpace(romajiWriting))
            throw new ArgumentException("Romaji writing cannot be empty", nameof(romajiWriting));

        if (string.IsNullOrWhiteSpace(translation))
            throw new ArgumentException("Translation cannot be empty", nameof(translation));

        _translations.Add(new GlyphTranslation(japaneseWriting, romajiWriting, translation));
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
    /// Validates that the character is a single character.
    /// </summary>
    private static void ValidateCharacter(string character)
    {
        if (string.IsNullOrWhiteSpace(character))
            throw new ArgumentException("Character cannot be empty", nameof(character));

        if (Rune.DecodeFromUtf16(character, out _, out var consumed) != System.Buffers.OperationStatus.Done
            || consumed != character.Length)
        {
            throw new ArgumentException("Character must be a single character", nameof(character));
        }
    }

    /// <summary>
    /// Validates the romaji reading.
    /// </summary>
    private static void ValidateRomaji(string romaji)
    {
        if (string.IsNullOrWhiteSpace(romaji))
            throw new ArgumentException("Romaji cannot be empty", nameof(romaji));
    }
}

/// <summary>
/// Value object representing a translation example for a glyph.
/// </summary>
public class GlyphTranslation : ValueObject
{
    public GlyphTranslation(string japaneseWriting, string romajiWriting, string translation)
    {
        JapaneseWriting = japaneseWriting;
        RomajiWriting = romajiWriting;
        Translation = translation;
    }

    public string JapaneseWriting { get; }

    public string RomajiWriting { get; }

    public string Translation { get; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return JapaneseWriting;
        yield return RomajiWriting;
        yield return Translation;
    }
}

/// <summary>
/// Domain event raised when a glyph is created.
/// </summary>
public class GlyphCreatedEvent : DomainEvent
{
    public GlyphCreatedEvent(Guid id, string character, string romaji, GlyphType type)
    {
        GlyphId = id;
        Character = character;
        Romaji = romaji;
        Type = type;
    }

    public Guid GlyphId { get; private set; }
    public string Character { get; private set; }
    public string Romaji { get; private set; }
    public GlyphType Type { get; private set; }
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