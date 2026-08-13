using System.Text.RegularExpressions;
using Core.Domain.Services;

namespace Core.Infrastructure.Services;

/// <summary>
/// Implementation of email domain service.
/// Contains email validation business logic.
/// </summary>
public class DeleteMeEmailDomainService : IDeleteMeEmailDomainService
{
    // Simple email validation pattern (in production, consider a more sophisticated approach)
    private static readonly Regex EmailRegex = new(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    // Business rule: blacklisted email domains
    private static readonly HashSet<string> BlacklistedDomains = new(StringComparer.OrdinalIgnoreCase)
    {
        "tempmail.com",
        "throwaway.email",
        "guerrillamail.com"
    };

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email.Length > 254) // RFC 5321
            return false;

        return EmailRegex.IsMatch(email);
    }

    public async Task<bool> IsEmailDomainAllowedAsync(string email, CancellationToken cancellationToken = default)
    {
        if (!IsValidEmail(email))
            return false;

        var domain = email.Split('@').Last();

        // Simulate async operation (in real scenario, might check against database)
        await Task.Delay(0, cancellationToken);

        return !BlacklistedDomains.Contains(domain);
    }
}