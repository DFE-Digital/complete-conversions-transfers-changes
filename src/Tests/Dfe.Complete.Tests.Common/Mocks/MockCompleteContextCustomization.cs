using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Moq;

namespace Dfe.Complete.Tests.Common.Mocks;

public class MockCompleteContext
{
    public static Mock<CompleteContext> Create(IEnumerable<Project>? projects = null, IEnumerable<GiasEstablishment>? giasEstablishments = null)
    {
        var mock = new Mock<CompleteContext>();

        if (projects != null)
        {
            var projectsMock = MockDbSet<Project>.Create(projects);
            mock.Setup(m => m.Projects).Returns(projectsMock.Object);
        }

        if (giasEstablishments != null)
        {
            var giasEstablishmentsMock = MockDbSet<GiasEstablishment>.Create(giasEstablishments);
            mock.Setup(m => m.GiasEstablishments).Returns(giasEstablishmentsMock.Object);
        }

        return mock;
    }
}
