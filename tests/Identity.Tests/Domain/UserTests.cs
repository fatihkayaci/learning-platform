using FluentAssertions;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;

namespace Identity.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_WhenEmailIsEmpty_ThrowsBusinessException()
    {
        // Act
        Action act = () => User.Create("", "hashed_password", "John Doe", UserRole.Student);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Create_WhenFullNameIsEmpty_ThrowsBusinessException()
    {
        // Act
        Action act = () => User.Create("test@example.com", "hashed_password", "", UserRole.Student);

        // Assert
        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Create_WhenValidData_ReturnsUser()
    {
        // Act
        User user = User.Create("TEST@EXAMPLE.COM", "hashed_password", "John Doe", UserRole.Student);

        // Assert
        user.Email.Should().Be("test@example.com");
        user.FullName.Should().Be("John Doe");
        user.Role.Should().Be(UserRole.Student);
    }
}
