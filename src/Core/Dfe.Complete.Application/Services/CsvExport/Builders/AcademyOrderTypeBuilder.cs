using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class AcademyOrderTypeBuilder<T>(Func<T, Project> getProject) : IColumnBuilder<T>
    {
        public string Build(T value)
        {
            if (getProject(value).Type == ProjectType.Transfer)
                return "not applicable";

            if (getProject(value).DirectiveAcademyOrder == true)
                return "directive academy order";

            return "academy order";
        }
    }
}
