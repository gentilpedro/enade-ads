using System.Net;
using System.Net.Mail;
using EnadeAds.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EnadeAds.Accounts;

public sealed class SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender<ApplicationUser>
{
    private readonly SmtpOptions smtp = options.Value;

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendAsync(email, "Confirme seu cadastro no Rumo",
            $"<p>Olá, {WebUtility.HtmlEncode(user.FullName)}!</p>" +
            $"<p>Para ativar sua conta, <a href='{confirmationLink}'>clique aqui</a>.</p>" +
            "<p>Se você não se cadastrou, ignore este e-mail.</p>");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendAsync(email, "Redefinição de senha do Rumo",
            $"<p>Para criar uma nova senha, <a href='{resetLink}'>clique aqui</a>.</p>" +
            "<p>Se você não pediu a redefinição, ignore este e-mail.</p>");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendAsync(email, "Código de redefinição de senha do Rumo",
            $"<p>Seu código de redefinição de senha é: <strong>{WebUtility.HtmlEncode(resetCode)}</strong></p>");

    private async Task SendAsync(string to, string subject, string htmlBody)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(smtp.FromAddress, smtp.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };
        message.To.Add(to);

        using var client = new SmtpClient(smtp.Host, smtp.Port)
        {
            EnableSsl = smtp.EnableSsl,
            Credentials = string.IsNullOrEmpty(smtp.UserName) ? null : new NetworkCredential(smtp.UserName, smtp.Password),
        };

        await client.SendMailAsync(message);
        logger.LogInformation("E-mail '{Subject}' enviado", subject);
    }
}
