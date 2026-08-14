using Core.Application.Abstractions;
using Core.Application.DTOs;

namespace Core.Application.Features.Users.GetUser;

/// <summary>
/// Query to get a user by ID.
/// Queries represent read operations in CQRS.
/// </summary>
public record GetUserQuery(Guid UserId) : IQuery<UserDto?>;