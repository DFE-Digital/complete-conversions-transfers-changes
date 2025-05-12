using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace Dfe.Complete.Application.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotDevOrTestEnvironmentException() : Exception("Command is only available on Development and Test environments")
    {
    }

}
