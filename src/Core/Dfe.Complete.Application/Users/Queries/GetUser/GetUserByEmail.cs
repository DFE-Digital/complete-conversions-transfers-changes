using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Users.Queries.GetUser;

public record GetUserByEmailQuery(string Email) : IRequest<Result<UserDto?>>;

public class GetUserByEmailQueryHandler(ICompleteRepository<User> userRepository, IMapper mapper, ILogger<GetUserByEmailQueryHandler> logger) : IRequestHandler<GetUserByEmailQuery, Result<UserDto?>>
{
    public async Task<Result<UserDto?>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.FindAsync(user => user.Email == request.Email, cancellationToken);

            var userDto = mapper.Map<UserDto?>(user);

            return Result<UserDto?>.Success(userDto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(GetUserByEmailQueryHandler), request);
            return Result<UserDto?>.Failure(e.Message);
        }
    }
}