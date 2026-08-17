using Core.Application.DTOs;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Glyphs.GetGlyphs;

/// <summary>
/// Query to get all glyphs of a given type, regardless of their learned status.
/// Used for management and editing.
/// </summary>
/// <param name="GlyphType">The type of glyph to retrieve (Hiragana, Katakana or Kanji level).</param>
public record GetAllGlyphsQuery(GlyphType GlyphType) : IRequest<IEnumerable<GlyphDto>>;