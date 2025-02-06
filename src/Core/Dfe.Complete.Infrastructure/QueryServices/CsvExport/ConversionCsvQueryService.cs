using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces.CsvExport;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices.CsvExport
{
    public class ConversionCsvQueryService(CompleteContext context) : IConversionCsvQueryService
    {
        public IQueryable<ConversionCsvModel> GetByMonth(int month, int year)
        {
            var latestDates = context.SignificantDateHistories
                .GroupBy(sdh => sdh.ProjectId)
                .Select(g => new { ProjectId = g.Key, UpdatedAt = g.Max(sdh => sdh.UpdatedAt) });

            var query = from project in context.Projects
                        where project.Type == ProjectType.Conversion
                           && project.SignificantDate.Value.Month == month
                           && project.SignificantDate.Value.Year == year
                           && project.SignificantDateProvisional == false
                           && project.State == ProjectState.Active
                           && project.AssignedTo != null
                        join establishment in context.GiasEstablishments on project.Urn equals establishment.Urn
                        join academy in context.GiasEstablishments on project.AcademyUrn equals academy.Urn into academyJoin
                            from academy in academyJoin.DefaultIfEmpty() 
                        join localAuthority in context.LocalAuthorities on establishment.LocalAuthorityCode equals localAuthority.Code
                        join taskData in context.ConversionTasksData on project.TasksDataId equals taskData.Id
                        join keyContact in context.KeyContacts on project.Id equals keyContact.ProjectId into keyContactJoin
                            from keyContact in keyContactJoin.DefaultIfEmpty()
                        join headteacher in context.Contacts on keyContact.HeadteacherId equals headteacher.Id into headteacherJoin
                            from headteacher in headteacherJoin.DefaultIfEmpty()
                        join mainContact in context.Contacts on project.MainContactId equals mainContact.Id into mainContactJoin
                            from mainContact in mainContactJoin.DefaultIfEmpty()
                        join laContact in context.Contacts on localAuthority.Id equals laContact.LocalAuthorityId into laContactJoin
                            from laContact in laContactJoin.DefaultIfEmpty()
                        join incomingTrustContact in context.Contacts on project.IncomingTrustMainContactId equals incomingTrustContact.Id into incomingTrustContactJoin
                            from incomingTrustContact in incomingTrustContactJoin.DefaultIfEmpty()
                        join outgoingTrustContact in context.Contacts on project.OutgoingTrustMainContactId equals outgoingTrustContact.Id into outgoingTrustContactJoin
                            from outgoingTrustContact in outgoingTrustContactJoin.DefaultIfEmpty()
                        join incomingTrustCeo in context.Contacts on keyContact.IncomingTrustCeoId equals incomingTrustCeo.Id into incomingTrustCeoJoin
                            from incomingTrustCeo in incomingTrustCeoJoin.DefaultIfEmpty()
                        join solicitor in context.Contacts
                            on new { ProjectId = project.Id, Category = (int?)ContactCategory.Solicitor }
                            equals new { ProjectId = solicitor.ProjectId, Category = (int?)solicitor.Category }
                            into solicitorJoin
                        from solicitor in solicitorJoin.DefaultIfEmpty() // Ensures LEFT JOIN behavior
                        join diocese in context.Contacts
                            on new { ProjectId = project.Id, Category = (int?)ContactCategory.Diocese }
                            equals new { ProjectId = diocese.ProjectId, Category = (int?)diocese.Category }
                            into dioceseJoin
                        from diocese in dioceseJoin.DefaultIfEmpty()
                        join directorOfServices in context.Contacts
                            on new { LaId = localAuthority.Id, Category = (int?)ContactCategory.LocalAuthority }
                            equals new { LaId = directorOfServices.LocalAuthorityId, Category = (int?)directorOfServices.Category }
                            into directorOfServicesJoin
                        from directorOfServices in directorOfServicesJoin.DefaultIfEmpty()
                        join latestDate in latestDates on project.Id equals latestDate.ProjectId into latestDateJoin
                        from latestDate in latestDateJoin.DefaultIfEmpty()
                        join significantDateHistory in context.SignificantDateHistories
                            on new { ProjectId = project.Id, UpdatedAt = latestDate != null ? latestDate.UpdatedAt : (DateTime?)null }
                            equals new { ProjectId = significantDateHistory.ProjectId, UpdatedAt = (DateTime?)significantDateHistory.UpdatedAt }
                            into significantDateHistoryJoin
                        from significantDateHistory in significantDateHistoryJoin.DefaultIfEmpty()
                        select new ConversionCsvModel(
                            project,
                            establishment,
                            academy,
                            localAuthority,
                            null,
                            taskData,
                            project.RegionalDeliveryOfficer,
                            project.AssignedTo,
                            mainContact,
                            headteacher,
                            laContact,
                            incomingTrustContact,
                            outgoingTrustContact,
                            incomingTrustCeo,
                            solicitor,
                            diocese,
                            directorOfServices
                        );
            var a = query.ToQueryString();

            return query;
        }
    }
}
