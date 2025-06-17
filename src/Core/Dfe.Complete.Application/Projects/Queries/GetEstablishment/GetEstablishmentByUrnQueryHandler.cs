using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Common.Models;
using AutoMapper;
using Dfe.AcademiesApi.Client.Contracts;

namespace Dfe.Complete.Application.Projects.Queries.GetEstablishment
{
    public record GetEstablishmentByUrnQuery(int Urn) : IRequest<Result<EstablishmentDto?>>;

    public class GetEstablishmentByUrnQueryHandler(ICompleteRepository<GiasEstablishment> giasEstablishmentRepo,
        IMapper mapper)
        : IRequestHandler<GetEstablishmentByUrnQuery, Result<EstablishmentDto?>>
    {
        public async Task<Result<EstablishmentDto?>> Handle(GetEstablishmentByUrnQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var giasEstablishment = await giasEstablishmentRepo.GetAsync(x => x.Urn == new Urn(request.Urn));
                var establishmentDto = mapper.Map<EstablishmentDto?>(giasEstablishment);

                return Result<EstablishmentDto?>.Success(establishmentDto);
            }
            catch (Exception ex)
            {
                return Result<EstablishmentDto?>.Failure(ex.Message);
            }
        }
    }
}