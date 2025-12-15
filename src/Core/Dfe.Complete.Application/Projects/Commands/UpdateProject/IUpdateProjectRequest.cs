using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    /// <summary>
    /// Interface for extracting common properties from update project commands
    /// </summary>
    public interface IUpdateProjectRequest
    {
        ProjectId ProjectId { get; }
        Ukprn? IncomingTrustUkprn { get; }
        string? NewTrustReferenceNumber { get; }
        string? GroupReferenceNumber { get; }
        DateOnly AdvisoryBoardDate { get; }
        string? AdvisoryBoardConditions { get; }
        string EstablishmentSharepointLink { get; }
        string IncomingTrustSharepointLink { get; }
        bool TwoRequiresImprovement { get; }
        bool IsHandingToRCS { get; }
        UserDto User { get; }
    }
}
