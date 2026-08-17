using Core.Domain.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Domain.Repositories;

public interface IGlyphRepository : IRepository<Glyph, Guid>
{
    Task<List<Glyph>> GetActiveByTypeAsync(GlyphType glyphType, CancellationToken cancellationToken = default);
    
    Task<List<Glyph>> GetByTypeAsync(GlyphType glyphType, CancellationToken cancellationToken = default);
}