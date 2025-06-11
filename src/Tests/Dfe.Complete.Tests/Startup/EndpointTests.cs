using Dfe.Complete.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;

namespace Dfe.Complete.Tests.StartupTests;

public class EndpointTests
{
    private static readonly Lazy<IEnumerable<RouteEndpoint>> _endpoints = new(InitEndpoints);

    [Fact]
    public void ExportsEndpoint_IsMapped_And_Protected()
    {
        var exports = _endpoints.Value
            .SingleOrDefault(e => e.RoutePattern.RawText == "/projects/all/exports");

        Assert.NotNull(exports);

        var authorize = exports!.Metadata
            .OfType<IAuthorizeData>()
            .SingleOrDefault();

        Assert.NotNull(authorize);
        Assert.Equal(UserPolicyConstants.CanViewAllProjectsExports, authorize!.Policy);
    }

    private static IEnumerable<RouteEndpoint> InitEndpoints()
    {
        var factory = new CustomWebApplicationFactory<Startup>();
        var endpointDataSource = factory.Services.GetRequiredService<EndpointDataSource>();

        return endpointDataSource.Endpoints.OfType<RouteEndpoint>();
    }
}
