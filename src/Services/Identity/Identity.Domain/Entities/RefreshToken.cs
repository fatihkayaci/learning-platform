using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    public DateTime ExpiresOn { get; private set; }
    public DateTime? RevokedOn { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? ReasonRevoked { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsRevoked => RevokedOn.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, int expirationDays)
    {
       if (userId == Guid.Empty)
            throw new BusinessException("Id cannot be empty");
       if (string.IsNullOrWhiteSpace(token))
            throw new BusinessException("Token cannot be empty");
       if (expirationDays <= 0)
            throw new BusinessException("Expiration days must be greater than zero");

        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresOn = DateTime.UtcNow.AddDays(expirationDays)
        };
    }

    public void Revoke(string reason, string? replacedByToken = null)
    {
        RevokedOn = DateTime.UtcNow;
        ReasonRevoked = reason;
        ReplacedByToken = replacedByToken;
    }
}