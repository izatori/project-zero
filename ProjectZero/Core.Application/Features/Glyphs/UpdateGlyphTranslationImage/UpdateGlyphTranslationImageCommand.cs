using MediatR;

namespace Core.Application.Features.Glyphs.UpdateGlyphTranslationImage;

public record UpdateGlyphTranslationImageCommand(
    Guid GlyphId,
    string JapaneseWriting,
    string RomajiWriting,
    string Translation,
    string? ImageFileName) : IRequest;