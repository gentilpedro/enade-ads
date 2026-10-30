using EnadeAds.Data;
using Microsoft.EntityFrameworkCore;

namespace EnadeAds.Study;

/// <summary>
/// A trilha ativa de quem está navegando, guardada num cookie. O <see cref="ApplicationDbContext"/> filtra
/// roadmap, questões, desafios e simulados por ela. Fora de uma requisição (importação na subida, testes)
/// vale a trilha padrão.
/// </summary>
public sealed class TrackScope(IHttpContextAccessor http)
{
    public const string CookieName = "trilha";

    private int? id;

    public int Id => id ??= Tracks.IdOf(http.HttpContext?.Request.Cookies[CookieName]) ?? Tracks.Default;

    /// <summary>Troca a trilha só nesta requisição (testes e tarefas de fundo).</summary>
    public void Use(int trackId) => id = trackId;
}

/// <summary>
/// Trilha ativa com os dados do banco (nome e datas que o admin pode mudar).
/// Carrega as trilhas uma vez por requisição num contexto próprio: o cabeçalho e a página pedem a trilha ao mesmo
/// tempo, e um <see cref="ApplicationDbContext"/> não aceita duas consultas simultâneas.
/// </summary>
public sealed class TrackCatalog(IServiceScopeFactory scopes, TrackScope scope)
{
    private Task<IReadOnlyList<Track>>? all;

    public async Task<Track> CurrentAsync(CancellationToken cancellationToken = default)
    {
        var tracks = await ListAsync(cancellationToken);
        return tracks.FirstOrDefault(t => t.Id == scope.Id) ?? tracks.FirstOrDefault(t => t.Id == Tracks.Default)
            ?? Tracks.Seed.Single(t => t.Id == Tracks.Default);
    }

    public Task<IReadOnlyList<Track>> ListAsync(CancellationToken cancellationToken = default) => all ??= LoadAsync(cancellationToken);

    private async Task<IReadOnlyList<Track>> LoadAsync(CancellationToken cancellationToken)
    {
        using var own = scopes.CreateScope();
        var db = own.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.Tracks.AsNoTracking().OrderBy(t => t.Order).ToListAsync(cancellationToken);
    }
}

public static class TrackEndpoints
{
    /// <summary>
    /// <c>/trilha/{slug}?volta=/questoes</c> troca a trilha ativa e volta para a página. É um link comum (GET)
    /// porque só muda uma preferência de navegação, como trocar de idioma, e funciona sem JavaScript.
    /// </summary>
    public static void MapTrackSwitch(this IEndpointRouteBuilder app) =>
        app.MapGet("/trilha/{slug}", (string slug, string? volta, HttpContext context) =>
        {
            if (Tracks.IdOf(slug) is null)
            {
                return Results.NotFound();
            }

            context.Response.Cookies.Append(TrackScope.CookieName, slug, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = context.Request.IsHttps,
                MaxAge = TimeSpan.FromDays(365),
            });

            // Só volta para páginas do próprio site; o resto vai para o início.
            return Results.LocalRedirect(IsLocal(volta) ? volta! : "/");
        });

    private static bool IsLocal(string? url) =>
        !string.IsNullOrEmpty(url) && url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'));
}
