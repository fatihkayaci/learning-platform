using Identity.Application.DTOs;
using Identity.Domain.Enums;
using MediatR;

namespace Identity.Application.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password, string FullName, UserRole Role) : IRequest<UserDto>;