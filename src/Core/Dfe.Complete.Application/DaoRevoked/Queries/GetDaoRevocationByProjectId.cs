using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.DaoRevoked.Interfaces;
using Dfe.Complete.Application.DaoRevoked.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.DaoRevoked.Queries
{
    public record GetDaoRevocationByProjectIdQuery(ProjectId ProjectId) : IRequest<Result<DaoRevocationDto?>>;

    public class GetDaoRevocationByProjectIdQueryHandler(IDaoRevocationReadRepository daoRevocationReadRepository,
         IMapper mapper)
        : IRequestHandler<GetDaoRevocationByProjectIdQuery, Result<DaoRevocationDto?>>
    {
        public async Task<Result<DaoRevocationDto?>> Handle(GetDaoRevocationByProjectIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await daoRevocationReadRepository.DaoRevocations.AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"No project found for Id: {request.ProjectId.Value}.");

                var daoRevocationDto = mapper.Map<DaoRevocationDto?>(result); 

                return Result<DaoRevocationDto?>.Success(daoRevocationDto);
            }
            catch (Exception ex)
            {
                return Result<DaoRevocationDto?>.Failure(ex.Message);
            }
        }
    }
}
