using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Users.Queries.GetUser;

public record GetUserWithProjectsQuery(UserId UserId, ProjectState? State = ProjectState.Active)
    : PaginatedRequest<PaginatedResult<UserWithProjectsResultModel>>;

public class GetUserWithProjectsHandler(
    ICompleteRepository<User> users,
    ICompleteRepository<GiasEstablishment> establishments)
    : IRequestHandler<GetUserWithProjectsQuery, PaginatedResult<UserWithProjectsResultModel>>
{
    public async Task<PaginatedResult<UserWithProjectsResultModel>> Handle(GetUserWithProjectsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var foundUser = await users.Query().Include(user =>
                    user.ProjectAssignedTos)
                .FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);
            if (foundUser is null)
            {
                return PaginatedResult<UserWithProjectsResultModel>.Failure("User is not found");
            }

            foundUser.ProjectAssignedTos = foundUser.ProjectAssignedTos
                .Where(project => request.State == null || project.State == request.State).ToList();

            var result = new UserWithProjectsResultModel(foundUser.Id,
                $"{foundUser.FirstName} " +
                $"{foundUser.LastName}",
                foundUser.Email,
                EnumExtensions.FromDescription<ProjectTeam>(foundUser.Team),
                foundUser.ProjectAssignedTos.Skip(request.Page * request.Count).Take(request.Count).Select(
                    async project =>
                    {
                        var establishment = await establishments.FindAsync(
                            establishment => establishment.Urn == project.Urn,
                            cancellationToken);
                        return ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(project,
                            establishment);
                    }).Select(item => item.Result).ToList(),
                foundUser.ProjectAssignedTos.Count(project => project.Type == ProjectType.Conversion),
                foundUser.ProjectAssignedTos.Count(project => project.Type == ProjectType.Transfer)
            );
            return PaginatedResult<UserWithProjectsResultModel>.Success(result, foundUser.ProjectAssignedTos.Count);
        }
        catch (Exception ex)
        {
            return PaginatedResult<UserWithProjectsResultModel>.Failure(ex.Message);
        }
    }
}