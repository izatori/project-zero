namespace Core.Application.DTOs;

/// <summary>
/// Data Transfer Object for User.
/// Used to transfer user data between layers.
/// </summary>
public record UserDto(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsActive
);