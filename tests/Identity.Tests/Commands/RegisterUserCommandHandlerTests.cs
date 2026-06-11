using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Identity;
using FluentAssertions;
using Identity.Application.Commands.RegisterUser;
using Identity.Application.Common.Interfaces;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using NSubstitute;

namespace Identity.Tests.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(_userRepository, _passwordHasher, _eventPublisher);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsBusinessException()
    {
        // Arrange
        RegisterUserCommand command = new("test@example.com", "Password123!", "John Doe", UserRole.Student);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Email is already registered");
    }

    [Fact]
    public async Task Handle_WhenValidData_CreatesUserAndPublishesEvent()
    {
        // Arrange
        RegisterUserCommand command = new("test@example.com", "Password123!", "John Doe", UserRole.Student);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(false);
        _passwordHasher.Hash(command.Password).Returns("hashed_password");

        // Act
        UserDto result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Email.Should().Be("test@example.com");
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<UserRegisteredEvent>(), Arg.Any<CancellationToken>());
    }
}
