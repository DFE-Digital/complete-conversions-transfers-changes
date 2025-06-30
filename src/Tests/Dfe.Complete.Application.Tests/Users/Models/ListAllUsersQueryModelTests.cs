using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities; 
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Tests.Users.Models
{
    public class ListAllUsersQueryModelTests
    {
        [Fact]
        public void Can_Construct_ListAllUsersQueryModel()
        {
            var user = new User { Id = new UserId(Guid.NewGuid()), Email = "test@example.com", Team = "TeamA" };
            var model = new ListAllUsersQueryModel(user);

            Assert.Equal(user, model.User);
        }
    } 
}