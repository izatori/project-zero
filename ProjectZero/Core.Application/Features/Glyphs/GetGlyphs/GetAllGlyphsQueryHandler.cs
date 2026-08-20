using Core.Application.DTOs;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Glyphs.GetGlyphs;

/// <summary>
/// Handler for GetAllGlyphsQuery.
/// Retrieves all glyphs of a given type.
/// </summary>
public class GetAllGlyphsQueryHandler : IRequestHandler<GetAllGlyphsQuery, IEnumerable<GlyphDto>>
{
    private readonly IGlyphRepository _glyphRepository;
    private readonly ITranslationRepository _translationRepository;

    public GetAllGlyphsQueryHandler(IGlyphRepository glyphRepository, ITranslationRepository translationRepository)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
        _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
    }

    public async Task<IEnumerable<GlyphDto>> Handle(GetAllGlyphsQuery query, CancellationToken cancellationToken = default)
    {
        var glyphs = await _glyphRepository.GetByTypeAsync(query.GlyphType, cancellationToken);

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