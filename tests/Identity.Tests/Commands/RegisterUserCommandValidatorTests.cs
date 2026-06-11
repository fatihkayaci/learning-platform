using FluentAssertions;
using FluentValidation.Results;
using Identity.Application.Commands.RegisterUser;
using Identity.Domain.Enums;

namespace Identity.Tests.Commands;

public class RegisterUserCommandValidatorTests
{
    [Fact]
    public void Validate_WhenEmailIsEmpty_FailsValidation()
    {
        // Arrange
        RegisterUserCommandValidator validator = new();
        RegisterUserCommand command = new("", "Password123!", "John Doe", UserRole.Student);

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_FailsValidation()
    {
        // Arrange
        RegisterUserCommandValidator validator = new();
        RegisterUserCommand command = new("not-an-email", "Password123!", "John Doe", UserRole.Student);

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenPasswordIsTooShort_FailsValidation()
    {
        // Arrange
        RegisterUserCommandValidator validator = new();
        RegisterUserCommand command = new("test@example.com", "123", "John Doe", UserRole.Student);

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenFullNameIsEmpty_FailsValidation()
    {
        // Arrange
        RegisterUserCommandValidator validator = new();
        RegisterUserCommand command = new("test@example.com", "Password123!", "", UserRole.Student);

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_PassesValidation()
    {
        // Arrange
        RegisterUserCommandValidator validator = new();
        RegisterUserCommand command = new("test@example.com", "Password123!", "John Doe", UserRole.Student);

        // Act
        ValidationResult result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
