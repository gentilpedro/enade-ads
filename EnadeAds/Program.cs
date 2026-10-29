using EnadeAds.Accounts;
using EnadeAds.Components;
using EnadeAds.Components.Account;
using EnadeAds.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando EnadeAds");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddInteractiveWebAssemblyComponents()
        .AddAuthenticationStateSerialization();

    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

    // Revalida a sessão a cada minuto: mudança de perfil (aprovar ou revogar professor) vale rápido.
    builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(1));
    builder.Services.AddScoped<TeacherApprovals>();

    builder.Services.Configure<AdminOptions>(builder.Configuration.GetSection(AdminOptions.SectionName));
    builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));

    var smtp = builder.Configuration.GetSection(SmtpOptions.SectionName).Get<SmtpOptions>() ?? new SmtpOptions();
    if (smtp.IsConfigured)
    {
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, SmtpEmailSender>();
    }
    else if (builder.Environment.IsDevelopment())
    {
        // Sem SMTP em desenvolvimento, o link de confirmação aparece na própria tela.
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
    }
    else
    {
        // Sem SMTP fora de desenvolvimento, o link na tela permitiria confirmar qualquer e-mail .edu.
        throw new InvalidOperationException("Configure a seção 'Smtp' (Host e FromAddress) para enviar os e-mails de confirmação.");
    }

    var app = builder.Build();

    await DatabaseInitializer.InitializeAsync(app.Services);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();
    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(EnadeAds.Client._Imports).Assembly);

    // Add additional endpoints required by the Identity /Account Razor components.
    app.MapAdditionalIdentityEndpoints();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "EnadeAds encerrado de forma inesperada");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
