using Identity.Domain.Enums;

namespace Identity.Application.DTOs;

public record UserDto(Guid Id, string Email, string FullName, UserRole Role);