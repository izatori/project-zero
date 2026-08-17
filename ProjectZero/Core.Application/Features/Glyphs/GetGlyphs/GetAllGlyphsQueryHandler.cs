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

    public GetAllGlyphsQueryHandler(IGlyphRepository glyphRepository)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
    }

    public async Task<IEnumerable<GlyphDto>> Handle(GetAllGlyphsQuery query, CancellationToken cancellationToken = default)
    {
        var glyphs = await _glyphRepository.GetByTypeAsync(query.GlyphType, cancellationToken);

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
                t.Translation,
                t.ImageFileName,
                t.IsLearned,
                t.IsFavourite)).ToList()));
    }
}