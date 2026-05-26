using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ICollection<Course> Courses { get; private set; } = new List<Course>();
    
    public Category() { }
    
    public static Category Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException("Category name cannot be empty");
            
        return new Category { Name = name };
    }

}