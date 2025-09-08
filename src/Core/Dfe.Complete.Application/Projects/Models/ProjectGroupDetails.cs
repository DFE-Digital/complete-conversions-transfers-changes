namespace Dfe.Complete.Application.Projects.Models;

public record ProjectGroupDetails(string GroupId, string TrustName, string TrustReference, string GroupReference, IEnumerable<ProjectGroupCardDetails> ProjectDetails);