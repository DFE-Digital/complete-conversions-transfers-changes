using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Pages.Projects.TaskList
{
    public static class ConversionTasks
    {
        public static (List<TaskListItemViewModel> ProjectKickoffTasks, 
            List<TaskListItemViewModel> LegalDocumentsTasks, 
            List<TaskListItemViewModel> ReadyForOpeningTasks, 
            List<TaskListItemViewModel> AfterOpeningTasks) BuildTaskList(ConversionTaskListViewModel conversionTaskList, string projectId)
        {
            TaskLinkBuilder taskLinkBuilder = new(RouteConstants.ProjectTask, projectId);

            return (GetProjectKickoffTasks(conversionTaskList, taskLinkBuilder),
                GetLegalDocumentsTasks(conversionTaskList, taskLinkBuilder),
                GetReadyForOpeningTasks(conversionTaskList, taskLinkBuilder),
                GetAfterOpeningTasks(conversionTaskList, taskLinkBuilder));
        }

        private static List<TaskListItemViewModel> GetProjectKickoffTasks(ConversionTaskListViewModel conversionTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var projectKickoffTasks = new List<TaskListItemBuildModel>
            {
                new(NoteTaskIdentifier.PostDecisionActions, conversionTaskList.PostDecisionActions, 1),
                new(NoteTaskIdentifier.Handover, conversionTaskList.HandoverWithRegionalDeliveryOfficer, 2),
                new(NoteTaskIdentifier.StakeholderKickoff, conversionTaskList.ExternalStakeHolderKickoff, 3),
                new(NoteTaskIdentifier.ConfirmRiskProtectionArrangements, conversionTaskList.ConfirmAcademyRiskProtectionArrangements, 4),
                new(NoteTaskIdentifier.CheckAccuracyOfHigherNeeds, conversionTaskList.CheckAccuracyOfHigherNeeds, 5),
                new(NoteTaskIdentifier.CompleteNotificationOfChange, conversionTaskList.CompleteNotificationOfChange, 7),
                new(NoteTaskIdentifier.ConfirmAndProcessTheSponsoredSupportGrant, conversionTaskList.ConfirmAndProcessSponsoredSupportGrant, 8),
                new(NoteTaskIdentifier.NurseryArrangement, conversionTaskList.ConfirmNurseryArrangement, 9),
                new(NoteTaskIdentifier.ConfirmChildrenCentre,  conversionTaskList.ConfirmChildrenCentre, 10),
                new(NoteTaskIdentifier.ConfirmStatutoryConsultation, conversionTaskList.ConfirmStatutoryConsultation, 11),
                new(NoteTaskIdentifier.AcademyDetails, conversionTaskList.ConfirmAcademyName, 12),
                new(NoteTaskIdentifier.ConfirmHeadTeacherDetails, conversionTaskList.ConfirmHeadTeacherDetails, 13),
                new(NoteTaskIdentifier.ConfirmChairOfGovernorsDetails, conversionTaskList.ConfirmChairOfGovernorsDetails, 14),
                new(NoteTaskIdentifier.ConfirmIncomingTrustCeoContact, conversionTaskList.ConfirmIncomingTrustCeoDetails, 15),
                new(NoteTaskIdentifier.MainContact, conversionTaskList.ConfirmMainContact, 16),
                new(NoteTaskIdentifier.ConfirmProposedCapacityOfTheAcademy, conversionTaskList.ConfirmProposedCapacityOfTheAcademy, 17),
                new(NoteTaskIdentifier.LAConfirmsPayrollDeadline, conversionTaskList.LAConfirmsPayrollDeadline, 18)
            };

            if (conversionTaskList.ShowProcessConversionSupportGrant)
            {
                projectKickoffTasks.Add(new TaskListItemBuildModel(NoteTaskIdentifier.ProcessConversionSupportGrant, conversionTaskList.ProcessConversionSupportGrant, 6));
            }

            return [.. projectKickoffTasks
                .Select(x => new TaskListItemViewModel(
                    x.Identifier.ToDisplayDescription(),
                    taskLinkBuilder.Build(x.Identifier.ToDescription()),
                    x.Status,
                    x.DisplayOrder    
                )).OrderBy(x => x.DisplayOrder)];
        }

        private static List<TaskListItemViewModel> GetLegalDocumentsTasks(ConversionTaskListViewModel conversionTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var legalDocumentsTasks = new List<TaskListItemBuildModel>
            {
                new(NoteTaskIdentifier.LandQuestionnaire, conversionTaskList.LandQuestionnaire, 1),
                new(NoteTaskIdentifier.SupplementalFundingAgreement, conversionTaskList.SupplementalFundingAgreement, 2),
                new(NoteTaskIdentifier.ChurchSupplementalAgreement, conversionTaskList.ChurchSupplementalAgreement, 3),
                new(NoteTaskIdentifier.MasterFundingAgreement, conversionTaskList.MasterFundingAgreement, 4),
                new(NoteTaskIdentifier.ArticleOfAssociation, conversionTaskList.ArticlesOfAssociation, 5),
                new(NoteTaskIdentifier.DeedOfVariation, conversionTaskList.DeedOfVariation, 6),
                new(NoteTaskIdentifier.TrustModificationOrder, conversionTaskList.TrustModificationOrder, 7),
                new(NoteTaskIdentifier.DirectionToTransfer, conversionTaskList.DirectionToTransfer, 8),
                new(NoteTaskIdentifier.OneHundredAndTwentyFiveYearLease, conversionTaskList.OneHundredAndTwentyFiveYearLease, 9),
                new(NoteTaskIdentifier.Subleases, conversionTaskList.Tubleases, 10),
                new(NoteTaskIdentifier.ThirdPartyLeases, conversionTaskList.ThirdPartyLeases, 11),
                new(NoteTaskIdentifier.TenancyAtWill, conversionTaskList.TenancyAtWill, 12),
                new(NoteTaskIdentifier.CommercialTransferAgreement, conversionTaskList.CommercialTransferAgreement, 13),
                new(NoteTaskIdentifier.PrivateFinanceInitiative, conversionTaskList.PrivateFinanceInitiative, 14)
            };

            return [.. legalDocumentsTasks
                .Select(x => new TaskListItemViewModel(
                    x.Identifier.ToDisplayDescription(),
                    taskLinkBuilder.Build(x.Identifier.ToDescription()),
                    x.Status,
                    x.DisplayOrder    
                )).OrderBy(x => x.DisplayOrder)];
        }

        private static List<TaskListItemViewModel> GetReadyForOpeningTasks(ConversionTaskListViewModel conversionTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var readyForOpeningTasks = new List<TaskListItemBuildModel>
            {
                new(NoteTaskIdentifier.ConfirmSchoolBankDetails, conversionTaskList.ConfirmSchoolBankDetails, 1),
                new(NoteTaskIdentifier.ConfirmSchoolHasCompletedAllActions, conversionTaskList.ConfirmTheSchoolHasCompletedAllActions, 2),
                new(NoteTaskIdentifier.ConfirmAllConditionsMet, conversionTaskList.ConfirmAllConditionsHaveBeenMet, 3),
                new(NoteTaskIdentifier.TupeConsultation, conversionTaskList.TupeConsultation, 4),
                new(NoteTaskIdentifier.ConfirmDBSChecks, conversionTaskList.ConfirmDbsChecks, 5)
            };

            return [.. readyForOpeningTasks
                .Select(x => new TaskListItemViewModel(
                    x.Identifier.ToDisplayDescription(),
                    taskLinkBuilder.Build(x.Identifier.ToDescription()),
                    x.Status,
                    x.DisplayOrder    
                )).OrderBy(x => x.DisplayOrder)];
        }

        private static List<TaskListItemViewModel> GetAfterOpeningTasks(ConversionTaskListViewModel conversionTaskList, TaskLinkBuilder taskLinkBuilder)
        {
            var afterOpeningTasks = new List<TaskListItemBuildModel>
            {
                new (NoteTaskIdentifier.ConfirmAcademyOpenedDate, conversionTaskList.ConfirmDateAcademyOpened, 1),
                new (NoteTaskIdentifier.RedactAndSend, conversionTaskList.RedactAndSendDocuments, 2),
                new (NoteTaskIdentifier.ReceiveGrantPaymentCertificate, conversionTaskList.ProjectReceiveDeclarationOfExpenditureCertificate, 3)
            };

            return [.. afterOpeningTasks
                .Select(x => new TaskListItemViewModel(
                    x.Identifier.ToDisplayDescription(),
                    taskLinkBuilder.Build(x.Identifier.ToDescription()),
                    x.Status,
                    x.DisplayOrder    
                )).OrderBy(x => x.DisplayOrder)];
        }
    }
}