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

public record GetUserWithProjectsQuery(UserId UserId, ProjectState? State, OrderUserProjectsByField? OrderUserProjectsBy = OrderUserProjectsByField.Id)
    : PaginatedRequest<PaginatedResult<UserWithProjectsDto>>;

public class GetUserWithProjectsHandler(
    ICompleteRepository<User> users,
    ICompleteRepository<GiasEstablishment> establishments)
    : IRequestHandler<GetUserWithProjectsQuery, PaginatedResult<UserWithProjectsDto>>
{
    public async Task<PaginatedResult<UserWithProjectsDto>> Handle(
        GetUserWithProjectsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            //Find the user first
            var foundUser = await users.Query()
                .Include(user => user.ProjectAssignedTos)
                .SingleOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);

            // Exit immediately if there is no user
            if (foundUser is null)
                return PaginatedResult<UserWithProjectsDto>.Failure("User is not found");

            var projects = foundUser.ProjectAssignedTos.Where(project => request.State == null || project.State == request.State);

            if (request.OrderUserProjectsBy == OrderUserProjectsByField.SignificantDate)
                projects = projects.OrderBy(project => project.SignificantDate);
            else if (request.OrderUserProjectsBy == OrderUserProjectsByField.Id)
                projects = projects.OrderBy(project => project.Id);

            var projectsList = projects.Skip(request.Page * request.Count).Take(request.Count).ToList();

            var conversionCount = projectsList.Count(projects => projects.Type == ProjectType.Conversion);
            var transferCount = projectsList.Count(projects => projects.Type == ProjectType.Transfer);

            // Find all matching establishments for projects found
            var establishmentUrns = projectsList.Select(p => p.Urn).Distinct();
            var establishmentsDict = await establishments.Query()
                .Where(e => establishmentUrns.Contains(e.Urn)).ToDictionaryAsync(e => e.Urn!, cancellationToken);

            // Format results into expected model
            var result = new UserWithProjectsDto(
                foundUser.Id,
                foundUser.FullName,
                foundUser.Email,
                foundUser.Team.FromDescriptionValue<ProjectTeam>(),
                projectsList.Select(project =>
                {
                    establishmentsDict.TryGetValue(project.Urn, out var establishment);
                    return ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(project,
                        establishment);
                }).ToList(),
                conversionCount,
                transferCount
            );
            return PaginatedResult<UserWithProjectsDto>.Success(result, conversionCount + transferCount);
        }
        catch (Exception ex)
        {
            return PaginatedResult<UserWithProjectsDto>.Failure(ex.Message);
        }
    }
}