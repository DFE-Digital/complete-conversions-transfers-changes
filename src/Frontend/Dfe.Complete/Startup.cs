using Azure.Identity;
using Dfe.Complete.Application.Common.Mappers;
using Dfe.Complete.Authorization;
using Dfe.Complete.Configuration;
using DataProtectionOptions = Dfe.Complete.Configuration.DataProtectionOptions;
using Dfe.Complete.Security;
using Dfe.Complete.StartupConfiguration;
using DfE.CoreLibs.Security.Authorization;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Dfe.Complete.Infrastructure;
using Dfe.Complete.Infrastructure.Security.Authorization;
using Dfe.Complete.Services;

namespace Dfe.Complete;

public class Startup
{
    private readonly TimeSpan _authenticationExpiration;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        _authenticationExpiration = TimeSpan.FromMinutes(int.Parse(Configuration["AuthenticationExpirationInMinutes"] ?? "60"));
    }

    private IConfiguration Configuration { get; }

    private IConfigurationSection GetConfigurationSectionFor<T>()
    {
        string sectionName = typeof(T).Name.Replace("Options", string.Empty);
        return Configuration.GetRequiredSection(sectionName);
    }

    private T GetTypedConfigurationFor<T>()
    {
        return GetConfigurationSectionFor<T>().Get<T>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddFeatureManagement();
        services.AddHealthChecks();
        services
           .AddRazorPages(options =>
           {
               options.Conventions.AuthorizeFolder("/");
               options.Conventions.AddPageRoute("/Projects/EditProjectNote", "projects/{projectId}/notes/edit");
           })
           .AddViewOptions(options =>
           {
               options.HtmlHelperOptions.ClientValidationEnabled = false;
           });

        services.AddControllersWithViews()
           .AddMicrosoftIdentityUI();
        SetupDataProtection(services);

        services.AddCompleteClientProject(Configuration);

        services.AddScoped<ErrorService>();

        services.AddScoped(sp => sp.GetService<IHttpContextAccessor>()?.HttpContext?.Session);
        services.AddSession(options =>
        {
            options.IdleTimeout = _authenticationExpiration;
            options.Cookie.Name = ".Complete.Session";
            options.Cookie.IsEssential = true;
        });
        services.AddHttpContextAccessor();

        services.AddApplicationAuthorization(Configuration);

        services.AddMicrosoftIdentityWebAppAuthentication(Configuration);
        ConfigureCookies(services);

        services.AddApplicationInsightsTelemetry();

        services.AddSingleton<IAuthorizationHandler, HeaderRequirementHandler>();

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        RegisterClients(services);

        services.AddGovUkFrontend();

        // New API client

        services.AddApplicationDependencyGroup(Configuration);
        services.AddInfrastructureDependencyGroup(Configuration);

        services.AddCustomClaimProvider<CustomDatabaseClaimsProvider>();

        // AutoMapper
        services.AddAutoMapper(typeof(AutoMapping));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Errors");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSecurityHeaders(
           SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment())
              .AddXssProtectionDisabled()
        );

        app.UseStatusCodePagesWithReExecute("/Errors", "?statusCode={0}");

        app.UseHttpsRedirection();
        app.UseHealthChecks("/health");

        //For Azure AD redirect uri to remain https
        ForwardedHeadersOptions forwardOptions = new() { ForwardedHeaders = ForwardedHeaders.All, RequireHeaderSymmetry = false };
        forwardOptions.KnownNetworks.Clear();
        forwardOptions.KnownProxies.Clear();
        app.UseForwardedHeaders(forwardOptions);

        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }

    private void ConfigureCookies(IServiceCollection services)
    {
        services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
           options =>
           {
               options.AccessDeniedPath = "/access-denied";
               options.Cookie.Name = ".Complete.Login";
               options.Cookie.HttpOnly = true;
               options.Cookie.IsEssential = true;
               options.ExpireTimeSpan = _authenticationExpiration;
               options.SlidingExpiration = true;
               if (string.IsNullOrEmpty(Configuration["CI"]))
               {
                   options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
               }
           });
    }

    private void RegisterClients(IServiceCollection services)
    {
        services.AddHttpClient("CompleteClient", (_, client) =>
        {
            CompleteOptions completeOptions = GetTypedConfigurationFor<CompleteOptions>();
            client.BaseAddress = new Uri(completeOptions.ApiEndpoint);
            client.DefaultRequestHeaders.Add("ApiKey", completeOptions.ApiKey);
        });

        services.AddHttpClient("AcademiesApiClient", (sp, client) =>
        {
            AcademiesOptions academiesApiOptions = GetTypedConfigurationFor<AcademiesOptions>();
            client.BaseAddress = new Uri(academiesApiOptions.ApiEndpoint);
            client.DefaultRequestHeaders.Add("ApiKey", academiesApiOptions.ApiKey);
        });
    }

    private void SetupDataProtection(IServiceCollection services)
    {
        var dp = services.AddDataProtection();
        var dpTargetPath = @"/srv/app/storage";
        DataProtectionOptions dpOptions = GetTypedConfigurationFor<DataProtectionOptions>();

        if (Directory.Exists(dpTargetPath)) {
            dp.PersistKeysToFileSystem(new DirectoryInfo(dpTargetPath));

            // If a Key Vault Key URI is defined, expect to encrypt the keys.xml
            string? kvProtectionKeyUri = dpOptions.KeyVaultKey;

            if (!string.IsNullOrWhiteSpace(kvProtectionKeyUri))
            {
                dp.ProtectKeysWithAzureKeyVault(
                    new Uri(kvProtectionKeyUri),
                    new DefaultAzureCredential()
                );
            }
        }
    }
}
