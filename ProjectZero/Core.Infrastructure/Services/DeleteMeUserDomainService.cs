using Core.Domain.Entities;
using Core.Domain.Repositories;
using Core.Domain.Services;

namespace Core.Infrastructure.Services;

/// <summary>
/// Implementation of domain service for user business operations.
/// 
/// Note: This implements the domain service interface defined in Core.Domain.
/// Implementations live in Core.Infrastructure because they may need infrastructure
/// (like database access), but the logic itself is still domain logic.
/// </summary>
public class DeleteMeUserDomainService : IDeleteMeUserDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly IDeleteMeEmailDomainService _deleteMeEmailDomainService;

    public DeleteMeUserDomainService(IUserRepository userRepository, IDeleteMeEmailDomainService deleteMeEmailDomainService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _deleteMeEmailDomainService = deleteMeEmailDomainService ?? throw new ArgumentNullException(nameof(deleteMeEmailDomainService));
    }

    public async Task<bool> EmailIsUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        // Validate email first using domain service
        if (!_deleteMeEmailDomainService.IsValidEmail(email))
            return false;

        // Then check if it exists in repository
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return existingUser == null;
    }

    public bool CanUserPerformAction(User user, string action)
    {
        // Business rule: only active users can perform actions
        if (!user.IsActive)
            return false;

        // Add more business rules as needed
        return action switch
        {
            "view-profile" => true,
            "edit-profile" => true,
            "delete-account" => user.CreatedAt < DateTime.UtcNow.AddDays(-30), // 30-day rule
            _ => false
        };
    }

    public int CalculateActivityScore(User user)
    {
        int score = 0;

        // Score based on account age
        var accountAge = DateTime.UtcNow - user.CreatedAt;
        score += (int)(accountAge.TotalDays / 10);

        // Score bonus if profile is complete
        if (!string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.Email))
            score += 10;

        // Score based on active status
        if (user.IsActive)
            score += 20;

        return Math.Min(score, 100); // Cap at 100
    }
}