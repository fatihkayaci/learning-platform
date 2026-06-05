using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

public class Course : BaseEntity
{
    public string Name { get; private set; } = null!;
    public Guid InstructorId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public List<Lesson> Lessons { get; private set; } = [];
    public int EnrollmentCount { get; private set; }

    public Course(){}

    public static Course Create(string name, Guid instructorId, Guid categoryId, string? description = null)
    {
       if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException("Course Name cannot be empty");
       if (instructorId == Guid.Empty)
            throw new BusinessException("Instructor cannot be empty");
       if (categoryId == Guid.Empty)
            throw new BusinessException("Category cannot be empty");

        return new Course
        {
            Name = name,
            InstructorId = instructorId,
            CategoryId = categoryId,
            Description = description ?? string.Empty
        };
    }

    public void IncrementEnrollmentCount() => EnrollmentCount++;

    public void Update(string name, string? description, Guid categoryId)
    {
       if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException("Course Name cannot be empty");
       if (categoryId == Guid.Empty)
            throw new BusinessException("Category cannot be empty");

        Name = name;
        CategoryId = categoryId;
        Description = description ?? Description;
    }
}