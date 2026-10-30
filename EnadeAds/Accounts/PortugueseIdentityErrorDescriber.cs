using Microsoft.AspNetCore.Identity;

namespace EnadeAds.Accounts;

/// <summary>Mensagens de validação do ASP.NET Core Identity em português (senha, e-mail, perfis, tokens).</summary>
public sealed class PortugueseIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError() => Error(nameof(DefaultError), "Ocorreu um erro desconhecido.");

    public override IdentityError ConcurrencyFailure() =>
        Error(nameof(ConcurrencyFailure), "Os dados foram alterados por outra pessoa ao mesmo tempo. Recarregue a página e tente de novo.");

    public override IdentityError PasswordMismatch() => Error(nameof(PasswordMismatch), "Senha incorreta.");

    public override IdentityError InvalidToken() => Error(nameof(InvalidToken), "O link é inválido ou expirou. Peça um novo.");

    public override IdentityError RecoveryCodeRedemptionFailed() =>
        Error(nameof(RecoveryCodeRedemptionFailed), "Código de recuperação inválido.");

    public override IdentityError LoginAlreadyAssociated() =>
        Error(nameof(LoginAlreadyAssociated), "Já existe uma conta usando este login.");

    public override IdentityError InvalidUserName(string? userName) =>
        Error(nameof(InvalidUserName), $"O nome de usuário \"{userName}\" é inválido.");

    public override IdentityError InvalidEmail(string? email) => Error(nameof(InvalidEmail), $"O e-mail \"{email}\" é inválido.");

    // Nome de usuário é o próprio e-mail nesta plataforma: as duas mensagens falam de e-mail.
    public override IdentityError DuplicateUserName(string userName) =>
        Error(nameof(DuplicateUserName), $"Já existe uma conta com o e-mail \"{userName}\".");

    public override IdentityError DuplicateEmail(string email) =>
        Error(nameof(DuplicateEmail), $"Já existe uma conta com o e-mail \"{email}\".");

    public override IdentityError InvalidRoleName(string? role) => Error(nameof(InvalidRoleName), $"O perfil \"{role}\" é inválido.");

    public override IdentityError DuplicateRoleName(string role) => Error(nameof(DuplicateRoleName), $"O perfil \"{role}\" já existe.");

    public override IdentityError UserAlreadyHasPassword() => Error(nameof(UserAlreadyHasPassword), "A conta já tem uma senha.");

    public override IdentityError UserLockoutNotEnabled() =>
        Error(nameof(UserLockoutNotEnabled), "O bloqueio não está ativado para esta conta.");

    public override IdentityError UserAlreadyInRole(string role) => Error(nameof(UserAlreadyInRole), $"A conta já tem o perfil \"{role}\".");

    public override IdentityError UserNotInRole(string role) => Error(nameof(UserNotInRole), $"A conta não tem o perfil \"{role}\".");

    public override IdentityError PasswordTooShort(int length) =>
        Error(nameof(PasswordTooShort), $"A senha precisa ter pelo menos {length} caracteres.");

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
        Error(nameof(PasswordRequiresUniqueChars), $"A senha precisa ter pelo menos {uniqueChars} caracteres diferentes.");

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        Error(nameof(PasswordRequiresNonAlphanumeric), "A senha precisa ter pelo menos um símbolo (por exemplo ! @ # $).");

    public override IdentityError PasswordRequiresDigit() => Error(nameof(PasswordRequiresDigit), "A senha precisa ter pelo menos um número (0 a 9).");

    public override IdentityError PasswordRequiresLower() => Error(nameof(PasswordRequiresLower), "A senha precisa ter pelo menos uma letra minúscula.");

    public override IdentityError PasswordRequiresUpper() => Error(nameof(PasswordRequiresUpper), "A senha precisa ter pelo menos uma letra maiúscula.");

    private static IdentityError Error(string code, string description) => new() { Code = code, Description = description };
}
