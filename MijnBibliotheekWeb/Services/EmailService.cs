using Microsoft.AspNetCore.Identity.UI.Services;

namespace MijnBibliotheekWeb.Services;

// Dummy e-mail service voor schoolproject
// Schrijft de e-mail naar de output/console in plaats van echt te versturen
public class EmailService : IEmailSender
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Log de email naar de console/logger zodat we de link kunnen zien
        _logger.LogInformation("================ EMAIL VERZONDEN ================");
        _logger.LogInformation($"Aan: {email}");
        _logger.LogInformation($"Onderwerp: {subject}");
        _logger.LogInformation($"Inhoud: {htmlMessage}");
        _logger.LogInformation("=================================================");

        return Task.CompletedTask;
    }
}
