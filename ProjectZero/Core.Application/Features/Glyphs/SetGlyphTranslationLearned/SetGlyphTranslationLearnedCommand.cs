using MediatR;

namespace Core.Application.Features.Glyphs.SetGlyphTranslationLearned;

public record SetGlyphTranslationLearnedCommand(
    Guid GlyphId,
    string JapaneseWriting,
    string RomajiWriting,
    string Translation,
    bool IsLearned) : IRequest;