using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces.CsvExport;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Infrastructure.Models;

namespace Dfe.Complete.Infrastructure.QueryServices.CsvExport
{
    public class ConversionCsvQueryService(CompleteContext context) : IConversionCsvQueryService
    {
        public IQueryable<ConversionCsvModel> GetByMonth(int month, int year)
        {
            //var subQuery = context.SignificantDateHistories.OrderByDescending(x => x.UpdatedAt).FirstOrDefault();

            var query = context.Projects
              .Where(project => project.Type == ProjectType.Conversion)
              .Where(project => project.SignificantDate.Value.Month == month && project.SignificantDate.Value.Month == year)
              .Join(context.GiasEstablishments, project => project.Urn, establishment => establishment.Urn,
                (project, establishment) => new { project, establishment })
              .Join(context.GiasEstablishments, composite => composite.project.AcademyUrn, establishment => establishment.Urn,
                (composite, academy) => new { composite.project, composite.establishment, academy })
              .Join(context.LocalAuthorities, composite => composite.establishment.LocalAuthorityCode, localAuthority => localAuthority.Code,
                (composite, localAuthority) => new { composite.project, composite.establishment, composite.academy, localAuthority })
              .GroupJoin(context.SignificantDateHistories, composite => composite.project.Id, significantDateHistory => significantDateHistory.ProjectId,
                        (composite, significantDateHistory) => new { composite.project, composite.establishment, composite.academy, composite.localAuthority, significantDateHistory })
              .SelectMany(x => x.significantDateHistory.OrderByDescending(x => x.UpdatedAt).Take(1),
                            (x, significantDateHistory) => new ConversionCsvModel(x.project, x.establishment, x.academy, x.localAuthority, significantDateHistory));


            return query;
        }
    }
}
