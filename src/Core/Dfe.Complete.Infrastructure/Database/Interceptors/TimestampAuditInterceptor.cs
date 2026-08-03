using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dfe.Complete.Infrastructure.Database.Interceptors
{
    public class TimestampAuditInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,InterceptionResult<int> result)
        {
            UpdateTimestamps(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateTimestamps(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void UpdateTimestamps(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker
                .Entries()
                .Where(entry => entry.State is EntityState.Modified);

            foreach (var entry in entries)
            {
                SetUpdatedAt(entry, DateTime.UtcNow);
            }
        }

        private static void SetUpdatedAt(EntityEntry entry, DateTime now)
        {
            var updatedAtProperty = entry.Properties
                .FirstOrDefault(property =>
                    property.Metadata.Name == nameof(Domain.Entities.SignificantChangeProject.UpdatedAt)
                    && property.Metadata.ClrType == typeof(DateTime));

            updatedAtProperty?.CurrentValue = now;
        }
    }
}