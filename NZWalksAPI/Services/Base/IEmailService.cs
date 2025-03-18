namespace NZWalksAPI.Services.Base;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}