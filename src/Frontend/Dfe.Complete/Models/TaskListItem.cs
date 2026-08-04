using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Models;

[ExcludeFromCodeCoverage]
public sealed record TaskListItemViewModel(
    string Name,
    string Link,
    TaskListStatus Status,
    int DisplayOrder
);

[ExcludeFromCodeCoverage]
public sealed record TaskListItemBuildModel(
    NoteTaskIdentifier Identifier,
    TaskListStatus Status,
    int DisplayOrder
);
