namespace Core.Domain.Services;

/// <summary>
/// Domain service for email validation logic.
/// This example shows a domain service that validates business rules.
/// </summary>
public interface IDeleteMeEmailDomainService
{
    /// <summary>
    /// Validate email format against business rules.
    /// </summary>
    bool IsValidEmail(string email);

    /// <summary>
    /// Check if an email domain is allowed (blacklist/whitelist business rule).
    /// </summary>
    Task<bool> IsEmailDomainAllowedAsync(string email, CancellationToken cancellationToken = default);
}