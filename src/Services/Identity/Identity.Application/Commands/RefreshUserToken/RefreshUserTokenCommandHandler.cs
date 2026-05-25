using Identity.Application.Common.Interfaces;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
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
            throw new Exception("Refresh token bulunamadı");

        if (!token.IsActive)
            throw new Exception("Token geçersiz veya süresi dolmuş");

        User? user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
        if (user == null)
            throw new Exception("Kullanıcı bulunamadı");

        string accessToken = _tokenService.GenerateAccessToken(user);
        
        return new TokenResponseDto(accessToken);

    }
}