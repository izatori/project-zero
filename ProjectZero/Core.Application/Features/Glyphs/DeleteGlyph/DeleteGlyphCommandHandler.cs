using Core.Domain.Abstractions;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Glyphs.DeleteGlyph;

public class DeleteGlyphCommandHandler : IRequestHandler<DeleteGlyphCommand>
{
    private readonly IGlyphRepository _glyphRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGlyphCommandHandler(IGlyphRepository glyphRepository, IUnitOfWork unitOfWork)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(DeleteGlyphCommand command, CancellationToken cancellationToken = default)
    {
        var glyph = await _glyphRepository.GetByIdAsync(command.GlyphId, cancellationToken)
            ?? throw new KeyNotFoundException($"Glyph '{command.GlyphId}' was not found.");

        await _glyphRepository.DeleteAsync(glyph, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}