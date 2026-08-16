using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Glyphs.UpdateGlyph;

public record UpdateGlyphCommand(
    Guid GlyphId,
    string Character,
    string Romaji,
    GlyphType GlyphType,
    string ImageFileName,
    string? StrokeAnimationFileName) : IRequest;