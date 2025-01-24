using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Models;

namespace Dfe.Complete.Application.Common.Models
{
    public record ConversionCsvModel(Project Project,
                                     GiasEstablishment CurrentSchool,
                                     GiasEstablishment? Academy,
                                     LocalAuthority LocalAuthority,
                                     SignificantDateHistory? SignificantDateHistory,
                                     ConversionTasksData ConversionTasks);
}
