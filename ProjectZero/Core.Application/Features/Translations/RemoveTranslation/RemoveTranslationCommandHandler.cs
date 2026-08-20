using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Translations.RemoveTranslation;

public class RemoveTranslationCommandHandler : IRequestHandler<RemoveTranslationCommand>
{
    private readonly ITranslationRepository _translationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTranslationCommandHandler(ITranslationRepository translationRepository, IUnitOfWork unitOfWork)
    {
        _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(RemoveTranslationCommand command, CancellationToken cancellationToken = default)
    {
        var translation = await _translationRepository.GetByIdAsync(command.TranslationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Translation '{command.TranslationId}' was not found.");

        await _translationRepository.DeleteAsync(translation, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}