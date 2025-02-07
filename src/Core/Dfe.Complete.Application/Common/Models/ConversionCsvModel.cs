using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Common.Models
{
    public record ConversionCsvModel(Project Project,
                                     GiasEstablishment CurrentSchool,
                                     GiasEstablishment? Academy,
                                     LocalAuthority LocalAuthority,
                                     SignificantDateHistory? SignificantDateHistory,
                                     ConversionTasksData ConversionTasks,
                                     User? CreatedBy,
                                     User? AssignedTo,
                                     Contact? MainContact,
                                     Contact? Headteacher,
                                     Contact? LocalAuthorityContact,
                                     Contact? IncomingContact,
                                     Contact? OutgoingContact,
                                     Contact? IncomingCEOContact,
                                     Contact? SolicitorContact,
                                     Contact? DioceseContact,
                                     Contact? DirectorOfServicesContact);
}
