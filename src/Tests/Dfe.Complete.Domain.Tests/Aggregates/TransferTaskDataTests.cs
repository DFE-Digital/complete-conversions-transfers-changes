using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Domain.Tests.Aggregates
{
    public class TransferTaskDataTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public void Constructor_ShouldThrowArgumentNullException_WhenTransferTaskIsDefault(DateTime updatedAt)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new TransferTasksData(new TaskDataId(Guid.NewGuid()),
                                 default,
                                 updatedAt,
                                 false,
                                 false,
                                 false));

            Assert.Equal("createdAt", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public void Constructor_ShouldThrowArgumentNullException_WhenTransferTaskUpdatedAtIsDefault(TaskDataId id, DateTime createdAt)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new TransferTasksData(id, createdAt, default, false, false, false));

            Assert.Equal("updatedAt", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization))]
        public void Constructor_Should_ShouldCorrectlySetFields(TaskDataId id, DateTime createdAt, DateTime updatedAt, bool inadequateOfsted, bool isDueToIssues, bool outGoingTrustWillClose)
        {
            // Act & Assert
            var conversionTask = new TransferTasksData(id, createdAt, updatedAt, inadequateOfsted, isDueToIssues, outGoingTrustWillClose);

            Assert.Equal(id, conversionTask.Id);
            Assert.Equal(createdAt, conversionTask.CreatedAt);
            Assert.Equal(updatedAt, conversionTask.UpdatedAt);
            Assert.Equal(inadequateOfsted, conversionTask.InadequateOfsted);
            Assert.Equal(isDueToIssues, conversionTask.FinancialSafeguardingGovernanceIssues);
            Assert.Equal(outGoingTrustWillClose, conversionTask.OutgoingTrustToClose);
        }
    }
}
