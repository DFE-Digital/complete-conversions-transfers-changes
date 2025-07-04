using MediatR;
using Dfe.Complete.Application.Projects.Models.TransferTasks;

namespace Dfe.Complete.Application.Projects.Queries.TransferTasks
{
    public class GetTransferFormMTaskDataByProjectIdQuery : IRequest<TransferFormMTaskDataDto>
    {
        public long ProjectId { get; }
        public GetTransferFormMTaskDataByProjectIdQuery(long projectId)
        {
            ProjectId = projectId;
        }
    }
}
