using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Translations.SetTranslationLearned;

public class SetTranslationLearnedCommandHandler : IRequestHandler<SetTranslationLearnedCommand>
{
    private readonly ITranslationRepository _translationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetTranslationLearnedCommandHandler(ITranslationRepository translationRepository, IUnitOfWork unitOfWork)
    {
        _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(SetTranslationLearnedCommand command, CancellationToken cancellationToken = default)
    {
        var translation = await _translationRepository.GetByIdAsync(command.TranslationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Translation '{command.TranslationId}' was not found.");

        if (command.IsLearned)
        {
            translation.MarkAsLearned();
        }
        else
        {
            translation.MarkAsNotLearned();
        }

        await _translationRepository.UpdateAsync(translation, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}