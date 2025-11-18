using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmSponsoredSupportGrantTask
{
    public class ConfirmTransferGrantFundingLevelTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmTransferGrantFundingLevelTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmTransferGrantFundingLevel)
    {
        private readonly ISender _sender = sender;

        [BindProperty(Name = "notapplicable")]
        public bool? NotApplicable { get; set; }
        
        public IEnumerable<SponsoredSupportGrantType> SponsoredSupportGrantTypes { get; set; } = new List<SponsoredSupportGrantType>();

        [BindProperty(Name = "sponsored_support_grant_type")]
        public string? SponsoredSupportGrantType { get; set; } 
        
        [BindProperty(Name = "paymentAmount")]
        public bool? PaymentAmount { get; set; }
        
        [BindProperty(Name = "paymentForm")]
        public bool? PaymentForm { get; set; }

        [BindProperty(Name = "sendInformation")]
        public bool? SendInformation { get; set; }
        
        [BindProperty(Name = "informTrust")]
        public bool? InformTrust { get; set; }

        [BindProperty]
        public Guid? TasksDataId { get; set; }

        [BindProperty]
        public ProjectType? Type { get; set; }
 
        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();
            TasksDataId = Project.TasksDataId?.Value;
            Type = Project.Type;
            
            SponsoredSupportGrantTypes = EnumExtensions.ToList<SponsoredSupportGrantType>();
            
            if (Type == ProjectType.Transfer)
            {
                NotApplicable = TransferTaskData.SponsoredSupportGrantNotApplicable;
                SponsoredSupportGrantType = TransferTaskData.SponsoredSupportGrantType;
            }
            else
            {
                TaskIdentifier = NoteTaskIdentifier.ConfirmAndProcessTheSponsoredSupportGrant;
                SponsoredSupportGrantTypes = SponsoredSupportGrantTypes.Where(x =>
                    x != Domain.Enums.SponsoredSupportGrantType.StandardTransferGrant);

                NotApplicable = ConversionTaskData.SponsoredSupportGrantNotApplicable;
                SponsoredSupportGrantType = ConversionTaskData.SponsoredSupportGrantType;
                
                PaymentAmount = ConversionTaskData.SponsoredSupportGrantPaymentAmount;
                PaymentForm = ConversionTaskData.SponsoredSupportGrantPaymentForm;
                SendInformation = ConversionTaskData.SponsoredSupportGrantSendInformation;
                InformTrust = ConversionTaskData.SponsoredSupportGrantInformTrust;
            }
            
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var sponsoredSupportGrantType = SponsoredSupportGrantType == null ? null : EnumExtensions.FromDescriptionValue<SponsoredSupportGrantType>(SponsoredSupportGrantType);
            await _sender.Send(new UpdateConfirmSponsoredSupportGrantTaskCommand(new TaskDataId(TasksDataId.GetValueOrDefault())!, Type, NotApplicable, sponsoredSupportGrantType, PaymentAmount, PaymentForm, SendInformation, InformTrust));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
