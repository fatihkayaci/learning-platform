using FluentAssertions;
using Identity.Application.Commands.LoginUser;
using Identity.Application.Common.Interfaces;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using NSubstitute;

namespace Identity.Tests.Commands;

public class LoginUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _handler = new LoginUserCommandHandler(_userRepository, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsBusinessException()
    {
        // Arrange
        LoginUserCommand command = new("test@example.com", "Password123!");
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((Identity.Domain.Entities.User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Email or Password incorrect");
    }

    [Fact]
    public async Task Handle_WhenPasswordIsWrong_ThrowsBusinessException()
    {
        // Arrange
        LoginUserCommand command = new("test@example.com", "WrongPassword!");
        User user = User.Create("test@example.com", "hashed_password", "John Doe", UserRole.Student);
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Email or Password incorrect");
    }

    [Fact]
    public async Task Handle_WhenValidCredentials_ReturnsLoginResponseDto()
    {
        // Arrange
        LoginUserCommand command = new("test@example.com", "Password123!");
        User user = User.Create("test@example.com", "hashed_password", "John Doe", UserRole.Student);
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);
        _tokenService.GenerateAccessToken(user).Returns("access_token");
        _tokenService.GenerateRefreshToken().Returns("refresh_token");

        // Act
        LoginResponseDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");
        await _userRepository.Received(1).AddRefreshTokenAsync(Arg.Any<RefreshToken>(), Arg.Any<CancellationToken>());
    }
}
