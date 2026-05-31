using BuildingBlocks.Messaging.Events.Identity;
using Identity.Application.Common.Interfaces;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using MediatR;

namespace Identity.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEventPublisher _eventPublisher;

    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IEventPublisher eventPublisher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _eventPublisher = eventPublisher;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            throw new BusinessException("Email is already registered");

        string passwordHash = _passwordHasher.Hash(request.Password);

        User user = User.Create(request.Email, passwordHash, request.FullName, request.Role);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        UserRegisteredEvent @event = new(user.Id, user.Email, user.Role.ToString(), DateTime.UtcNow);
        await _eventPublisher.PublishAsync(@event, cancellationToken);

        return new UserDto(user.Id, user.Email, user.FullName, user.Role);
    }
}
