using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

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
    Task<ProjectGroupDto?> GetGroupForGroupId(string? groupId, CancellationToken cancellationToken);
    Task<ProjectGroupId> CreateProjectGroup(string groupId, int incomingTrustUkprn, CancellationToken cancellationToken);
    Task ValidateUrnAndTrustsAsync(int urn, int incomingTrustUkprn, int? outgoingTrustUkprn = null, CancellationToken cancellationToken = default);
    Task<HandoverProjectCommonData> PrepareCommonProjectDataAsync(int urn, int incomingTrustUkprn, string? groupId, string createdByFirstName, string createdByLastName, string createdByEmail, CancellationToken cancellationToken);

}
