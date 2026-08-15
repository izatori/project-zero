using Core.Domain.Abstractions;
using Core.Domain.Entities;

namespace Core.Domain.Repositories;

public interface IGlyphRepository : IRepository<Glyph, Guid>
{
    
}