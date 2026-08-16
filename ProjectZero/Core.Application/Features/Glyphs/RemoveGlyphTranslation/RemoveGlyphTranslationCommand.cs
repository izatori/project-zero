using MediatR;

namespace Core.Application.Features.Glyphs.RemoveGlyphTranslation;

public record RemoveGlyphTranslationCommand(
    Guid GlyphId,
    string JapaneseWriting,
    string RomajiWriting,
    string Translation) : IRequest;