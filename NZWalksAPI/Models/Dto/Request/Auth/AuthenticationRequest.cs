using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NZWalksAPI.Models.Dto.Request.Auth;

public class AuthenticationRequest
{
    [Required]
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("Password")]
    public string Password { get; set; } = string.Empty;
}