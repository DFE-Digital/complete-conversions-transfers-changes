using Dfe.Complete.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding; 

namespace Dfe.Complete.Tests.Extensions
{
    public class ModelStateExtensionsTests
    {
        [Fact]
        public void RemoveError_RemovesSpecificError_AndKeepsOthers()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
             
            modelState.AddModelError("SignificantDate", "SignificantDate must include a month and year");
            modelState.AddModelError("SignificantDate", "Another error");

            // Act
            modelState.RemoveError("SignificantDate", "SignificantDate must include a month and year");

            // Assert
            var entry = modelState["SignificantDate"];
            Assert.NotNull(entry);
            var error = entry.Errors.FirstOrDefault();
            Assert.NotNull(error);
            Assert.Equal("Another error", error.ErrorMessage);

            Assert.False(modelState.IsActuallyValid());
        }

        [Fact]
        public void RemoveError_RemovesLastError_MakesModelStateValid()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("SignificantDate", "SignificantDate must include a month and year");

            // Act
            modelState.RemoveError("SignificantDate", "SignificantDate must include a month and year");

            // Assert
            var entry = modelState["SignificantDate"];
            Assert.NotNull(entry); 
            Assert.Empty(entry.Errors);
            Assert.True(modelState.IsActuallyValid());
        }

        [Fact]
        public void RemoveError_KeyDoesNotExist_DoesNothing()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("OtherField", "Some error");

            // Act
            modelState.RemoveError("SignificantDate", "SignificantDate must include a month and year");

            // Assert
            var entry = modelState["SignificantDate"];
            Assert.Null(entry); 
            Assert.False(modelState.IsActuallyValid());
        }

        [Fact]
        public void RemoveError_ErrorMessageDoesNotExist_DoesNotRemoveOtherErrors()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("SignificantDate", "Some other error");

            // Act
            modelState.RemoveError("SignificantDate", "SignificantDate must include a month and year");

            // Assert
            var entry = modelState["SignificantDate"];
            Assert.NotNull(entry);
            var error = entry.Errors.FirstOrDefault();
            Assert.NotNull(error); 
            Assert.Equal("Some other error", error.ErrorMessage);
            Assert.False(modelState.IsActuallyValid());
        }
    }
}
