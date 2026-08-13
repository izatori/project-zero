namespace Core.Application.Abstractions;

/// <summary>
/// Base class for application-level exceptions.
/// </summary>
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message) : base(message)
    {
    }

    protected ApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"Entity '{entityName}' with ID '{id}' was not found.")
    {
    }
}

/// <summary>
/// Exception thrown when a validation error occurs.
/// </summary>
public class ValidationException : ApplicationException
{
    public ValidationException(string message, Dictionary<string, string[]>? errors = null)
        : base(message)
    {
        Errors = errors ?? new();
    }

    public Dictionary<string, string[]> Errors { get; }
}