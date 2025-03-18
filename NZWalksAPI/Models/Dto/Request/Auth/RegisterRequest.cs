using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NZWalksAPI.Models.Dto.Request.Auth;

public class RegisterRequest
{
    [Required]
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("role")]
    public string[] Role { get; set; } = ["User"];
}