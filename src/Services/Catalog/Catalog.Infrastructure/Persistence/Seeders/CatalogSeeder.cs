using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Seeders;

public static class CatalogSeeder
{
    private static readonly Guid Instructor1Id = new("a1b2c3d4-0001-0000-0000-000000000001");
    private static readonly Guid Instructor2Id = new("a1b2c3d4-0002-0000-0000-000000000002");

    public static async Task SeedAsync(CatalogDbContext context)
    {
        bool hasCategories = await context.Categories.AnyAsync();
        if (hasCategories)
            return;

        Category backendCategory = Category.Create("Backend Development");
        Category frontendCategory = Category.Create("Frontend Development");

        context.Entry(backendCategory).Property(c => c.Description).CurrentValue = "Server-side development courses";
        context.Entry(frontendCategory).Property(c => c.Description).CurrentValue = "Client-side development courses";

        await context.Categories.AddRangeAsync(backendCategory, frontendCategory);
        await context.SaveChangesAsync();

        Course course1 = Course.Create("ASP.NET Core Fundamentals", Instructor1Id, backendCategory.Id, "Learn ASP.NET Core from scratch");
        Course course2 = Course.Create("Entity Framework Core", Instructor1Id, backendCategory.Id, "Master EF Core ORM");
        Course course3 = Course.Create("React from Zero to Hero", Instructor2Id, frontendCategory.Id, "Complete React course");
        Course course4 = Course.Create("TypeScript Essentials", Instructor2Id, frontendCategory.Id, "TypeScript for modern development");

        await context.Courses.AddRangeAsync(course1, course2, course3, course4);
        await context.SaveChangesAsync();

        Lesson[] lessons =
        [
            Lesson.Create("Introduction to ASP.NET Core", "https://example.com/video1", 1, course1.Id),
            Lesson.Create("Middleware Pipeline", "https://example.com/video2", 2, course1.Id),
            Lesson.Create("Controllers and Routing", "https://example.com/video3", 3, course1.Id),

            Lesson.Create("DbContext and Migrations", "https://example.com/video4", 1, course2.Id),
            Lesson.Create("Querying with LINQ", "https://example.com/video5", 2, course2.Id),

            Lesson.Create("React Components", "https://example.com/video6", 1, course3.Id),
            Lesson.Create("State and Props", "https://example.com/video7", 2, course3.Id),
            Lesson.Create("Hooks in Depth", "https://example.com/video8", 3, course3.Id),

            Lesson.Create("TypeScript Types", "https://example.com/video9", 1, course4.Id),
            Lesson.Create("Interfaces and Generics", "https://example.com/video10", 2, course4.Id)
        ];

        await context.Lessons.AddRangeAsync(lessons);
        await context.SaveChangesAsync();
    }
}
