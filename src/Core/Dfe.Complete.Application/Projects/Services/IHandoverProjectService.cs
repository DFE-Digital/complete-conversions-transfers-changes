using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Services;

public interface IHandoverProjectService
{
    Task<UserId> GetOrCreateUserAsync(UserDto userDto, CancellationToken cancellationToken);
    Task<Project?> FindExistingProjectAsync(int urn, CancellationToken cancellationToken);
    Task SaveProjectAndTaskAsync<TTaskData>(Project project, TTaskData taskData, CancellationToken cancellationToken) where TTaskData : class;
    ConversionTasksData CreateConversionTaskAsync();
    TransferTasksData CreateTransferTaskAsync(
        bool InadequateOfsted = false,
        bool FinancialSafeguardingGovernanceIssues = false,

        bool OutgoingTrustToClose = false);
    void ValidateGroupId(ProjectGroupDto group, int trustUkprn);
    Task<Guid> GetLocalAuthorityForUrn(int urn, CancellationToken cancellationToken);
    Task<Region> GetRegionForUrn(int urn, CancellationToken cancellationToken);
    Task<ProjectGroupId> GetOrCreateProjectGroup(string groupId, int incomingTrustUkprn, CancellationToken cancellationToken);
    Task ValidateUrnAsync(int urn, CancellationToken cancellationToken);
    Task ValidateTrnAndTrustNameAsync(string trn, string trustName, CancellationToken cancellationToken);
    Task ValidateTrustAsync(int trustUkprn, CancellationToken cancellationToken);
    Task<HandoverProjectCommonData> PrepareCommonProjectDataAsync(int urn, string createdByFirstName, string createdByLastName, string createdByEmail, CancellationToken cancellationToken);
}
