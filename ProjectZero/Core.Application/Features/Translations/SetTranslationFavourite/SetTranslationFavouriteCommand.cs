using MediatR;

namespace Core.Application.Features.Translations.SetTranslationFavourite;

public record SetTranslationFavouriteCommand(
    Guid TranslationId,
    bool IsFavourite) : IRequest;