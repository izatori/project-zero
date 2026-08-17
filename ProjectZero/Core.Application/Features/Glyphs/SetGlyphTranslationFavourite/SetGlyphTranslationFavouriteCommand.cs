using MediatR;

namespace Core.Application.Features.Glyphs.SetGlyphTranslationFavourite;

public record SetGlyphTranslationFavouriteCommand(
    Guid GlyphId,
    string JapaneseWriting,
    string RomajiWriting,
    string Translation,
    bool IsFavourite) : IRequest;