using Catalog.Application.Commands.CreateCourse;
using FluentAssertions;
using FluentValidation.Results;

namespace Catalog.Tests.Commands;

public class CreateCourseCommandValidatorTests
{
    [Fact]
    public void Validate_WhenNameIsEmpty_FailsValidation()
    {
        // Arrange
        CreateCourseCommandValidator validator = new();
        CreateCourseCommand command = new("", null, Guid.NewGuid());

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenCategoryIdIsEmpty_FailsValidation()
    {
        // Arrange
        CreateCourseCommandValidator validator = new();
        CreateCourseCommand command = new("Clean Architecture", null, Guid.Empty);

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_PassesValidation()
    {
        // Arrange
        CreateCourseCommandValidator validator = new();
        CreateCourseCommand command = new("Clean Architecture", null, Guid.NewGuid());

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
