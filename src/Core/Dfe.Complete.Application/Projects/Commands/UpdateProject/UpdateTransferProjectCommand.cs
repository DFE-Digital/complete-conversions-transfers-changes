using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    public record UpdateTransferProjectCommand(
        ProjectId ProjectId,
        Ukprn? IncomingTrustUkprn,        
        string? NewTrustReferenceNumber,
        string? GroupReferenceNumber,
        DateOnly AdvisoryBoardDate,
        string? AdvisoryBoardConditions,
        string EstablishmentSharepointLink,
        string IncomingTrustSharepointLink,
        string OutgoingTrustSharepointLink,
        bool TwoRequiresImprovement,
        bool InadequateOfsted,
        bool FinancialSafeguardingGovernanceIssues,
        bool OutgoingTrustToClose,
        bool IsHandingToRCS,
        UserDto User
    ) : IRequest, IUpdateProjectRequest;

    public class UpdateTransferProjectCommandHandler : UpdateProjectCommandBase<UpdateTransferProjectCommand>, IRequestHandler<UpdateTransferProjectCommand>
    {
        private readonly ICompleteRepository<TransferTasksData> _transferTaskDataRepository;

        public UpdateTransferProjectCommandHandler(
            ICompleteRepository<Project> projectRepository,
            ICompleteRepository<ProjectGroup> projectGroupRepository,
            ICompleteRepository<TransferTasksData> transferTaskDataRepository)
            : base(projectRepository, projectGroupRepository)
        {
            _transferTaskDataRepository = transferTaskDataRepository;
        }

        protected override async Task UpdateSpecificProjectProperties(Project project, UpdateTransferProjectCommand request, CancellationToken cancellationToken)
        {
            // Transfer-specific properties           
            project.OutgoingTrustSharepointLink = request.OutgoingTrustSharepointLink;

            // Update transfer tasks data
            var tasksData = await _transferTaskDataRepository.GetAsync(p => p.Id == project.TasksDataId);
            if (tasksData != null) // we take for granted that each project has a TasksData record attached
            {
                tasksData.InadequateOfsted = request.InadequateOfsted;
                tasksData.FinancialSafeguardingGovernanceIssues = request.FinancialSafeguardingGovernanceIssues;
                tasksData.OutgoingTrustToClose = request.OutgoingTrustToClose;

                await _transferTaskDataRepository.UpdateAsync(tasksData, cancellationToken);
            }
        }
    }
}
