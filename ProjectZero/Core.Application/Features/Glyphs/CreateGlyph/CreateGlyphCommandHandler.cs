using Core.Domain.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Glyphs.CreateGlyph;

public class CreateGlyphCommandHandler : IRequestHandler<CreateGlyphCommand, Guid>
{
    private readonly IGlyphRepository _glyphRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGlyphCommandHandler(IGlyphRepository glyphRepository, IUnitOfWork unitOfWork)
    {
        _glyphRepository = glyphRepository ?? throw new ArgumentNullException(nameof(glyphRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Guid> Handle(CreateGlyphCommand command, CancellationToken cancellationToken = default)
    {
        var glyph = Glyph.Create(
            command.Character,
            command.Romaji,
            command.GlyphType,
            command.ImageFileName,
            command.StrokeAnimationFileName);

        await _glyphRepository.AddAsync(glyph, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return glyph.Id;
    }
}