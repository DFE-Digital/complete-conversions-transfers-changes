using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Dfe.Complete.Tests.Extensions
{
    public class TempDataExtensionsTests
    {
        [Fact]
        public void SetTaskSuccessNotification_SetsSuccessNotification()
        {
            // Arrange
            var tempDataMock = new Mock<ITempDataDictionary>();
            var tempData = tempDataMock.Object;

            // Act
            tempData.SetTaskSuccessNotification();

            // Assert
            tempDataMock.VerifySet(td => td["NotificationType"] = "success");
            tempDataMock.VerifySet(td => td["NotificationTitle"] = "Success");
            tempDataMock.VerifySet(td => td["NotificationMessage"] = "Task updated successfully");
        }
    }
}
