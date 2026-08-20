using MediatR;

namespace Core.Application.Features.Translations.RemoveTranslation;

public record RemoveTranslationCommand(Guid TranslationId) : IRequest;