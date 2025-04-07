using System;

namespace Dfe.Complete.Extensions;

public static class IHostEnvironmentExtensions {
        public static bool IsTest(this IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment == null) throw new InvalidOperationException("no");

            return hostEnvironment.IsEnvironment("Test");
        }
}