using MediatR;

namespace Core.Application.Features.Glyphs.SetGlyphFavourite;

public record SetGlyphFavouriteCommand(
    Guid GlyphId,
    bool IsFavourite) : IRequest;