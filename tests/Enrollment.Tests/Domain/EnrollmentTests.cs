using Enrollment.Domain.Exceptions;
using FluentAssertions;

namespace Enrollment.Tests.Domain;

public class EnrollmentTests
{
    [Fact]
    public void Create_WhenStudentIdIsEmpty_ThrowsBusinessException()
    {
        // Act
        Action act = () => Enrollment.Domain.Entities.Enrollment.Create(Guid.Empty, Guid.NewGuid(), 5);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Create_WhenCourseIdIsEmpty_ThrowsBusinessException()
    {
        // Act
        Action act = () => Enrollment.Domain.Entities.Enrollment.Create(Guid.NewGuid(), Guid.Empty, 5);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Create_WhenValidData_ReturnsEnrollment()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        // Act
        Enrollment.Domain.Entities.Enrollment enrollment = Enrollment.Domain.Entities.Enrollment.Create(studentId, courseId, 10);

        // Assert
        enrollment.StudentId.Should().Be(studentId);
        enrollment.CourseId.Should().Be(courseId);
        enrollment.TotalLessonCount.Should().Be(10);
    }
}
