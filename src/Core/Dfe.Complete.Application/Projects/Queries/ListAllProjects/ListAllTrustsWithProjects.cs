﻿using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllTrustsWithProjectsQuery() : PaginatedRequest<PaginatedResult<List<ListTrustsWithProjectsResultModel>>>;

    public class ListAllTrustsWithProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService, ITrustsV4Client trustsClient, ILogger<ListAllTrustsWithProjectsQueryHandler> logger)
        : IRequestHandler<ListAllTrustsWithProjectsQuery, PaginatedResult<List<ListTrustsWithProjectsResultModel>>>
    {
        public async Task<PaginatedResult<List<ListTrustsWithProjectsResultModel>>> Handle(ListAllTrustsWithProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var allProjects = await listAllProjectsQueryService.ListAllProjects(new ProjectFilters(ProjectState.Active, null))
                    .Select(p => p.Project)
                    .ToListAsync(cancellationToken);

                var standardProjects = allProjects.Where(p => !p.FormAMat);
                var matProjects = allProjects.Where(p => p.FormAMat);

                // Get trusts related to projects
                var incomingTrustUkprns = standardProjects.Select(p => p.IncomingTrustUkprn.Value.ToString()).Distinct();
                var standardProjectsTrust = await trustsClient.GetByUkprnsAllAsync(incomingTrustUkprns, cancellationToken);

                var trusts = standardProjectsTrust
                    .Select(item => new ListTrustsWithProjectsResultModel(
                        item.Ukprn,
                        item.Name.ToTitleCase(),
                        item.ReferenceNumber,
                        standardProjects.Count(p => p.IncomingTrustUkprn?.ToString() == item.Ukprn && p.Type == ProjectType.Conversion),
                        standardProjects.Count(p => p.IncomingTrustUkprn?.ToString() == item.Ukprn && p.Type == ProjectType.Transfer)
                    ))
                    .ToList();
                
                //Group mats by reference and form result model
                var mats = matProjects
                    .GroupBy(p => p.NewTrustReferenceNumber)
                    .Select(trust => new ListTrustsWithProjectsResultModel(
                        trust.Key,
                        trust.First().NewTrustName, 
                        trust.Key,
                        trust.Count(p => p.Type == ProjectType.Conversion),
                        trust.Count(p => p.Type == ProjectType.Transfer)
                    ))
                    .ToList();

                var allTrusts = trusts.Concat(mats)
                    .OrderBy(r => r.trustName)
                    .ToList();

                var result = allTrusts
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToList();

                return PaginatedResult<List<ListTrustsWithProjectsResultModel>>.Success(result, allTrusts.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(ListAllTrustsWithProjectsQueryHandler), request);
                return PaginatedResult<List<ListTrustsWithProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}