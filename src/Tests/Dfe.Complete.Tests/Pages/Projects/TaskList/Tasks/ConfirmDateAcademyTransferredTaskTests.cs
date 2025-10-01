using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmDateAcademyTransferredTask;

namespace Dfe.Complete.Tests.Pages.Projects.TaskList.Tasks
{
    public class ConfirmDateAcademyTransferredTaskTests
    {
        // Helper method stays at class level
        private static void ValidatePastOnly(ConfirmDateAcademyTransferredTaskModel model, DateOnly? date)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (date is null) return;

            if (date.Value >= today)
            {
                model.ModelState.AddModelError(
                    nameof(model.DateAcademyTransferred),
                    string.Format(ValidationConstants.DateInPast, "Academy transferred date"));
            }
        }

        public class PastOnlyDateValidation
        {
            [Fact]
            public void Should_Error_WhenDateIsInFuture()
            {
                // Arrange
                var model = new ConfirmDateAcademyTransferredTaskModel(null!, null!, null!, null!)
                {
                    TaskIdentifier = NoteTaskIdentifier.ConfirmDateAcademyTransferred
                };
                var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

                // Act
                ValidatePastOnly(model, futureDate);

                // Assert
                Assert.True(model.ModelState.ContainsKey(nameof(model.DateAcademyTransferred)));
                var error = model.ModelState[nameof(model.DateAcademyTransferred)]?.Errors.FirstOrDefault();
                Assert.NotNull(error);
                Assert.Equal(string.Format(ValidationConstants.DateInPast, "Academy transferred date"), error!.ErrorMessage);
            }

            [Fact]
            public void Should_Error_WhenDateIsToday()
            {
                // Arrange
                var model = new ConfirmDateAcademyTransferredTaskModel(null!, null!, null!, null!)
                {
                    TaskIdentifier = NoteTaskIdentifier.ConfirmDateAcademyTransferred
                };
                var todayDate = DateOnly.FromDateTime(DateTime.Today);

                // Act
                ValidatePastOnly(model, todayDate);

                // Assert
                Assert.True(model.ModelState.ContainsKey(nameof(model.DateAcademyTransferred)));
                var error = model.ModelState[nameof(model.DateAcademyTransferred)]?.Errors.FirstOrDefault();
                Assert.NotNull(error);
                Assert.Equal(string.Format(ValidationConstants.DateInPast, "Academy transferred date"), error!.ErrorMessage);
            }

            [Fact]
            public void ShouldNot_Error_WhenDateIsInPast()
            {
                // Arrange
                var model = new ConfirmDateAcademyTransferredTaskModel(null!, null!, null!, null!)
                {
                    TaskIdentifier = NoteTaskIdentifier.ConfirmDateAcademyTransferred
                };
                var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

                // Act
                ValidatePastOnly(model, pastDate);

                // Assert
                Assert.False(model.ModelState.ContainsKey(nameof(model.DateAcademyTransferred)));
                Assert.True(model.ModelState.IsValid);
            }
        }
    }
}
