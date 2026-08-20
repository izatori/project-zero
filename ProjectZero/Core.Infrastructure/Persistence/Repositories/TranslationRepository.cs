using Core.Domain.Entities;
using Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Translation aggregate root.
/// Handles persistence of Translation entities using Entity Framework Core.
/// </summary>
public class TranslationRepository : ITranslationRepository
{
    private readonly ApplicationDbContext _context;

    public TranslationRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Tracks a new translation so it will be inserted when the unit of work is saved.
    /// </summary>
    public async Task AddAsync(Translation aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Translations.AddAsync(aggregate, cancellationToken);
    }

    /// <summary>
    /// Marks an existing translation as modified so its changes are persisted on save.
    /// </summary>
    public async Task UpdateAsync(Translation aggregate, CancellationToken cancellationToken = default)
    {
        _context.Translations.Update(aggregate);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Marks a translation for deletion so it is removed on save.
    /// </summary>
    public async Task DeleteAsync(Translation aggregate, CancellationToken cancellationToken = default)
    {
        _context.Translations.Remove(aggregate);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves a translation by its identifier, or null if it does not exist.
    /// </summary>
    public async Task<Translation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Translations.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves translations linked to the given glyph, ordered by creation.
    /// </summary>
    public async Task<List<Translation>> GetByGlyphIdAsync(Guid? glyphId, CancellationToken cancellationToken = default)
    {
        return await _context.Translations
            .Where(t => t.GlyphId == glyphId)
            .OrderBy(t => t.JapaneseWriting)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves translations linked to any of the given glyphs.
    /// </summary>
    public async Task<List<Translation>> GetByGlyphIdsAsync(IEnumerable<Guid> glyphIds, CancellationToken cancellationToken = default)
    {
        var ids = glyphIds.ToList();

        if (ids.Count == 0)
        {
            return [];
        }

        return await _context.Translations
            .Where(t => t.GlyphId != null && ids.Contains(t.GlyphId.Value))
            .OrderBy(t => t.JapaneseWriting)
            .ToListAsync(cancellationToken);
    }
}