using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces.CsvExport;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;

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

            var query = from project in context.Projects
                        where project.Type == ProjectType.Conversion
                           && project.SignificantDate.Value.Month == month
                           && project.SignificantDate.Value.Year == year
                        join establishment in context.GiasEstablishments on project.Urn equals establishment.Urn
                        join academy in context.GiasEstablishments on project.AcademyUrn equals academy.Urn
                        join localAuthority in context.LocalAuthorities on establishment.LocalAuthorityCode equals localAuthority.Code
                        join taskData in context.ConversionTasksData on project.TasksDataId equals taskData.Id.Value
                        join keyContact in context.KeyContacts on project.Id equals keyContact.ProjectId
                        join headteacher in context.Contacts on keyContact.HeadteacherId equals headteacher.Id
                        join mainContact in context.Contacts on project.MainContactId equals mainContact.Id
                        join laContact in context.Contacts on localAuthority.Id equals laContact.LocalAuthorityId
                        join incomingTrustContact in context.Contacts on project.IncomingTrustMainContactId equals incomingTrustContact.Id
                        join outgoingTrustContact in context.Contacts on project.OutgoingTrustMainContactId equals outgoingTrustContact.Id
                        join incomingTrustCeo in context.Contacts on keyContact.IncomingTrustCeoId equals incomingTrustCeo.Id
                        join solicitor in context.Contacts on project.Id equals solicitor.ProjectId where solicitor.Category == ContactCategory.Solicitor
                        join diocese in context.Contacts on project.Id equals diocese.ProjectId where diocese.Category == ContactCategory.Diocese
                        join directorOfServices in context.Contacts on project.Id equals directorOfServices.ProjectId where directorOfServices.Category == ContactCategory.LocalAuthority
                        where diocese.Category == ContactCategory.Diocese
                        join significantDateHistory in context.SignificantDateHistories on project.Id equals significantDateHistory.ProjectId into x
                            from sd in x.OrderByDescending(x => x.UpdatedAt).Take(1).DefaultIfEmpty()
            select new ConversionCsvModel(
                project,
                establishment,
                academy,
                localAuthority,
                sd,
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


            return query;
        }
    }
}
