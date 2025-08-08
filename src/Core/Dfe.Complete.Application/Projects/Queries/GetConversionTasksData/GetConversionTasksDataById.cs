using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetConversionTasksData
{
    public record GetConversionTasksDataByIdQuery(TaskDataId? Id) : IRequest<Result<ConversionTaskDataDto>>;

    public class GetConversionTasksDataByIdQueryHandler(
        ICompleteRepository<ConversionTasksData> conversionTaskDataRepository,
        IMapper mapper,
        ILogger<GetConversionTasksDataByIdQueryHandler> logger)
        : IRequestHandler<GetConversionTasksDataByIdQuery, Result<ConversionTaskDataDto>>
    {
        public async Task<Result<ConversionTaskDataDto>> Handle(GetConversionTasksDataByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await conversionTaskDataRepository.GetAsync(p => p.Id == request.Id);

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
