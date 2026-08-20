using Core.Domain.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Translations.AddTranslation;

public class AddTranslationCommandHandler : IRequestHandler<AddTranslationCommand>
{
    private readonly IGlyphRepository _glyphRepository;
    private readonly ITranslationRepository _translationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddTranslationCommandHandler(IGlyphRepository glyphRepository, ITranslationRepository translationRepository, IUnitOfWork unitOfWork)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
        _translationRepository = translationRepository ?? throw new ArgumentNullException(nameof(translationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(AddTranslationCommand command, CancellationToken cancellationToken = default)
    {
        var glyph = await _glyphRepository.GetByIdAsync(command.GlyphId, cancellationToken)
            ?? throw new KeyNotFoundException($"Glyph '{command.GlyphId}' was not found.");

        var translation = Translation.Create(
            glyph.Id,
            command.JapaneseWriting,
            command.RomajiWriting,
            command.Translation,
            command.ImageFileName);

        await _translationRepository.AddAsync(translation, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}