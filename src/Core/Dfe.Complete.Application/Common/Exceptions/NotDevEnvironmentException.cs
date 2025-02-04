using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace Dfe.Complete.Application.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotDevEnvironmentException() : Exception("Command is only available on Development environment")
    {
    }

}
