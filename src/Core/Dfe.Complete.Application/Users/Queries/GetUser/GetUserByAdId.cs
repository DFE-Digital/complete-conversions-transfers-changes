using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Users.Queries.GetUser;

public record GetUserByAdIdQuery(string UserAdId) : IRequest<Result<UserDto?>>;

public class GetUserByAdIdQueryHandler(ICompleteRepository<User> userRepository, IMapper mapper, ILogger<GetUserByAdIdQueryHandler> logger) : IRequestHandler<GetUserByAdIdQuery, Result<UserDto?>>
{
    public async Task<Result<UserDto?>> Handle(GetUserByAdIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.FindAsync(u => u.ActiveDirectoryUserId == request.UserAdId, cancellationToken);

            var userDto = mapper.Map<UserDto?>(user);

            return Result<UserDto?>.Success(userDto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(GetUserByAdIdQueryHandler), request);
            return Result<UserDto?>.Failure(e.Message);
        }
    }
}