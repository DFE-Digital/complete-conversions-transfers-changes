﻿using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.CountAllProjects
{
    public record CountAllProjectsQuery(ProjectState? ProjectStatus, ProjectType? Type) : IRequest<Result<int>>;

    public class CountAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<CountAllProjectsQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(CountAllProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectsQuery = await listAllProjectsQueryService
                    .ListAllProjects(request.ProjectStatus, request.Type)
                    .ToListAsync(cancellationToken);
                
                var filteredProjectQuery = request.ProjectStatus == ProjectState.Active
                    ? projectsQuery.Where(p => p.Project.AssignedTo != null)
                    : projectsQuery;
                
                var result = filteredProjectQuery.Count();
                
                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure(ex.Message);
            }
        }
    }
}