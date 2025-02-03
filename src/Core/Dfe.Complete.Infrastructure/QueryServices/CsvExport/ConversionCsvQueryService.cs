using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces.CsvExport;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices.CsvExport
{
    public class ConversionCsvQueryService(CompleteContext context) : IConversionCsvQueryService
    {
        public IQueryable<ConversionCsvModel> GetByMonth(int month, int year)
        {


            //var query = context.Projects
            //  .Where(project => project.Type == ProjectType.Conversion)
            //  .Where(project => project.SignificantDate.Value.Month == month && project.SignificantDate.Value.Month == year)
            //  .Join(context.GiasEstablishments, project => project.Urn, establishment => establishment.Urn,
            //    (project, establishment) => new { project, establishment })
            //  .Join(context.GiasEstablishments, composite => composite.project.AcademyUrn, establishment => establishment.Urn,
            //    (composite, academy) => new { composite.project, composite.establishment, academy })
            //  .Join(context.LocalAuthorities, composite => composite.establishment.LocalAuthorityCode, localAuthority => localAuthority.Code,
            //    (composite, localAuthority) => new { composite.project, composite.establishment, composite.academy, localAuthority })
            //  .Join(context.ConversionTasksData, composite => composite.project.TasksDataId, taskData => taskData.Id.Value,
            //    (composite, taskData) => new { composite.project, composite.establishment, composite.academy, composite.localAuthority, taskData })
            //  .Join(context.KeyContacts, composite => composite.project.Id, keyContact => keyContact.ProjectId,
            //    (composite, keyContact) => new { composite.project, composite.establishment, composite.academy, composite.localAuthority, composite.taskData, keyContact })
            //  .Join(context.Contacts, composite => composite.keyContact.HeadteacherId, contact => contact.Id,
            //    (composite, headteacher) => new { composite.project, composite.establishment, composite.academy, composite.localAuthority, composite.taskData, composite.keyContact, headteacher })
            //  .GroupJoin(context.SignificantDateHistories, composite => composite.project.Id, significantDateHistory => significantDateHistory.ProjectId,
            //            (composite, significantDateHistory) => new { composite, significantDateHistory })
            //  .SelectMany(x => x.significantDateHistory.OrderByDescending(x => x.UpdatedAt).Take(1),
            //                (x, significantDateHistory) => new ConversionCsvModel(x.composite.project, x.composite.establishment, x.composite.academy, x.composite.localAuthority, significantDateHistory, x.composite.taskData, x.composite.project.RegionalDeliveryOfficer, x.));

            //var query = from project in context.Projects
            //            where project.Type == ProjectType.Conversion
            //               && project.SignificantDate.Value.Month == month
            //               && project.SignificantDate.Value.Year == year
            //            join establishment in context.GiasEstablishments on project.Urn equals establishment.Urn
            //            join academy in context.GiasEstablishments on project.AcademyUrn equals academy.Urn
            //            join localAuthority in context.LocalAuthorities on establishment.LocalAuthorityCode equals localAuthority.Code
            //            join taskData in context.ConversionTasksData on project.TasksDataId equals taskData.Id.Value
            //            join keyContact in context.KeyContacts on project.Id equals keyContact.ProjectId
            //            join headteacher in context.Contacts on keyContact.HeadteacherId equals headteacher.Id
            //            join mainContact in context.Contacts on project.MainContactId equals mainContact.Id
            //            join laContact in context.Contacts on localAuthority.Id equals laContact.LocalAuthorityId
            //            join incomingTrustContact in context.Contacts on project.IncomingTrustMainContactId equals incomingTrustContact.Id
            //            join outgoingTrustContact in context.Contacts on project.OutgoingTrustMainContactId equals outgoingTrustContact.Id
            //            join incomingTrustCeo in context.Contacts on keyContact.IncomingTrustCeoId equals incomingTrustCeo.Id
            //            join solicitor in context.Contacts on project.Id equals solicitor.ProjectId
            //            where solicitor.Category == ContactCategory.Solicitor
            //            join diocese in context.Contacts on project.Id equals diocese.ProjectId
            //            where diocese.Category == ContactCategory.Diocese
            //            join directorOfServices in context.Contacts on project.Id equals directorOfServices.ProjectId
            //            where directorOfServices.Category == ContactCategory.LocalAuthority
            //            where diocese.Category == ContactCategory.Diocese
            //            join significantDateHistory in context.SignificantDateHistories on project.Id equals significantDateHistory.ProjectId into x
            //            from sd in x.OrderByDescending(x => x.UpdatedAt).Take(1).DefaultIfEmpty()
            //            select new ConversionCsvModel(
            //                project,
            //                establishment,
            //                academy,
            //                localAuthority,
            //                sd,
            //                taskData,
            //                project.RegionalDeliveryOfficer,
            //                project.AssignedTo,
            //                mainContact,
            //                headteacher,
            //                laContact,
            //                incomingTrustContact,
            //                outgoingTrustContact,
            //                incomingTrustCeo,
            //                solicitor,
            //                diocese,
            //                directorOfServices
            //            );
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
