using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.GetUser;

public record GetUserByAdIdQuery(string UserAdId) : IRequest<Result<UserDto?>>;

public class GetUserByAdIdQueryHandler(ICompleteRepository<User> userRepository, IMapper mapper) : IRequestHandler<GetUserByAdIdQuery, Result<UserDto?>>
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
            return Result<UserDto?>.Failure(e.Message);
        }
    }
}