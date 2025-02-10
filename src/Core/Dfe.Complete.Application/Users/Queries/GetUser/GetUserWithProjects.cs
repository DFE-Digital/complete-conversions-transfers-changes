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

public record GetUserWithProjectsQuery(UserId UserId, ProjectState? State)
    : PaginatedRequest<PaginatedResult<UserWithProjectsDto>>;

public class GetUserWithProjectsHandler(
    ICompleteRepository<User> users,
    ICompleteRepository<GiasEstablishment> establishments)
    : IRequestHandler<GetUserWithProjectsQuery, PaginatedResult<UserWithProjectsDto>>
{
    public async Task<PaginatedResult<UserWithProjectsDto>> Handle(GetUserWithProjectsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            //Find the user first
            var foundUser = await users.Query()
                .Where(user => user.Id == request.UserId)
                .Select(user => new
                {
                    User = user,
                    FilteredProjects = user.ProjectAssignedTos
                        .Where(project => request.State == null || project.State == request.State)
                        .OrderBy(p => p.Id)
                        .Skip(request.Page * request.Count)
                        .Take(request.Count)
                        .ToList(),
                    ConversionCount = user.ProjectAssignedTos.Count(project => project.Type == ProjectType.Conversion),
                    TransferCount = user.ProjectAssignedTos.Count(project => project.Type == ProjectType.Transfer)
                })
                .FirstOrDefaultAsync(cancellationToken);

            // Exit immediately if there is no user
            if (foundUser is null)
            {
                return PaginatedResult<UserWithProjectsDto>.Failure("User is not found");
            }

            // Find all matching establishments for projects found
            var establishmentUrns = foundUser.FilteredProjects.Select(p => p.Urn).Distinct();
            var establishmentsDict = await establishments.Query()
                .Where(e => establishmentUrns.Contains(e.Urn)).ToDictionaryAsync(e => e.Urn!, cancellationToken);

            // Format results into expected model
            var result = new UserWithProjectsDto(
                foundUser.User.Id,
                $"{foundUser.User.FirstName} {foundUser.User.LastName}",
                foundUser.User.Email,
                foundUser.User.Team.FromDescriptionValue<ProjectTeam>(),
                foundUser.FilteredProjects.Select(project =>
                {
                    establishmentsDict.TryGetValue(project.Urn, out var establishment);
                    return ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(project,
                        establishment);
                }).ToList(),
                foundUser.ConversionCount,
                foundUser.TransferCount
            );
            return PaginatedResult<UserWithProjectsDto>.Success(result, foundUser.ConversionCount + foundUser.TransferCount);
        }
        catch (Exception ex)
        {
            return PaginatedResult<UserWithProjectsDto>.Failure(ex.Message);
        }
    }
}