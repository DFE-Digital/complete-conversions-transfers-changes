using MediatR;
using Dfe.Complete.Application.Projects.Models.TransferTasks;

namespace Dfe.Complete.Application.Projects.Commands.TransferTasks
{
    public class UpdateTransferFormMTaskDataByProjectIdCommand : IRequest<Unit>
    {
        public long ProjectId { get; }
        public TransferFormMTaskDataDto Data { get; }
        public UpdateTransferFormMTaskDataByProjectIdCommand(long projectId, TransferFormMTaskDataDto data)
        {
            ProjectId = projectId;
            Data = data;
        }
    }
}
