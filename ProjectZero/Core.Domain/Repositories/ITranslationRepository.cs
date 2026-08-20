using Core.Domain.Abstractions;
using Core.Domain.Entities;

namespace Core.Domain.Repositories;

public interface ITranslationRepository : IRepository<Translation, Guid>
{
    Task<List<Translation>> GetByGlyphIdAsync(Guid? glyphId, CancellationToken cancellationToken = default);

    Task<List<Translation>> GetByGlyphIdsAsync(IEnumerable<Guid> glyphIds, CancellationToken cancellationToken = default);
}