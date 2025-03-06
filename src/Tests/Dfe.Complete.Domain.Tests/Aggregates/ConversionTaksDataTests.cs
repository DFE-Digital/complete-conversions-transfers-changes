using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Domain.Tests.Aggregates
{
    public class ConversionTaksDataTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public void Constructor_ShouldThrowArgumentNullException_WhenConversionTaskIsDefault(TaskDataId id, DateTime updatedAt)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new ConversionTasksData(new TaskDataId(Guid.NewGuid()),
                                 default,
                                 updatedAt));

            Assert.Equal("createdAt", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public void Constructor_ShouldThrowArgumentNullException_WhenConversionTaskUpdatedAtIsDefault(TaskDataId id, DateTime createdAt)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new ConversionTasksData(id, createdAt, default));

            Assert.Equal("updatedAt", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization))]
        public void Constructor_Should_ShouldCorrectlySetFields(TaskDataId id, DateTime createdAt, DateTime updatedAt)
        {
            // Act & Assert
            var conversionTask = new ConversionTasksData(id, createdAt, updatedAt);

            Assert.Equal(id, conversionTask.Id);
            Assert.Equal(createdAt, conversionTask.CreatedAt);
            Assert.Equal(updatedAt, conversionTask.UpdatedAt);
        }
    }
}
