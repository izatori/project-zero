namespace Core.Application.Abstractions;

/// <summary>
/// Handler interface for queries.
/// </summary>
/// <typeparam name="TQuery">The query type this handler processes</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}