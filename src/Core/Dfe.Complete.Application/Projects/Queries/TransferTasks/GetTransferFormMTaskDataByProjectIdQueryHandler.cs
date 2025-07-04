using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Projects.Models.TransferTasks;

namespace Dfe.Complete.Application.Projects.Queries.TransferTasks
{
    public class GetTransferFormMTaskDataByProjectIdQueryHandler : IRequestHandler<GetTransferFormMTaskDataByProjectIdQuery, TransferFormMTaskDataDto>
    {
        private readonly ICompleteRepository<Project> _projectRepository;
        private readonly ICompleteRepository<TransferTasksData> _transferTaskRepository;

        public GetTransferFormMTaskDataByProjectIdQueryHandler(
            ICompleteRepository<Project> projectRepository,
            ICompleteRepository<TransferTasksData> transferTaskRepository)
        {
            _projectRepository = projectRepository;
            _transferTaskRepository = transferTaskRepository;
        }

        public async Task<TransferFormMTaskDataDto> Handle(GetTransferFormMTaskDataByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectId, cancellationToken);
            if (project == null || project.Type != Domain.Enums.ProjectType.Transfer || project.TasksDataId == null)
            {
                return new TransferFormMTaskDataDto();
            }
            var transferTasks = await _transferTaskRepository.GetAsync(project.TasksDataId, cancellationToken);
            if (transferTasks == null)
            {
                return new TransferFormMTaskDataDto();
            }
            return new TransferFormMTaskDataDto
            {
                FormMReceivedFormM = transferTasks.FormMReceivedFormM,
                FormMReceivedTitlePlans = transferTasks.FormMReceivedTitlePlans,
                FormMCleared = transferTasks.FormMCleared,
                FormMSigned = transferTasks.FormMSigned,
                FormMSaved = transferTasks.FormMSaved,
                FormMNotApplicable = transferTasks.FormMNotApplicable
            };
        }
    }
}
