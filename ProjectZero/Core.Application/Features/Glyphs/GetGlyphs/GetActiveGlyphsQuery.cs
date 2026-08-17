using Core.Application.DTOs;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Glyphs.GetGlyphs;

/// <summary>
/// Query to get active (not yet learned) glyphs of a given type.
/// Queries represent read operations in CQRS.
/// </summary>
/// <param name="GlyphType">The type of glyph to retrieve (Hiragana, Katakana or Kanji level).</param>
public record GetActiveGlyphsQuery(GlyphType GlyphType) : IRequest<IEnumerable<GlyphDto>>;