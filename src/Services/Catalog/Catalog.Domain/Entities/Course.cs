namespace Catalog.Domain.Entities;

public class Course : BaseEntity
{
    public string Name { get; private set; } = null!;
    public Guid InstructorId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public List<Lesson> Lessons { get; private set; } = [];

    public Course(){}

    public static Course Create(string name, Guid instructorId, Guid categoryId, string? description = null)
    {
        return new Course
        {
            Name = name,
            InstructorId = instructorId,
            CategoryId = categoryId,
            Description = description ?? string.Empty
        };
    }

    public void Update(string name, string? description, Guid categoryId)
    {
        Name = name ?? Name;
        CategoryId = categoryId;
        Description = description ?? Description;
    }
}