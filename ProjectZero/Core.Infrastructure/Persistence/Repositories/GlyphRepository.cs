using Core.Domain.Entities;
using Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Repositories;

public class GlyphRepository : IGlyphRepository
{
    private readonly ApplicationDbContext _context;

    public GlyphRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(Glyph aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Glyphs.AddAsync(aggregate, cancellationToken);
    }

    public async Task UpdateAsync(Glyph aggregate, CancellationToken cancellationToken = default)
    {
        _context.Glyphs.Update(aggregate);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Glyph aggregate, CancellationToken cancellationToken = default)
    {
        _context.Glyphs.Remove(aggregate);
        await Task.CompletedTask;
    }

    public async Task<Glyph?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Glyphs.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<List<Glyph>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Glyphs
            .OrderBy(g => g.Character)
            .ToListAsync(cancellationToken);
    }
}