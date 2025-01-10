using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Domain.Enums
{
    public enum ProjectType
    {
        [Description("Conversion::TasksData")]
        Conversion = 1,
        [Description("Transfer::TasksData")]
        Transfer = 2
    }
}
