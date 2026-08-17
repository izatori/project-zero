using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Glyph aggregate root.
/// Handles persistence of Glyph entities using Entity Framework Core.
/// </summary>
public class GlyphRepository : IGlyphRepository
{
    private readonly ApplicationDbContext _context;

    public GlyphRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Tracks a new glyph so it will be inserted when the unit of work is saved.
    /// </summary>
    public async Task AddAsync(Glyph aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Glyphs.AddAsync(aggregate, cancellationToken);
    }

    /// <summary>
    /// Marks an existing glyph as modified so its changes are persisted on save.
    /// </summary>
    public async Task UpdateAsync(Glyph aggregate, CancellationToken cancellationToken = default)
    {
        _context.Glyphs.Update(aggregate);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Marks a glyph for deletion so it is removed on save.
    /// </summary>
    public async Task DeleteAsync(Glyph aggregate, CancellationToken cancellationToken = default)
    {
        _context.Glyphs.Remove(aggregate);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves a glyph by its identifier, or null if it does not exist.
    /// </summary>
    public async Task<Glyph?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Glyphs.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves glyphs of the given type that have not yet been learned, ordered by character.
    /// </summary>
    public async Task<List<Glyph>> GetActiveByTypeAsync(GlyphType glyphType, CancellationToken cancellationToken = default)
    {
        return await _context.Glyphs
            .Where(g => g.Type == glyphType && !g.IsLearned)
            .OrderBy(g => g.Character)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all glyphs of the given type, ordered by character.
    /// </summary>
    public async Task<List<Glyph>> GetByTypeAsync(GlyphType glyphType, CancellationToken cancellationToken = default)
    {
        return await _context.Glyphs
            .Where(g => g.Type == glyphType)
            .OrderBy(g => g.Character)
            .ToListAsync(cancellationToken);
    }
}