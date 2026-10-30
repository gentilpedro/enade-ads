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

        // Questões das seeds (Data/Seed/inep e enem): só entram as que ainda não existem.
        var importer = scope.ServiceProvider.GetRequiredService<InepQuestionImporter>();
        foreach (var folder in InepQuestionImporter.DefaultFolders)
        {
            await importer.ImportFolderAsync(folder);
        }

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
