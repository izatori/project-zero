using Core.Domain.Entities;

namespace Core.Domain.Services;

/// <summary>
/// Domain service for user-related business operations that don't belong to a single entity.
/// Domain services contain business logic that operates across multiple aggregates
/// or represents a business process.
/// 
/// Key characteristics:
/// - Stateless (no internal state)
/// - Contains business logic (not just data access)
/// - Interfaces only - implementations in Core.Infrastructure
/// - Only depends on domain objects (entities, value objects, other domain services)
/// - NO infrastructure or application layer dependencies
/// </summary>
public interface IDeleteMeUserDomainService
{
    /// <summary>
    /// Check if a user with the given email already exists.
    /// This is a domain operation (validates business rule) not just data access.
    /// </summary>
    Task<bool> EmailIsUniqueAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates whether a user can perform a specific action.
    /// Example of cross-aggregate business logic.
    /// </summary>
    bool CanUserPerformAction(User user, string action);

    /// <summary>
    /// Calculate user activity score based on user history.
    /// Complex business calculation that doesn't belong in entity.
    /// </summary>
    int CalculateActivityScore(User user);
}