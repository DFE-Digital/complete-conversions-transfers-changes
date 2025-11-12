using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Complete.Tests.Extensions
{
    public class TempDataExtensionsTests
    {
        [Fact]
        public void SetNotification_SetsExpectedValues()
        {
            // Arrange
            var tempDataMock = new Mock<ITempDataDictionary>();
            var tempData = tempDataMock.Object;

            // Act
            tempData.SetNotification(NotificationType.Success, "TestTitle", "TestMessage");

            // Assert
            tempDataMock.VerifySet(td => td["NotificationType"] = "success");
            tempDataMock.VerifySet(td => td["NotificationTitle"] = "TestTitle");
            tempDataMock.VerifySet(td => td["NotificationMessage"] = "TestMessage");
        }
    }
}
