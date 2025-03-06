using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class UserNameBuilder<T>(Func<T, User> selector): IColumnBuilder<T>
    {
        public string Build(T model)
        {
            var user = selector(model);
            return user == null ? string.Empty : $"{user.FirstName} {user.LastName}";
        }
    }
}
