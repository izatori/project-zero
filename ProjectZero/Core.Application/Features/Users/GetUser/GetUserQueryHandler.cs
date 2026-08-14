using Core.Application.DTOs;
using Core.Domain.Repositories;
using MediatR;

namespace Core.Application.Features.Users.GetUser;

/// <summary>
/// Handler for GetUserQuery.
/// Retrieves and maps user data to DTO.
/// </summary>
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Handles the retrieval of a user.
    /// </summary>
    public async Task<UserDto?> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
            return null;

        // Map domain entity to DTO
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt,
            user.IsActive
        );
    }
}