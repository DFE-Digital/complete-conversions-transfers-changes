using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Tests.Users.Models
{
    public class OrderUserQueryByTests
    {
        [Fact]
        public void Can_Construct_OrderUserQueryBy_With_Values()
        {
            var order = new OrderUserQueryBy(OrderUserByField.Email, OrderByDirection.Descending);

            Assert.Equal(OrderUserByField.Email, order.Field);
            Assert.Equal(OrderByDirection.Descending, order.Direction);
        }

        [Fact]
        public void OrderUserQueryBy_Defaults()
        {
            var order = new OrderUserQueryBy();

            Assert.Equal(OrderUserByField.Team, order.Field);
            Assert.Equal(OrderByDirection.Ascending, order.Direction);
        }
    }
}
