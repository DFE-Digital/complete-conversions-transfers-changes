using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.DaoRevoked.Interfaces;
using Dfe.Complete.Application.DaoRevoked.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.DaoRevoked.Queries
{
    public record GetDaoRevocationByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<DaoRevocationDto?>>;

    public class GetDaoRevocationByProjectIdQueryHandler(IDaoRevocationReadRepository daoRevocationReadRepository,
         IMapper mapper,
         ILogger<GetDaoRevocationByProjectIdQueryHandler> logger)
        : IRequestHandler<GetDaoRevocationByProjectIdQuery, Result<DaoRevocationDto?>>
    {
        public async Task<Result<DaoRevocationDto?>> Handle(GetDaoRevocationByProjectIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await daoRevocationReadRepository.DaoRevocations.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.ProjectId == request.ProjectId, cancellationToken) ?? throw new NotFoundException($"No project found for Id: {request.ProjectId.Value}.");

                var daoRevocationDto = mapper.Map<DaoRevocationDto?>(result); 

                return Result<DaoRevocationDto?>.Success(daoRevocationDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetDaoRevocationByProjectIdQueryHandler), request);

                return Result<DaoRevocationDto?>.Failure(ex.Message);
            }
        }
    }
}
