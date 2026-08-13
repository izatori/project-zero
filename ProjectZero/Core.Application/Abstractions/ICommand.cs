namespace Core.Application.Abstractions;

/// <summary>
/// Marker interface for command objects.
/// Commands represent requests to perform an action that modifies domain state.
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Marker interface for command objects that return a result.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the command</typeparam>
public interface ICommand<out TResponse> : ICommand
{
}