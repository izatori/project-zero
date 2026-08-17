using Core.Domain.Entities;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Glyphs.CreateGlyph;

public record CreateGlyphCommand(
    string Character, 
    string Romaji, 
    GlyphType GlyphType, 
    string ImageFileName, 
    string? StrokeAnimationFileName) : IRequest<Guid>;
