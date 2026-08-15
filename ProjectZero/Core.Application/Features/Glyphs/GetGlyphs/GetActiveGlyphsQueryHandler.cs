using Core.Application.DTOs;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Glyphs.GetGlyphs;

/// <summary>
/// Handler for GetActiveGlyphsQuery.
/// Retrieves active (not learned) glyphs of a given type.
/// </summary>
public class GetActiveGlyphsQueryHandler : IRequestHandler<GetActiveGlyphsQuery, IEnumerable<GlyphDto>>
{
    private readonly IGlyphRepository _glyphRepository;

    public GetActiveGlyphsQueryHandler(IGlyphRepository glyphRepository)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
    }

    /// <summary>
    /// Handles the query and maps the glyphs to DTOs.
    /// </summary>
    public async Task<IEnumerable<GlyphDto>> Handle(GetActiveGlyphsQuery query, CancellationToken cancellationToken = default)
    {
        var glyphs = await _glyphRepository.GetActiveByTypeAsync(query.GlyphType, cancellationToken);

        return glyphs.Select(g => new GlyphDto(
            g.Id,
            g.Character,
            g.Romaji,
            g.Type,
            g.ImageFileName,
            g.StrokeAnimationFileName,
            g.IsLearned,
            g.Translations.Select(t => new GlyphTranslationDto(
                t.JapaneseWriting,
                t.RomajiWriting,
                t.Translation)).ToList()));
    }
}