namespace Core.Application.DTOs;

public record TranslationDto(
    Guid Id,
    Guid? GlyphId,
    string JapaneseWriting,
    string RomajiWriting,
    string Translation,
    string? ImageFileName,
    bool IsLearned,
    bool IsFavourite);