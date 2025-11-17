using GovUK.Dfe.CoreLibs.Security.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Infrastructure.Security.Authorization
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

            services.AddApplicationAuthorization(configuration);

            return services;
        }

        /// <summary>
        /// Configures OpenIdConnect events for web authentication to handle user validation
        /// </summary>
        public static IServiceCollection AddWebAuthenticationWithUserValidation(this IServiceCollection services)
        {
            // Register the authentication user validation service
            services.AddScoped<IAuthenticationUserValidationService, AuthenticationUserValidationService>();

            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Events ??= new OpenIdConnectEvents();
                
                var originalOnTokenValidated = options.Events.OnTokenValidated;
                
                options.Events.OnTokenValidated = async context =>
                {
                    // Call the original event if it exists
                    if (originalOnTokenValidated != null)
                    {
                        await originalOnTokenValidated(context);
                    }

                    // Perform user validation
                    var validationService = context.HttpContext.RequestServices
                        .GetRequiredService<IAuthenticationUserValidationService>();

                    if (context.Principal != null)
                    {
                        try
                        {
                            var isValid = await validationService.ValidateUserOnAuthenticationAsync(context.Principal);
                            
                            if (!isValid)
                            {
                                // User not found in system - redirect to sign in with message
                                context.HttpContext.Response.Redirect("/Public/SignIn?error=user_not_found");
                                context.HandleResponse(); // Prevent default authentication flow
                            }
                        }
                        catch (InvalidOperationException ex) when (ex.Message.Contains("Duplicate account detected"))
                        {
                            // Duplicate account detected - redirect to sign in with specific message
                            context.HttpContext.Response.Redirect("/Public/SignIn?error=duplicate_account");
                            context.HandleResponse(); // Prevent default authentication flow
                        }
                        catch (Exception)
                        {
                            // General error - redirect to sign in with generic message
                            context.HttpContext.Response.Redirect("/Public/SignIn?error=validation_failed");
                            context.HandleResponse(); // Prevent default authentication flow
                        }
                    }
                    else
                    {
                        context.HttpContext.Response.Redirect("/Public/SignIn?error=no_principal");
                        context.HandleResponse(); // Prevent default authentication flow
                    }
                };
            });

            return services;
        }
    }
}
