using System.Threading.RateLimiting;

namespace EnadeAds.Security;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    /// <summary>Envios de formulários de conta por IP a cada minuto.</summary>
    public int AccountPostsPerMinute { get; set; } = 10;
}

/// <summary>
/// Limita, por IP, os POSTs dos formulários de conta que são alvo de força bruta e de cadastro em massa.
/// O resto da aplicação não tem limite.
/// </summary>
public static class AccountRateLimit
{
    public const string RejectedMessage = "Muitas tentativas em pouco tempo. Aguarde um minuto e tente de novo.";

    private static readonly string[] ProtectedPaths =
    [
        "/Account/Login",
        "/Account/Register",
        "/Account/ForgotPassword",
        "/Account/ResendEmailConfirmation",
        "/Account/ResetPassword",
        "/Account/LoginWith2fa",
        "/Account/LoginWithRecoveryCode",
    ];

    public static bool IsProtected(HttpContext context) =>
        HttpMethods.IsPost(context.Request.Method)
        && ProtectedPaths.Any(path => context.Request.Path.Equals(path, StringComparison.OrdinalIgnoreCase));

    public static PartitionedRateLimiter<HttpContext> Create(RateLimitOptions options) =>
        PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            if (!IsProtected(context))
            {
                return RateLimitPartition.GetNoLimiter("livre");
            }

            // Atrás do Nginx o IP real vem do X-Forwarded-For (UseForwardedHeaders já aplicou antes daqui).
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";
            return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = options.AccountPostsPerMinute,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            });
        });

    public static IServiceCollection AddAccountRateLimit(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>() ?? new RateLimitOptions();

        services.AddRateLimiter(limiter =>
        {
            limiter.GlobalLimiter = Create(options);
            limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            limiter.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.Headers.RetryAfter = "60";
                context.HttpContext.Response.ContentType = "text/plain; charset=utf-8";
                await context.HttpContext.Response.WriteAsync(RejectedMessage, cancellationToken);
            };
        });

        return services;
    }
}
