using MediatR;

namespace Core.Application.Features.Translations.UpdateTranslationImage;

public record UpdateTranslationImageCommand(
    Guid TranslationId,
    string? ImageFileName) : IRequest;