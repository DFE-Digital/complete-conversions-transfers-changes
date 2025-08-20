using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ArticlesOfAssociationTask
{
    public class ArticlesOfAssociationModel(ISender sender, IAuthorizationService authorizationService, ILogger<HandoverWithDeliveryOfficerTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.Handover)
    {
        [BindProperty(Name = "not-applicable")]
        public bool? NotApplicable { get; set; }

        [BindProperty(Name = "articles_of_association_cleared")]
        public bool? ArticlesOfAssociationCleared { get; set; }

        [BindProperty(Name = "articles_of_association_not_applicable")]
        public bool? ArticlesOfAssociationNotApplicable { get; set; }

        [BindProperty(Name = "articles_of_association_received")]
        public bool? ArticlesOfAssociationReceived { get; set; }
        [BindProperty(Name = "articles_of_association_sent")]
        public bool? ArticlesOfAssociationSent { get; set; }
        [BindProperty(Name = "articles_of_association_signed")]
        public bool? ArticlesOfAssociationSigned { get; set; }
        [BindProperty(Name = "articles_of_association_saved")]
        public bool? ArticlesOfAssociationSaved { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }
        [BindProperty]
        public ProjectType? Type { get; set; }
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            Type = Project.Type;
            TasksDataId = Project.TasksDataId?.Value;
            if (Project.Type == ProjectType.Transfer)
            {
                ArticlesOfAssociationCleared = TransferTaskData.ArticlesOfAssociationCleared;
                ArticlesOfAssociationNotApplicable = TransferTaskData.ArticlesOfAssociationNotApplicable;
                ArticlesOfAssociationReceived = TransferTaskData.ArticlesOfAssociationReceived;
                ArticlesOfAssociationSent = TransferTaskData.ArticlesOfAssociationSent;
                ArticlesOfAssociationSigned = TransferTaskData.ArticlesOfAssociationSigned;
                ArticlesOfAssociationSaved = TransferTaskData.ArticlesOfAssociationSaved;
            }
            else
            {
                ArticlesOfAssociationCleared = ConversionTaskData.ArticlesOfAssociationCleared;
                ArticlesOfAssociationNotApplicable = ConversionTaskData.ArticlesOfAssociationNotApplicable;
                ArticlesOfAssociationReceived = ConversionTaskData.ArticlesOfAssociationReceived;
                ArticlesOfAssociationSent = ConversionTaskData.ArticlesOfAssociationSent;
                ArticlesOfAssociationSigned = ConversionTaskData.ArticlesOfAssociationSigned;
                ArticlesOfAssociationSaved = ConversionTaskData.ArticlesOfAssociationSaved;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await sender.Send(new UpdateArticleOfAssociationTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, NotApplicable, ArticlesOfAssociationCleared, ArticlesOfAssociationReceived, ArticlesOfAssociationSent, ArticlesOfAssociationSigned, ArticlesOfAssociationSaved));
            TempData.SetNotification(NotificationType.Success, "Success", "Task updated successfully");
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
