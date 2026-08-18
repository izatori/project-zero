using MediatR;

namespace Core.Application.Features.Glyphs.SetGlyphLearned;

public record SetGlyphLearnedCommand(
    Guid GlyphId,
    bool IsLearned) : IRequest;