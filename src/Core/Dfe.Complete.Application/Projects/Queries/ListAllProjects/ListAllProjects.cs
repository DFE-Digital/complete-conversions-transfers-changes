﻿using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsQuery(
        ProjectState? ProjectStatus,
        ProjectType? Type,
        int Page = 0,
        int Count = 20) : IRequest<Result<List<ListAllProjectsResultModel>>>;

    public class ListAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<ListAllProjectsQuery, Result<List<ListAllProjectsResultModel>>>
    {
        public async Task<Result<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projectList = await listAllProjectsQueryService
                    .ListAllProjects(request.ProjectStatus, request.Type)
                    .ToListAsync(cancellationToken);
                
                var filteredProjectList = request.ProjectStatus == ProjectState.Active
                    ? projectList.Where(p => p.Project?.AssignedTo != null)
                    : projectList;
                
                var result = filteredProjectList
                    .Skip(request.Page * request.Count).Take(request.Count)
                    .Select(item => new ListAllProjectsResultModel(
                        item.Establishment?.Name,
                        item.Project!.Id,
                        item.Project.Urn,
                        item.Project.SignificantDate,
                        item.Project.State,
                        item.Project.Type,
                        item.Project.FormAMat,
                        item.Project.AssignedTo?.FullName,
                        item.Project.LocalAuthority.Name,
                        item.Project.Team,
                        item.Project.CompletedAt,
                        item.Project.Region,
                        item.Establishment?.LocalAuthorityName
                    ))
                    .ToList();
                return Result<List<ListAllProjectsResultModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<ListAllProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}