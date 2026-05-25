using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands.RefreshUserToken;

public record RefreshUserTokenCommand(string RefreshToken) : IRequest<TokenResponseDto>;