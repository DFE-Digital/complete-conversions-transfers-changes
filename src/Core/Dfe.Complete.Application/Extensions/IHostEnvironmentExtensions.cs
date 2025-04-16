using Microsoft.Extensions.Hosting;

namespace Dfe.Complete.Application.Extensions;

public static class IHostEnvironmentExtensions
{
    public static bool IsTest(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        return hostEnvironment.IsEnvironment("Test");
    }
}
