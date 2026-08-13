namespace Core.Domain.Abstractions;

/// <summary>
/// Base interface for repositories.
/// Repositories handle the persistence of aggregate roots and provide
/// a collection-like interface for accessing aggregates.
/// </summary>
/// <typeparam name="T">The aggregate root type</typeparam>
/// <typeparam name="TId">The type of the aggregate's identifier</typeparam>
public interface IRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : notnull
{
    Task AddAsync(T aggregate, CancellationToken cancellationToken = default);

    Task UpdateAsync(T aggregate, CancellationToken cancellationToken = default);

    Task DeleteAsync(T aggregate, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}