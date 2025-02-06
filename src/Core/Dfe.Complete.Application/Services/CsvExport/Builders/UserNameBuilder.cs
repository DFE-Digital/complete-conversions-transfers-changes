using Dfe.Complete.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
