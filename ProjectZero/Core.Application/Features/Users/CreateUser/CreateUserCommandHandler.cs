using Core.Application.Abstractions;
using Core.Domain.Abstractions;
using Core.Domain.Entities;
using Core.Domain.Repositories;

namespace Core.Application.Features.Users.CreateUser;

/// <summary>
/// Handler for CreateUserCommand.
/// Orchestrates domain logic and persistence.
/// </summary>
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the creation of a new user.
    /// </summary>
    public async Task<Guid> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Check if user with email already exists
        var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ValidationException("User with this email already exists", new()
            {
                { "Email", new[] { "A user with this email address already exists" } }
            });
        }

        // Create the domain entity (business logic is encapsulated in the entity)
        var user = User.Create(command.Name, command.Email);

        // Persist the user
        await _userRepository.AddAsync(user, cancellationToken);

        // Save changes (includes publishing domain events)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}