namespace EnadeAds.Security;

/// <summary>Cabeçalhos de segurança em todas as respostas.</summary>
public static class SecurityHeaders
{
    /// <summary>
    /// CSP da aplicação. Scripts só do próprio site; <c>'unsafe-inline'</c> continua necessário porque o Blazor
    /// gera um import map inline sem nonce, e <c>'wasm-unsafe-eval'</c> porque os componentes WebAssembly rodam no navegador.
    /// A defesa principal contra XSS é o escape de todo texto (StatementFormatter e Blazor), coberto por testes.
    /// </summary>
    public const string ContentSecurityPolicy =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'wasm-unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' https: data:; " +
        "font-src 'self'; " +
        "connect-src 'self'; " +
        "object-src 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'none'";

    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;
                headers.ContentSecurityPolicy = ContentSecurityPolicy;
                headers.XContentTypeOptions = "nosniff";
                headers.XFrameOptions = "DENY";
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
                return Task.CompletedTask;
            });

            await next();
        });
}
