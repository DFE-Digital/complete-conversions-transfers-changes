using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Models;

[ExcludeFromCodeCoverage]
public sealed record TaskListItemViewModel(
    string Name,
    string Link,
    TaskListStatus Status
);
