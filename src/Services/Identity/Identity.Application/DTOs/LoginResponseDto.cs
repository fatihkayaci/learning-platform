namespace Identity.Application.DTOs;

public record LoginResponseDto(string AccessToken, string RefreshToken);
