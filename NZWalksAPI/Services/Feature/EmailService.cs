using System.Security.Claims;
using MailKit.Net.Smtp;
using MimeKit;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Services.Feature;
 
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly TemplateService _templateService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmailService(IConfiguration configuration, TemplateService templateService, IHttpContextAccessor _httpContextAccessor)
    {
        _configuration = configuration;
        _templateService = templateService;
        this._httpContextAccessor = _httpContextAccessor;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User";
        var emailSettings = _configuration.GetSection("EmailSettings");

        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ThankYouTemplate.html");
        string emailBody = await _templateService.GetThankYouTemplateAsync(templatePath, userName);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = emailBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}