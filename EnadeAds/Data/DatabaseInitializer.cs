using EnadeAds.Accounts;
using EnadeAds.Questions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // As migrations são do PostgreSQL; outros provedores (como o SQLite dos testes) criam o schema direto.
        if (db.Database.IsNpgsql())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }

        // Questões das provas do Inep (Data/Seed/inep): só entram as que ainda não existem.
        await scope.ServiceProvider.GetRequiredService<InepQuestionImporter>().ImportFolderAsync(InepQuestionImporter.DefaultFolder);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
