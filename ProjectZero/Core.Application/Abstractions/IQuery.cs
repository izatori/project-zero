namespace Core.Application.Abstractions;

/// <summary>
/// Marker interface for query objects.
/// Queries represent requests to retrieve data without modifying domain state.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the query</typeparam>
public interface IQuery<out TResponse>
{
}