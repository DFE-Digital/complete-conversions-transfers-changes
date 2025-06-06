using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Users.Queries.GetUser;

public record GetUserByIdQuery(UserId UserId) : IRequest<Result<UserDto?>>;

public class GetUserByIdQueryHandler(ICompleteRepository<User> userRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger) : IRequestHandler<GetUserByIdQuery, Result<UserDto?>>
{
    public async Task<Result<UserDto?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken);

            var userDto = mapper.Map<UserDto?>(user);

            return Result<UserDto?>.Success(userDto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(GetUserByIdQueryHandler), request);
            return Result<UserDto?>.Failure(e.Message);
        }
    }
}