namespace Core.Application.Abstractions;

/// <summary>
/// Mediator interface for dispatching commands and queries.
/// Provides a central point for handling all application operations.
/// </summary>
public interface IMediator
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}