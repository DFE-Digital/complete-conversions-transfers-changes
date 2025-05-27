using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Application.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotDevOrTestEnvironmentException() : Exception("Command is only available on Development and Test environments")
    {
    }

}
