using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string VideoUrl{ get; private set; } = null!;
    public int Order{ get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;

    public Lesson(){}

    public static Lesson Create(string title, string videoUrl, int order, Guid courseId)
    {
        
       if (string.IsNullOrWhiteSpace(title))
            throw new BusinessException("Lesson title cannot be empty");
       if (string.IsNullOrWhiteSpace(videoUrl))
            throw new BusinessException("VideoUrl cannot be empty");
       if (order <= 0)
            throw new BusinessException("Order must be greater than zero");
       if (courseId == Guid.Empty)
            throw new BusinessException("Course cannot be empty");
            
        return new Lesson
        {
            Title = title,
            VideoUrl = videoUrl,
            Order = order,
            CourseId = courseId
        }; 
    }
}