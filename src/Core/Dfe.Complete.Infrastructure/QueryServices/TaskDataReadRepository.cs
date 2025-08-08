using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    internal class TaskDataReadRepository(CompleteContext ctx) : ITaskDataReadRepository
    {
        public IQueryable<ConversionTasksData> ConversionTaskData =>
            ctx.ConversionTasksData
                .AsNoTracking();
        public IQueryable<TransferTasksData> TransferTaskData =>
            ctx.TransferTasksData
                .AsNoTracking();

    }
}
