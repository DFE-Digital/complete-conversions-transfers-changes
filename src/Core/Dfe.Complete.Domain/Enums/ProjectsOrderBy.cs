namespace Dfe.Complete.Domain.Enums;

public enum OrderByDirection {
    Ascending,
    Descending
}

public enum OrderProjectByField
{
    CompletedAt,
    CreatedAt,
    SignificantDate
}

public enum OrderUserByField
{
    CreatedAt,
    Team,
    Email
}
