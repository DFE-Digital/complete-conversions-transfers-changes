using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Dfe.Complete.Api.Tests.Integration.Commands
{
    public class UpdateLAConfirmsPayrollDeadlineTaskCommandTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public UpdateLAConfirmsPayrollDeadlineTaskCommandTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Should_Update_Payroll_Deadline_Task_Successfully()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var tasksDataId = Guid.NewGuid();
            var payrollDeadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

            var command = new UpdateLAConfirmsPayrollDeadlineTaskCommand(new TaskDataId(tasksDataId), payrollDeadline);

            // Act
            var result = await _fixture.SendAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.PayrollDeadline.Should().Be(payrollDeadline);
        }
    }
}