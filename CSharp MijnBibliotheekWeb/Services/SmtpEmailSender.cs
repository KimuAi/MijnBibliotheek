using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace MijnBibliotheekWeb.Services;

/// E-mailsender die configuratie gebruikt (gebruik User Secrets / env voor wachtwoord).
public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _opts;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
    {
        _logger = logger;
        _opts = config.GetSection("Smtp").Get<SmtpOptions>() ?? new SmtpOptions();
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Nederlandse commentaar: e-mail wordt via SMTP verstuurd. Wachtwoord niet in repository opslaan.
        try
        {
            using var msg = new MailMessage();
            msg.From = new MailAddress(_opts.From ?? "no-reply@localhost");
            msg.To.Add(email);
            msg.Subject = subject;
            msg.Body = htmlMessage;
            msg.IsBodyHtml = true;

            using var client = new SmtpClient(_opts.Host, _opts.Port)
            {
                EnableSsl = _opts.UseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            if (!string.IsNullOrWhiteSpace(_opts.Username))
            {
                client.Credentials = new NetworkCredential(_opts.Username, _opts.Password ?? "");
            }

            // stuur asynchroon (SmtpClient heeft geen native SendMailAsync in alle runtimes; wrapper)
            await client.SendMailAsync(msg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "E-mail verzenden mislukt");
            throw;
        }
    }
}

/// Opties voor SMTP (configureer in appsettings of user-secrets)
public class SmtpOptions
{
    public string? Host { get; set; }
    public int Port { get; set; } = 25;
    public bool UseSsl { get; set; } = false;
    public string? Username { get; set; }
    public string? Password { get; set; } // bewaar in user-secrets of env
    public string? From { get; set; }
}