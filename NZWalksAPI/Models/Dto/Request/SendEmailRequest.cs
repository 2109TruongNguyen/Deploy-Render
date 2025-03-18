namespace NZWalksAPI.Models.Dto.Request;

public class SendEmailRequest
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
}