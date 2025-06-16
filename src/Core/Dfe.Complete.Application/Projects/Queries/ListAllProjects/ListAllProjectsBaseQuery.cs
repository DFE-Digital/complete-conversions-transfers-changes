using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public class ListAllProjectsBaseQuery
    {
        public int Page { get; init; }
        public int Count { get; init; }
        public ProjectState? State { get; init; }
        public bool? IsFormAMat { get; init; }
        public string? IncomingTrustUkprn { get; init; }
        public string? Search { get; init; }
        public OrderProjectQueryBy? OrderBy { get; init; }
    }
}
