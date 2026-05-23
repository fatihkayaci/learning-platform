using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public UserRole Role { get; private set; }

    private User() { }

    public static User Create(string email, string passwordHash, string fullName, UserRole role)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email boş olamaz", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash boş olamaz", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("İsim boş olamaz", nameof(fullName));
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException("Geçersiz rol", nameof(role));

        return new User
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FullName = fullName,
            Role = role
        };
    }
}