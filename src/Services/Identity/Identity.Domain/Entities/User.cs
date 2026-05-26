using Identity.Domain.Enums;
using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public ICollection<RefreshToken> RefreshTokens { get; private set; }
    private User()
    {
        RefreshTokens = new List<RefreshToken>();
    }

    public static User Create(string email, string passwordHash, string fullName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessException("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new BusinessException("Password hash cannot be empty");
        if (string.IsNullOrWhiteSpace(fullName))
            throw new BusinessException("Full name cannot be empty");
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new BusinessException("Invalid role");

        return new User
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FullName = fullName,
            Role = role
        };
    }
    public void AddRefreshToken(RefreshToken token)
    {
        RefreshTokens.Add(token);
    }
}