using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class FormAMat<T>(Func<T, Project> selector) : IColumnBuilder<T>
    {
        public string Build(T input)
        {
            var project = selector(input);

            if (project.IncomingTrustUkprn != null)
            {
                return "join a MAT";
            }
            else
            {
                return "form a MAT";
            }
        }
    }
}
