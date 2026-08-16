using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Glyphs.UpdateGlyph;

public class UpdateGlyphCommandHandler : IRequestHandler<UpdateGlyphCommand>
{
    private readonly IGlyphRepository _glyphRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGlyphCommandHandler(IGlyphRepository glyphRepository, IUnitOfWork unitOfWork)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(UpdateGlyphCommand command, CancellationToken cancellationToken = default)
    {
        var glyph = await _glyphRepository.GetByIdAsync(command.GlyphId, cancellationToken)
            ?? throw new KeyNotFoundException($"Glyph '{command.GlyphId}' was not found.");

        glyph.Update(
            command.Character,
            command.Romaji,
            command.GlyphType,
            command.ImageFileName,
            command.StrokeAnimationFileName);

        await _glyphRepository.UpdateAsync(glyph, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}