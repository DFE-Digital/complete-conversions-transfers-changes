using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Projects.Models.TransferTasks;

namespace Dfe.Complete.Application.Projects.Commands.TransferTasks
{
    public class UpdateTransferFormMTaskDataByProjectIdCommandHandler : IRequestHandler<UpdateTransferFormMTaskDataByProjectIdCommand, Unit>
    {
        private readonly ICompleteRepository<Project> _projectRepository;
        private readonly ICompleteRepository<TransferTasksData> _transferTaskRepository;

        public UpdateTransferFormMTaskDataByProjectIdCommandHandler(
            ICompleteRepository<Project> projectRepository,
            ICompleteRepository<TransferTasksData> transferTaskRepository)
        {
            _projectRepository = projectRepository;
            _transferTaskRepository = transferTaskRepository;
        }

        public async Task<Unit> Handle(UpdateTransferFormMTaskDataByProjectIdCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectId, cancellationToken);
            if (project == null || project.Type != Domain.Enums.ProjectType.Transfer || project.TasksDataId == null)
            {
                return Unit.Value;
            }
            var transferTasks = await _transferTaskRepository.GetAsync(project.TasksDataId, cancellationToken);
            if (transferTasks == null)
            {
                return Unit.Value;
            }
            transferTasks.FormMReceivedFormM = request.Data.FormMReceivedFormM;
            transferTasks.FormMReceivedTitlePlans = request.Data.FormMReceivedTitlePlans;
            transferTasks.FormMCleared = request.Data.FormMCleared;
            transferTasks.FormMSigned = request.Data.FormMSigned;
            transferTasks.FormMSaved = request.Data.FormMSaved;
            transferTasks.FormMNotApplicable = request.Data.FormMNotApplicable;
            await _transferTaskRepository.UpdateAsync(transferTasks, cancellationToken);
            return Unit.Value;
        }
    }
}
