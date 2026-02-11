namespace Dfe.Complete.Models;

public sealed record TaskListItemViewModel(
    string Name,
    string Link,
    TaskListStatus Status
);
