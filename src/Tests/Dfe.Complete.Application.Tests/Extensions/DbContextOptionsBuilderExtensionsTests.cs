using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Tests.Extensions
{
    public class DbContextOptionsBuilderExtensionsTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void UseCompleteSqlServer_ShouldConfigureSqlServer(bool enableRetryOnFailure)
        {
            // Arrange  
            var builder = new DbContextOptionsBuilder();
            var connectionString = "connectionstring";

            // Act  
            builder.UseCompleteSqlServer(connectionString, enableRetryOnFailure);

            // Assert  
            var options = builder.Options;
            Assert.NotNull(options);
            var sqlServerOptionExtension = options.Extensions.SingleOrDefault(e => e.GetType().Name == "SqlServerOptionsExtension");
            Assert.NotNull(sqlServerOptionExtension);
        }
    }
}
