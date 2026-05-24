using Identity.Application.Common.Interfaces;

namespace Identity.Infrastructure.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    // Work factor 12 = ~300ms per hash, recommended as of 2026
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash) => 
        BCrypt.Net.BCrypt.Verify(password, hash);
}