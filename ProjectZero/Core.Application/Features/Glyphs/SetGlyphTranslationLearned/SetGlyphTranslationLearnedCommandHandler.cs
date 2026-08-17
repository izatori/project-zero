using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Glyphs.SetGlyphTranslationLearned;

public class SetGlyphTranslationLearnedCommandHandler : IRequestHandler<SetGlyphTranslationLearnedCommand>
{
    private readonly IGlyphRepository _glyphRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetGlyphTranslationLearnedCommandHandler(IGlyphRepository glyphRepository, IUnitOfWork unitOfWork)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(SetGlyphTranslationLearnedCommand command, CancellationToken cancellationToken = default)
    {
        var glyph = await _glyphRepository.GetByIdAsync(command.GlyphId, cancellationToken)
            ?? throw new KeyNotFoundException($"Glyph '{command.GlyphId}' was not found.");

        glyph.SetTranslationLearned(
            command.JapaneseWriting,
            command.RomajiWriting,
            command.Translation,
            command.IsLearned);

        await _glyphRepository.UpdateAsync(glyph, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}