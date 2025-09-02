using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetConversionTasksData
{
    public record GetConversionTasksDataByIdQuery(TaskDataId? Id) : IRequest<Result<ConversionTaskDataDto>>;

    public class GetConversionTasksDataByIdQueryHandler(
        ITaskDataReadRepository taskDataReadRepository,
        IMapper mapper,
        ILogger<GetConversionTasksDataByIdQueryHandler> logger)
        : IRequestHandler<GetConversionTasksDataByIdQuery, Result<ConversionTaskDataDto>>
    {
        public async Task<Result<ConversionTaskDataDto>> Handle(GetConversionTasksDataByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await taskDataReadRepository.ConversionTaskData.FirstAsync(p => p.Id == request.Id, cancellationToken);
                var conversionTaskData = mapper.Map<ConversionTaskDataDto>(result);

                return Result<ConversionTaskDataDto>.Success(conversionTaskData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetConversionTasksDataByIdQueryHandler), request);
                return Result<ConversionTaskDataDto>.Failure(ex.Message);
            }
        }
    }
}
