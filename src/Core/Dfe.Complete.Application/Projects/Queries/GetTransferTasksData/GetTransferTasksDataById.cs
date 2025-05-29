using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Projects.Queries.GetProject;

namespace Dfe.Complete.Application.Projects.Queries.GetTransferTasksData
{
    public record GetTransferTasksDataByIdQuery(TaskDataId? Id) : IRequest<Result<TransferTaskDataDto>>;

    public class GetTransferTasksDataByIdQueryHandler(
        ICompleteRepository<TransferTasksData> transferTaskDataRepository,
        IMapper mapper,
        ILogger<GetTransferTasksDataByIdQueryHandler> logger)
        : IRequestHandler<GetTransferTasksDataByIdQuery, Result<TransferTaskDataDto>>
    {
        public async Task<Result<TransferTaskDataDto?>> Handle(GetTransferTasksDataByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await transferTaskDataRepository.GetAsync(p => p.Id == request.Id);

                var projectGroupDto = mapper.Map<TransferTaskDataDto?>(result);

                return Result<TransferTaskDataDto?>.Success(projectGroupDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectGroupByIdQueryHandler), request);
                return Result<TransferTaskDataDto?>.Failure(ex.Message);
            }
        }
    }
}
