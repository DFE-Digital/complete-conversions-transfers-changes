using Microsoft.EntityFrameworkCore;
using Moq;

namespace Dfe.Complete.Tests.Common.Mocks;

public static class MockDbSet<T> where T : class
{
    public static Mock<DbSet<T>> Create(IEnumerable<T> data)
    {
        var queryable = data.AsQueryable();
        var mock = new Mock<DbSet<T>>();
        mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        return mock;
    }
}
