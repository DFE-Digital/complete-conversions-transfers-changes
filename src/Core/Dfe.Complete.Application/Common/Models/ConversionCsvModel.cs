using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Common.Models
{
    public record ConversionCsvModel(Project Project,
                                     GiasEstablishment CurrentSchool,
                                     GiasEstablishment? Academy,
                                     LocalAuthority LocalAuthority,
                                     SignificantDateHistory? SignificantDateHistory,
                                     ConversionTasksData ConversionTasks,
                                     User? CreatedBy);
}
