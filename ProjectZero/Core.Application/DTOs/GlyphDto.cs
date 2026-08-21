using Core.Domain.Enums;

namespace Core.Application.DTOs;

public record GlyphDto(
    Guid Id,
    string Character,
    string Romaji,
    GlyphType Type,
    string ImageFileName,
    string? StrokeAnimationFileName,
    bool IsLearned,
    bool IsFavourite,
    IReadOnlyCollection<TranslationDto> Translations);