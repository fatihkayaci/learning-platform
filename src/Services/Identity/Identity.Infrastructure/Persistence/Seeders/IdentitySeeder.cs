using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Seeders;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IdentityDbContext context)
    {
        bool hasUsers = await context.Users.AnyAsync();
        if (hasUsers)
            return;

        string[] emails =
        [
            "student1@example.com",
            "student2@example.com",
            "instructor1@example.com",
            "instructor2@example.com"
        ];

        string passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!", workFactor: 12);

        User[] users =
        [
            User.Create(emails[0], passwordHash, "Alice Johnson", UserRole.Student),
            User.Create(emails[1], passwordHash, "Bob Smith", UserRole.Student),
            User.Create(emails[2], passwordHash, "Carol White", UserRole.Instructor),
            User.Create(emails[3], passwordHash, "David Brown", UserRole.Instructor)
        ];

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}
