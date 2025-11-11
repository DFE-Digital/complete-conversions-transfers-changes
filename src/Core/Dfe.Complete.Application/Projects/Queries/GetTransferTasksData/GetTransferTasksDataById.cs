using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetTransferTasksData
{
    public record GetTransferTasksDataByIdQuery(TaskDataId? Id) : IRequest<Result<TransferTaskDataDto>>;

    public class GetTransferTasksDataByIdQueryHandler(
        ITaskDataReadRepository taskDataReadRepository,
        IMapper mapper,
        ILogger<GetTransferTasksDataByIdQueryHandler> logger)
        : IRequestHandler<GetTransferTasksDataByIdQuery, Result<TransferTaskDataDto>>
    {
        public async Task<Result<TransferTaskDataDto>> Handle(GetTransferTasksDataByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await taskDataReadRepository.TransferTaskData.FirstAsync(p => p.Id == request.Id, cancellationToken);

                var transferTaskData = mapper.Map<TransferTaskDataDto>(result);

                return Result<TransferTaskDataDto>.Success(transferTaskData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetTransferTasksDataByIdQueryHandler), request);
                return Result<TransferTaskDataDto>.Failure(ex.Message);
            }
        }
    }
}
