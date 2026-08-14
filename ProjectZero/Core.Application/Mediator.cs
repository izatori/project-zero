using Core.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

/// <summary>
/// Default implementation of the application <see cref="IMediator"/>.
///
/// Acts as a central dispatcher in the CQRS pipeline: it receives a command or query
/// and routes it to the matching handler resolved from the dependency injection container.
/// The Blazor UI (and any other consumer) only talks to <see cref="IMediator"/>, never
/// to individual handlers, keeping the application layer decoupled from its consumers.
///
/// Handlers are resolved per invocation (scoped/transient from DI), so no handler state
/// is cached here. Each dispatch is isolated and gets its own handler instance.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates a new mediator backed by the given service provider.
    /// </summary>
    /// <param name="serviceProvider">The DI container used to resolve command/query handlers.
    /// Must be registered before calling any dispatch method, or an exception is thrown.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Dispatches a command that has no response value.
    /// </summary>
    /// <typeparam name="TCommand">The concrete command type (must implement <see cref="ICommand"/>).</typeparam>
    /// <param name="command">The command instance to execute.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <remarks>
    /// Resolves the handler as the closed generic <c>ICommandHandler&lt;TCommand&gt;</c> from DI,
    /// then invokes its <c>HandleAsync</c>. Because <typeparamref name="TCommand"/> is a compile-time
    /// generic argument, this path uses a direct (non-reflective) handler lookup.
    /// </remarks>
    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Dispatches a command that returns a response value.
    /// </summary>
    /// <typeparam name="TResponse">The response type produced by the command.</typeparam>
    /// <param name="command">The command instance to execute.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>The command handler's response.</returns>
    /// <remarks>
    /// Builds the closed generic <c>ICommandHandler&lt;TCommand, TResponse&gt;</c> at runtime using
    /// <see cref="Type.MakeGenericType"/> because <typeparamref name="TCommand"/> is only known
    /// dynamically (the caller passes it as an <see cref="ICommand{TResponse}"/>). The handler is
    /// resolved from DI and invoked via <see langword="dynamic"/> to call the strongly-typed
    /// <c>HandleAsync</c> without compile-time knowledge of the concrete command type.
    /// </remarks>
    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.HandleAsync((dynamic)command, cancellationToken);
    }

    /// <summary>
    /// Dispatches a query and returns its result.
    /// </summary>
    /// <typeparam name="TResponse">The response type produced by the query.</typeparam>
    /// <param name="query">The query instance to execute.</param>
    /// <param name="cancellationToken">Optional token to cancel the operation.</param>
    /// <returns>The query handler's response.</returns>
    /// <remarks>
    /// Resolves the closed generic <c>IQueryHandler&lt;TQuery, TResponse&gt;</c> from DI at runtime
    /// (built with <see cref="Type.MakeGenericType"/>) and invokes its <c>HandleAsync</c> via
    /// <see langword="dynamic"/>. Mirrors <see cref="SendAsync{TResponse}"/> but for read operations.
    /// </remarks>
    public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);
        return await handler.HandleAsync((dynamic)query, cancellationToken);
    }
}