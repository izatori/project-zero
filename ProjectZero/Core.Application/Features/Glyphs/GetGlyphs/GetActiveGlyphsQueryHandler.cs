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
    private readonly ITranslationRepository _translationRepository;

    public GetActiveGlyphsQueryHandler(IGlyphRepository glyphRepository, ITranslationRepository translationRepository)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
        _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
    }

    /// <summary>
    /// Handles the query and maps the glyphs to DTOs.
    /// </summary>
    public async Task<IEnumerable<GlyphDto>> Handle(GetActiveGlyphsQuery query, CancellationToken cancellationToken = default)
    {
        var glyphs = await _glyphRepository.GetActiveByTypeAsync(query.GlyphType, cancellationToken);

        var translations = await _translationRepository.GetByGlyphIdsAsync(glyphs.Select(g => g.Id), cancellationToken);
        var translationsByGlyph = translations.ToLookup(t => t.GlyphId!.Value);

        return glyphs.Select(g => new GlyphDto(
            g.Id,
            g.Character,
            g.Romaji,
            g.Type,
            g.ImageFileName,
            g.StrokeAnimationFileName,
            g.IsLearned,
            g.IsFavourite,
            translationsByGlyph[g.Id].Select(t => new TranslationDto(
                t.Id,
                t.GlyphId,
                t.JapaneseWriting,
                t.RomajiWriting,
                t.English,
                t.ImageFileName,
                t.IsLearned,
                t.IsFavourite)).ToList()));
    }
}