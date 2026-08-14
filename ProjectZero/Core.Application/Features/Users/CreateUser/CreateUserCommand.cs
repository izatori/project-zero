using MediatR;

namespace Core.Application.Features.Users.CreateUser;

/// <summary>
/// Command to create a new user.
/// Commands represent write operations in CQRS.
/// </summary>
public record CreateUserCommand(string Name, string Email) : IRequest<Guid>;