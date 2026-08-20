using MediatR;

namespace Core.Application.Features.Translations.SetTranslationLearned;

public record SetTranslationLearnedCommand(
    Guid TranslationId,
    bool IsLearned) : IRequest;