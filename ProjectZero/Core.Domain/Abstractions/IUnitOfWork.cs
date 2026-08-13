namespace Core.Domain.Abstractions;

/// <summary>
/// Unit of Work pattern interface.
/// Coordinates the writing out of changes and maintains a list of
/// objects affected by a business transaction.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}