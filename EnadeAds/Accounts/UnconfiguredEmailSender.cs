using EnadeAds.Data;
using Microsoft.AspNetCore.Identity;

namespace EnadeAds.Accounts;

/// <summary>
/// Produção ainda sem SMTP: o site funciona, mas nenhum e-mail sai. O link de confirmação não aparece na
/// tela (deixaria confirmar qualquer e-mail .edu), então as contas novas esperam o SMTP ser configurado.
/// </summary>
public sealed class UnconfiguredEmailSender(ILogger<UnconfiguredEmailSender> logger) : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) => Skip("confirmação de cadastro");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) => Skip("redefinição de senha");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) => Skip("código de redefinição de senha");

    private Task Skip(string kind)
    {
        logger.LogWarning("E-mail de {Kind} não enviado: a seção 'Smtp' (Host e FromAddress) não está configurada.", kind);
        return Task.CompletedTask;
    }
}
