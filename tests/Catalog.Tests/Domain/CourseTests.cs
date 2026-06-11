using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using FluentAssertions;

namespace Catalog.Tests.Domain;

public class CourseTests
{
    [Fact]
    public void Create_WhenNameIsEmpty_ThrowsBusinessException()
    {
        // Act
        Action act = () => Course.Create("", Guid.NewGuid(), Guid.NewGuid());

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Create_WhenInstructorIdIsEmpty_ThrowsBusinessException()
    {
        // Act
        Action act = () => Course.Create("Clean Architecture", Guid.Empty, Guid.NewGuid());

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Create_WhenValidData_ReturnsCourse()
    {
        // Arrange
        Guid instructorId = Guid.NewGuid();
        Guid categoryId = Guid.NewGuid();

        // Act
        Course course = Course.Create("Clean Architecture", instructorId, categoryId);

        // Assert
        course.Name.Should().Be("Clean Architecture");
        course.InstructorId.Should().Be(instructorId);
        course.CategoryId.Should().Be(categoryId);
    }
}
