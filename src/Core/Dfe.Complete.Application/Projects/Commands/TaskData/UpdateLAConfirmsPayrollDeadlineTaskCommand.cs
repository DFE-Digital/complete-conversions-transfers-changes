using System;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public class UpdateLAConfirmsPayrollDeadlineTaskCommand
    {
        public Guid TaskId { get; }
        public DateTime Deadline { get; }

        public UpdateLAConfirmsPayrollDeadlineTaskCommand(Guid taskId, DateTime deadline)
        {
            TaskId = taskId;
            Deadline = deadline;
        }
    }
}