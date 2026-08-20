using MediatR;

namespace Core.Application.Features.Translations.AddTranslation;

public record AddTranslationCommand(
    Guid GlyphId,
    string JapaneseWriting,
    string RomajiWriting,
    string Translation,
    string? ImageFileName = null) : IRequest;