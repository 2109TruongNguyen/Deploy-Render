namespace NZWalksAPI.Models.Dto.Response;

public class AuthenticationResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}