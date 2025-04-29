using DfE.CoreLibs.Security.Cypress;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Dfe.Complete.Tests.Authorization
{
    public class CypressAuthenticationRegistrationTests
    {
        private ServiceProvider BuildServices(Action<IServiceCollection> configure)
        {
            var services = new ServiceCollection();

            services.AddHttpContextAccessor();

            // Minimal IConfiguration with the CypressTestSecret
            var inMemConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string,string>("CypressTestSecret","super‐secret")
                }!)
                .Build();

            services.AddSingleton<IConfiguration>(inMemConfig);

            // Fake environment as Development
            services.AddSingleton<IHostEnvironment>(new HostingEnvironment { EnvironmentName = "Development" });

            configure(services);
            return services.BuildServiceProvider();
        }

        [Fact]
        public void AddCypressMultiAuthentication_Registers_ICypressRequestChecker()
        {
            var sp = BuildServices(services =>
            {
                // wire up AddAuthentication
                services
                    .AddAuthentication("MultiAuth")
                    .AddCypressMultiAuthentication();
            });

            // ICypressRequestChecker should be resolvable
            var checker = sp.GetService<ICypressRequestChecker>();
            Assert.NotNull(checker);
            Assert.IsType<CypressRequestChecker>(checker);
        }

        [Fact]
        public async Task AddCypressMultiAuthentication_Sets_DefaultSchemes_And_Registers_Schemes()
        {
            var sp = BuildServices(services =>
            {
                services
                    .AddAuthentication(opts =>
                    {
                        opts.DefaultScheme = "MultiAuth";
                        opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    })
                    .AddCypressMultiAuthentication();

                // Also register the real OIDC scheme so that DefaultChallengeScheme is valid
                services.AddAuthentication().AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, _ => { });
                // And the fallback cookie scheme
                services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
            });

            var authOptions = sp.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
            Assert.Equal("MultiAuth", authOptions.DefaultScheme);
            Assert.Equal(OpenIdConnectDefaults.AuthenticationScheme, authOptions.DefaultChallengeScheme);

            var schemeProvider = sp.GetRequiredService<IAuthenticationSchemeProvider>();

            var multi = await schemeProvider.GetSchemeAsync("MultiAuth");
            Assert.NotNull(multi);
            Assert.Equal("MultiAuth", multi.Name);
            Assert.Equal(typeof(PolicySchemeHandler), multi.HandlerType);

            // The "CypressAuth" scheme must be registered
            var cypress = await schemeProvider.GetSchemeAsync("CypressAuth");
            Assert.NotNull(cypress);
            Assert.Equal("CypressAuth", cypress.Name);
            // And its handler should be our test‐handler
            Assert.Equal(typeof(CypressAuthenticationHandler), cypress.HandlerType);

            // The fallback scheme (Cookies) should still be available
            var cookie = await schemeProvider.GetSchemeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Assert.NotNull(cookie);
        }
    }

    internal class HostingEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
