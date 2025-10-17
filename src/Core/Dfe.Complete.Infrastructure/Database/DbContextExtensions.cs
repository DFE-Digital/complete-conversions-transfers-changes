using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.Database
{
    public static class DbContextExtensions
    {
        public static DbContextOptionsBuilder UseCompleteSqlServer(
            this DbContextOptionsBuilder optionsBuilder,
            string connectionString,
            bool enableRetryOnFailure = false,
            bool enableSensitiveDataLogging = false)
        {
            optionsBuilder.UseSqlServer(
                connectionString,
                opt =>
                {
                    if (enableRetryOnFailure)
                    {
                        opt.EnableRetryOnFailure(
                            maxRetryCount: 2,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    }
                });

            if (enableSensitiveDataLogging)
                optionsBuilder.EnableSensitiveDataLogging();

            return optionsBuilder;
        }
    }
}
