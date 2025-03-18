using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Models.Dto.Request;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Controllers;

[Route("api/auth")]
[ApiController]
[Authorize]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest emailRequest)
    {
        await _emailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, "");
        
        return Ok("Success");
    }
}
