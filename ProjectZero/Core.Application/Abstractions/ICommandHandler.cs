namespace Core.Application.Abstractions;

/// <summary>
/// Handler interface for commands without a response.
/// </summary>
/// <typeparam name="TCommand">The command type this handler processes</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler interface for commands with a response.
/// </summary>
/// <typeparam name="TCommand">The command type this handler processes</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}