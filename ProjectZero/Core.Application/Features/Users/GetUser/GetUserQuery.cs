using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Users.GetUser;

/// <summary>
/// Query to get a user by ID.
/// Queries represent read operations in CQRS.
/// </summary>
public record GetUserQuery(Guid UserId) : IRequest<UserDto?>;