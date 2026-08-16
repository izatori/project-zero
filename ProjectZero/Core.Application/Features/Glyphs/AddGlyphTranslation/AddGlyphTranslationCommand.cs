using MediatR;

namespace Core.Application.Features.Glyphs.AddGlyphTranslation;

public record AddGlyphTranslationCommand(
    Guid GlyphId,
    string JapaneseWriting,
    string RomajiWriting,
    string Translation,
    string? ImageFileName = null) : IRequest;