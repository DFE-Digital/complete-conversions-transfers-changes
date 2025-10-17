using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ArticlesOfAssociationTask
{
    public class ArticlesOfAssociationTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ArticlesOfAssociationTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ArticleOfAssociation)
    {
        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }
        [BindProperty(Name = "sent")]
        public bool? Sent { get; set; }
        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }
        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        [ProjectType]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            if (Project.Type == ProjectType.Transfer)
            {
                Cleared = TransferTaskData.ArticlesOfAssociationCleared;
                NotApplicable = TransferTaskData.ArticlesOfAssociationNotApplicable;
                Received = TransferTaskData.ArticlesOfAssociationReceived;
                Sent = TransferTaskData.ArticlesOfAssociationSent;
                Signed = TransferTaskData.ArticlesOfAssociationSigned;
                Saved = TransferTaskData.ArticlesOfAssociationSaved;
            }
            else
            {
                Cleared = ConversionTaskData.ArticlesOfAssociationCleared;
                NotApplicable = ConversionTaskData.ArticlesOfAssociationNotApplicable;
                Received = ConversionTaskData.ArticlesOfAssociationReceived;
                Sent = ConversionTaskData.ArticlesOfAssociationSent;
                Signed = ConversionTaskData.ArticlesOfAssociationSigned;
                Saved = ConversionTaskData.ArticlesOfAssociationSaved;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateArticleOfAssociationTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, NotApplicable, Cleared, Received, Sent, Signed, Saved));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
