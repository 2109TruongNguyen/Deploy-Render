namespace NZWalksAPI.Models.Dto.Response;

public class AuthenResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}