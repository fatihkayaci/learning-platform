using FluentAssertions;
using FluentValidation.Results;
using Identity.Application.Commands.LoginUser;

namespace Identity.Tests.Commands;

public class LoginUserCommandValidatorTests
{
    [Fact]
    public void Validate_WhenEmailIsEmpty_FailsValidation()
    {
        // Arrange
        LoginUserCommandValidator validator = new();
        LoginUserCommand command = new("", "Password123!");

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenPasswordIsEmpty_FailsValidation()
    {
        // Arrange
        LoginUserCommandValidator validator = new();
        LoginUserCommand command = new("test@example.com", "");

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_PassesValidation()
    {
        // Arrange
        LoginUserCommandValidator validator = new();
        LoginUserCommand command = new("test@example.com", "Password123!");

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
