using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    internal class ProjectReadRepository(CompleteContext ctx) : IProjectReadRepository
    {
        public IQueryable<Project> Projects =>
            ctx.Projects
                .AsNoTracking()
                .Include(p => p.RegionalDeliveryOfficer)
                .Include(p => p.LocalAuthority)
                .Include(p => p.SignificantDateHistories)
                .Include(p => p.GiasEstablishment);

    }
}
