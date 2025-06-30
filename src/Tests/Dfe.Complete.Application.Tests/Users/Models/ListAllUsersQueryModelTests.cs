using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
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

        [Fact]
        public void ListAllUsersQueryModel_Equality_Works()
        {
            var user1 = new User { Id = new UserId(Guid.NewGuid()), Email = "a@b.com" };
            var user2 = new User { Id = user1.Id, Email = "a@b.com" };

            var model1 = new ListAllUsersQueryModel(user1);
            var model2 = new ListAllUsersQueryModel(user2);

            Assert.Equal(model1, model2);
        }
    } 
}