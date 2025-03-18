using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Services.Feature;

public class JwtService : IJwtService
{
    
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;

    public JwtService(IConfiguration config, UserManager<User> userManager)
    {
        _config = config;
        _userManager = userManager;
    }
    
    public string CreateAccessToken(User user)
    {
        var claims = new List<Claim>();
        var roles = _userManager.GetRolesAsync(user);
        
        claims.Add(new Claim(ClaimTypes.Name, user.UserName.ToString()));
        
        foreach (var role in roles.Result)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:AccessSecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_config["Jwt:AccessExpiration"])),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken(User user)
    {
        var claims = new List<Claim>();
        
        claims.Add(new Claim(ClaimTypes.Name, user.UserName.ToString()));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:RefreshSecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_config["Jwt:RefreshExpiration"])),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string? ValidateRefreshToken(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:RefreshSecretKey"]);

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true
            }, out _);

            return claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        }
        catch
        {
            return null;
        }
    }
}