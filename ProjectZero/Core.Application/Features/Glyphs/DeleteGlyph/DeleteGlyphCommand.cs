using MediatR;

namespace Core.Application.Features.Glyphs.DeleteGlyph;

public record DeleteGlyphCommand(Guid GlyphId) : IRequest;