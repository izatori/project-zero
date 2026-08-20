using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Translations.UpdateTranslationImage;

public class UpdateTranslationImageCommandHandler : IRequestHandler<UpdateTranslationImageCommand>
{
    private readonly ITranslationRepository _translationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTranslationImageCommandHandler(ITranslationRepository translationRepository, IUnitOfWork unitOfWork)
    {
        _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(UpdateTranslationImageCommand command, CancellationToken cancellationToken = default)
    {
        var translation = await _translationRepository.GetByIdAsync(command.TranslationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Translation '{command.TranslationId}' was not found.");

        translation.SetImageFileName(command.ImageFileName);

        await _translationRepository.UpdateAsync(translation, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}