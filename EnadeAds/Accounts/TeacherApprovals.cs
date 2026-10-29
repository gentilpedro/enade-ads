using EnadeAds.Data;
using Microsoft.AspNetCore.Identity;

namespace EnadeAds.Accounts;

public sealed record PendingTeacher(string UserId, string FullName, string Email, bool EmailConfirmed);

public enum TeacherDecisionResult
{
    Done,
    UserNotFound,
    NotPending,
}

public sealed class TeacherApprovals(UserManager<ApplicationUser> users, ILogger<TeacherApprovals> logger)
{
    public async Task<IReadOnlyList<PendingTeacher>> ListPendingAsync()
    {
        var pending = await users.GetUsersInRoleAsync(Roles.PendingTeacher);

        return pending
            .OrderBy(user => user.FullName)
            .Select(user => new PendingTeacher(user.Id, user.FullName, user.Email ?? "", user.EmailConfirmed))
            .ToList();
    }

    public Task<TeacherDecisionResult> ApproveAsync(string userId) => DecideAsync(userId, approve: true);

    public Task<TeacherDecisionResult> RejectAsync(string userId) => DecideAsync(userId, approve: false);

    private async Task<TeacherDecisionResult> DecideAsync(string userId, bool approve)
    {
        var user = await users.FindByIdAsync(userId);
        if (user is null)
        {
            return TeacherDecisionResult.UserNotFound;
        }

        if (!await users.IsInRoleAsync(user, Roles.PendingTeacher))
        {
            return TeacherDecisionResult.NotPending;
        }

        Ensure(await users.RemoveFromRoleAsync(user, Roles.PendingTeacher));
        if (approve)
        {
            Ensure(await users.AddToRoleAsync(user, Roles.Teacher));
        }

        // Invalida a sessão atual para que o novo perfil (ou a perda dele) valha no próximo login.
        Ensure(await users.UpdateSecurityStampAsync(user));

        logger.LogInformation("Pedido de professor {UserId} {Decision}.", user.Id, approve ? "aprovado" : "recusado");
        return TeacherDecisionResult.Done;
    }

    private static void Ensure(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(error => error.Description)));
        }
    }
}
