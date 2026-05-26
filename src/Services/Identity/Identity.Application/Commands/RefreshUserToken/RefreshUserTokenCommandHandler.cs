using Identity.Application.Common.Interfaces;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using MediatR;
namespace Identity.Application.Commands.RefreshUserToken;

public class RefreshUserTokenCommandHandler : IRequestHandler<RefreshUserTokenCommand, TokenResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    public RefreshUserTokenCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<TokenResponseDto> Handle(RefreshUserTokenCommand request, CancellationToken cancellationToken)
    {
        RefreshToken? token = await _userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (token == null)
            throw new NotFoundException("Refresh token not found");

        if (!token.IsActive)
            throw new BusinessException("Token is invalid or expired");

        User? user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException("User not found");

        string accessToken = _tokenService.GenerateAccessToken(user);
        
        return new TokenResponseDto(accessToken);

    }
}