using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public class ProjectGroupDto
{
    public ProjectGroupId Id { get; set; }

    public string? GroupIdentifier { get; set; }

    public Ukprn? TrustUkprn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
