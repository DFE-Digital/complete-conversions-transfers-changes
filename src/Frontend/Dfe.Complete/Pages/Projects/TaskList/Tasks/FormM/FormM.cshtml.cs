using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dfe.Complete.Application.Projects.Models.TransferTasks;
using Dfe.Complete.Application.Projects.Queries.TransferTasks;
using Dfe.Complete.Application.Projects.Commands.TransferTasks;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.FormM
{
    public class FormMModel : PageModel
    {
        private readonly IMediator _mediator;

        public TransferFormMTaskDataDto? FormMData { get; set; }

        [BindProperty]
        public TransferFormMTaskDataDto? Input { get; set; }

        public FormMModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync(long projectId)
        {
            // Only allow for transfer projects (add check as needed)
            var data = await _mediator.Send(new GetTransferFormMTaskDataByProjectIdQuery(projectId));
            if (data == null)
            {
                return NotFound();
            }
            FormMData = data;
            Input = data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long projectId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Input != null)
            {
                await _mediator.Send(new UpdateTransferFormMTaskDataByProjectIdCommand(projectId, Input));
            }
            return RedirectToPage("../TaskList", new { projectId });
        }
    }
}
