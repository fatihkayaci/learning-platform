using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

public class Review : BaseEntity
{
    public Guid CourseId { get; private set; }
    public Guid StudentId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; } = null!;

    
    public Review(){}

    public static Review Create(Guid courseId, Guid studentId, int rating, string comment)
    {
        if (courseId == Guid.Empty)
            throw new BusinessException("Course cannot be empty");
        if (studentId == Guid.Empty)
            throw new BusinessException("Student cannot be empty");
        if (rating < 1 || rating > 5)
            throw new BusinessException("Rating must be between 1 and 5");
        if (string.IsNullOrWhiteSpace(comment))
            throw new BusinessException("Comment cannot be empty");
            
        return new Review
        {
            CourseId = courseId,
            StudentId = studentId,
            Rating = rating,
            Comment = comment
        }; 
    }
}