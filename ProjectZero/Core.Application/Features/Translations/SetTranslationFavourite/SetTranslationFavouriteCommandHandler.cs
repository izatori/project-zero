using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Translations.SetTranslationFavourite;

public class SetTranslationFavouriteCommandHandler : IRequestHandler<SetTranslationFavouriteCommand>
{
    private readonly ITranslationRepository _translationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetTranslationFavouriteCommandHandler(ITranslationRepository translationRepository, IUnitOfWork unitOfWork)
    {
        _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(SetTranslationFavouriteCommand command, CancellationToken cancellationToken = default)
    {
        var translation = await _translationRepository.GetByIdAsync(command.TranslationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Translation '{command.TranslationId}' was not found.");

        if (command.IsFavourite)
        {
            translation.MarkAsFavourite();
        }
        else
        {
            translation.MarkAsNotFavourite();
        }

        await _translationRepository.UpdateAsync(translation, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}