using System.Security.Claims;
using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Services.Base;

public interface IJwtService
{
    string CreateAccessToken(User? user);
    
    string CreateRefreshToken(User? user);
    string? ValidateRefreshToken(string refreshToken);
}