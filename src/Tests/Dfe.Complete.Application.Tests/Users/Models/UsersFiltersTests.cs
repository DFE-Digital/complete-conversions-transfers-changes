using Dfe.Complete.Application.Users.Models;

namespace Dfe.Complete.Application.Tests.Users.Models
{
    public class UsersFiltersTests
    {
        [Fact]
        public void Can_Construct_UsersFilters_With_Values()
        {
            var filters = new UsersFilters("user@example.com", "TeamA");

            Assert.Equal("user@example.com", filters.Email);
            Assert.Equal("TeamA", filters.Team);
        }

        [Fact]
        public void UsersFilters_Defaults_Are_Null()
        {
            var filters = new UsersFilters();

            Assert.Null(filters.Email);
            Assert.Null(filters.Team);
        }
    }
}
