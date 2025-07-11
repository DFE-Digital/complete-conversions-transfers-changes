using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetGiasEstablishment
{
    public record GetGiasEstablishmentByUrnQuery(Urn Urn) : IRequest<Result<GiasEstablishmentDto?>>;

    public class GetGiasEstablishmentByUrnQueryHandler(ICompleteRepository<GiasEstablishment> giasEstablishmentRepository,
         IMapper mapper,
         ILogger<GetGiasEstablishmentByUrnQueryHandler> logger)
        : IRequestHandler<GetGiasEstablishmentByUrnQuery, Result<GiasEstablishmentDto?>>
    {
        public async Task<Result<GiasEstablishmentDto?>> Handle(GetGiasEstablishmentByUrnQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await giasEstablishmentRepository.GetAsync(p => p.Urn == request.Urn);

                var dto = mapper.Map<GiasEstablishmentDto?>(result);

                return Result<GiasEstablishmentDto?>.Success(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetGiasEstablishmentByUrnQueryHandler), request);
                return Result<GiasEstablishmentDto?>.Failure(ex.Message);
            }
        }
    }
}
