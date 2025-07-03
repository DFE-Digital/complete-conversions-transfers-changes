using MediatR;
using System;

namespace Dfe.Complete.Application.Projects.Commands.ConversionTasks
{
    public record UpdateConversionHandoverTaskCommand(
        Guid ProjectId,
        bool NotApplicable,
        bool ReviewProjectInformation,
        bool MakeNotes,
        bool AttendHandoverMeeting
    ) : IRequest<UpdateConversionHandoverTaskResult>;

    public class UpdateConversionHandoverTaskResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
