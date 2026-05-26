using Identity.Application.Common.Interfaces;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using MediatR;

namespace Identity.Application.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new BusinessException("Email or Password incorrect");

        string accessToken = _tokenService.GenerateAccessToken(user);
        string refreshTokenString = _tokenService.GenerateRefreshToken();

        RefreshToken refreshToken = RefreshToken.Create(user.Id, refreshTokenString, 7);
        await _userRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new LoginResponseDto(accessToken, refreshTokenString);
    }
}