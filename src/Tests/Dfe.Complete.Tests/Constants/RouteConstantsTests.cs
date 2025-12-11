using Dfe.Complete.Constants;

namespace Dfe.Complete.Tests.Constants
{
    public class RouteConstantsTests
    {
        [Fact]
        public void ProjectsInProgress_ShouldBeCorrect()
        {
            Assert.Equal("/projects/all/in-progress/all", RouteConstants.ProjectsInProgress);
        }

        [Fact]
        public void ProjectsByTrust_ShouldBeCorrect()
        {
            Assert.Equal("/projects/all/trusts", RouteConstants.ProjectsByTrust);
        }

        [Fact]
        public void TrustProjects_ShouldFormatCorrectly()
        {
            var ukprn = "123456";
            var expected = $"/projects/all/trusts/ukprn/{ukprn}";
            Assert.Equal(expected, string.Format(RouteConstants.TrustProjects, ukprn));
        }

        [Fact]
        public void TrustMATProjects_ShouldFormatCorrectly()
        {
            var reference = "MAT123";
            var expected = $"/projects/all/trusts/reference/{reference}";
            Assert.Equal(expected, string.Format(RouteConstants.TrustMATProjects, reference));
        }

        [Fact]
        public void ConversionProjectsByMonth_ShouldFormatCorrectly()
        {
            var year = "2025";
            var month = "05";
            var expected = $"/projects/all/by-month/conversions/{year}/{month}";
            Assert.Equal(expected, string.Format(RouteConstants.ConversionProjectsByMonth, year, month));
        }

        [Fact]
        public void TransfersProjectsByMonth_ShouldFormatCorrectly()
        {
            var year = "2025";
            var month = "06";
            var expected = $"/projects/all/by-month/transfers/{year}/{month}";
            Assert.Equal(expected, string.Format(RouteConstants.TransfersProjectsByMonth, year, month));
        }

        [Fact]
        public void ProjectsByRegion_ShouldFormatCorrectly()
        {
            var expected = $"/projects/all/regions";
            Assert.Equal(expected, string.Format(RouteConstants.ProjectsByRegion));
        }

        [Fact]
        public void ProjectsForRegion_ShouldFormatCorrectly()
        {
            var region = "north-west";
            var expected = $"/projects/all/regions/{region}";
            Assert.Equal(expected, string.Format(RouteConstants.ProjectsForRegion, region));
        }

        [Fact]
        public void CompletedProjects_ShouldBeCorrect()
        {
            Assert.Equal("/projects/all/completed", RouteConstants.CompletedProjects);
        }

        [Fact]
        public void YourProjectsInProgress_ShouldBeCorrect()
        {
            Assert.Equal("/projects/yours/in-progress", RouteConstants.YourProjectsInProgress);
        }

        [Fact]
        public void YourProjectsCompleted_ShouldBeCorrect()
        {
            Assert.Equal("/projects/yours/completed", RouteConstants.YourProjectsCompleted);
        }

        [Fact]
        public void TeamProjectsRoutes_ShouldBeCorrect()
        {
            Assert.Equal("/projects/team/in-progress", RouteConstants.TeamProjectsInProgress);
            Assert.Equal("/projects/team/new", RouteConstants.TeamProjectsNew);
            Assert.Equal("/projects/team/users", RouteConstants.TeamProjectsUsers);
            Assert.Equal("/projects/team/handed-over", RouteConstants.TeamProjectsHandedOver);
            Assert.Equal("/projects/team/completed", RouteConstants.TeamProjectsCompleted);
            Assert.Equal("/projects/team/unassigned", RouteConstants.TeamProjectsUnassigned);
        }

        [Fact]
        public void Project_ShouldFormatCorrectly()
        {
            var projectId = "abc123";
            var expected = $"/projects/{projectId}";
            Assert.Equal(expected, string.Format(RouteConstants.Project, projectId));
        }

        [Fact]
        public void CreateNewProject_ShouldBeCorrect()
        {
            Assert.Equal("/projects/CreateNewProject", RouteConstants.CreateNewProject);
        }

        [Fact]
        public void SelectCreateProjectType_ShouldBeCorrect()
        {
            Assert.Equal("/projects/new", RouteConstants.SelectCreateProjectType);
        }

        [Fact]
        public void ProjectNoteRoutes_ShouldFormatCorrectly()
        {
            var projectId = Guid.NewGuid();
            var noteId = "n1";
            var notesBase = $"/projects/{projectId}/notes";
            Assert.Equal(notesBase, string.Format(RouteConstants.ProjectViewNotes, projectId));
            Assert.Equal(notesBase + "/new", string.Format(RouteConstants.ProjectAddNote, projectId));
            Assert.Equal($"{notesBase}/{noteId}/edit", string.Format(RouteConstants.ProjectEditNote, projectId, noteId));
        }

        [Fact]
        public void ProjectTasksRoutes_ShouldFormatCorrectly()
        {
            var projectId = Guid.NewGuid();
            var basePath = $"/projects/{projectId}/tasks";

            Assert.Equal(basePath, string.Format(RouteConstants.ProjectTaskList, projectId));
            Assert.Equal(basePath + "/handover", string.Format(RouteConstants.ProjectHandoverWithDeliveryOfficerTask, projectId));
            Assert.Equal(basePath + "/stakeholder_kick_off", string.Format(RouteConstants.ProjectStakeholderKickoffTask, projectId));
            Assert.Equal(basePath + "/land_questionnaire", string.Format(RouteConstants.ProjectLandQuestionnaireTask, projectId));
            Assert.Equal(basePath + "/land_registry", string.Format(RouteConstants.ProjectLandRegistryTask, projectId));
            Assert.Equal(basePath + "/supplemental_funding_agreement", string.Format(RouteConstants.ProjectSupplementalFundingAgreementTask, projectId));
            Assert.Equal(basePath + "/rpa_policy", string.Format(RouteConstants.ProjectRiskProtectionArrangementPolicyTask, projectId));
            Assert.Equal(basePath + "/risk_protection_arrangement", string.Format(RouteConstants.ProjectRiskProtectionArrangementTask, projectId));
            Assert.Equal(basePath + "/check_accuracy_of_higher_needs", string.Format(RouteConstants.ProjectCheckAccuracyOfHigherNeedsTask, projectId));
            Assert.Equal(basePath + "/complete_notification_of_change", string.Format(RouteConstants.ProjectCompleteNotificationOfChangeTask, projectId));
            Assert.Equal(basePath + "/conversion_grant", string.Format(RouteConstants.ProjectProcessConversionSupportGrantTask, projectId));
            Assert.Equal(basePath + "/sponsored_support_grant", string.Format(RouteConstants.ProjectConfirmAndProcessSponsoredSupportGrantTask, projectId));
            Assert.Equal(basePath + "/academy_details", string.Format(RouteConstants.ProjectConfirmAcademyNameTask, projectId));
            Assert.Equal(basePath + "/confirm_headteacher_contact", string.Format(RouteConstants.ProjectConfirmHeadTeacherDetailsTask, projectId));
            Assert.Equal(basePath + "/confirm_chair_of_governors_contact", string.Format(RouteConstants.ProjectConfirmChairOfGovernorsDetailsTask, projectId));
            Assert.Equal(basePath + "/confirm_incoming_trust_ceo_contact", string.Format(RouteConstants.ProjectConfirmIncomingTrustCeoDetailsTask, projectId));
            Assert.Equal(basePath + "/main_contact", string.Format(RouteConstants.ProjectConfirmMainContactTask, projectId));
            Assert.Equal(basePath + "/proposed_capacity_of_the_academy", string.Format(RouteConstants.ProjectConfirmProposedCapacityOfTheAcademyTask, projectId));
            Assert.Equal(basePath + "/articles_of_association", string.Format(RouteConstants.ProjectArticlesOfAssociationTask, projectId));
            Assert.Equal(basePath + "/deed_of_variation", string.Format(RouteConstants.ProjectDeedOfVariationTask, projectId));
            Assert.Equal(basePath + "/trust_modification_order", string.Format(RouteConstants.ProjectTrustModificationOrderTask, projectId));
            Assert.Equal(basePath + "/direction_to_transfer", string.Format(RouteConstants.ProjectDirectionToTransferTask, projectId));
            Assert.Equal(basePath + "/one_hundred_and_twenty_five_year_lease", string.Format(RouteConstants.ProjectOneHundredAndTwentyFiveYearLeaseTask, projectId));
            Assert.Equal(basePath + "/subleases", string.Format(RouteConstants.ProjectSubleasesTask, projectId));
            Assert.Equal(basePath + "/tenancy_at_will", string.Format(RouteConstants.ProjectTenancyAtWillTask, projectId));
            Assert.Equal(basePath + "/commercial_transfer_agreement", string.Format(RouteConstants.ProjectCommercialTransferAgreementTask, projectId));
            Assert.Equal(basePath + "/school_completed", string.Format(RouteConstants.ProjectConfirmTheSchoolHasCompletedAllActionsTask, projectId));
            Assert.Equal(basePath + "/conditions_met", string.Format(RouteConstants.ProjectConfirmAllConditionsHaveBeenMetTask, projectId));
            Assert.Equal(basePath + "/share_information", string.Format(RouteConstants.ProjectShareTheInformationAboutOpeningTask, projectId));
            Assert.Equal(basePath + "/confirm_date_academy_opened", string.Format(RouteConstants.ProjectConfirmDateAcademyOpenedTask, projectId));
            Assert.Equal(basePath + "/redact_and_send", string.Format(RouteConstants.ProjectRedactAndSendTask, projectId));
            Assert.Equal(basePath + "/receive_grant_payment_certificate", string.Format(RouteConstants.ProjectReceiveDeclarationOfExpenditureCertificateTask, projectId));
            Assert.Equal(basePath + "/confirm_outgoing_trust_ceo_contact", string.Format(RouteConstants.ProjectConfirmOutingTrustCeoDetailsTask, projectId));
            Assert.Equal(basePath + "/request_new_urn_and_record", string.Format(RouteConstants.ProjectRequestNewURNAndRecordForTheAcademyTask, projectId));
            Assert.Equal(basePath + "/check_and_confirm_financial_information", string.Format(RouteConstants.ProjectCheckAndConfirmAcademyAndTrustFinancialInfoTask, projectId));
            Assert.Equal(basePath + "/confirm_new_urn_and_record", string.Format(RouteConstants.ProjectConfirmTransferGrantFundingLevelTask, projectId));

            Assert.Equal(basePath + "/form_m", string.Format(RouteConstants.ProjectFormMTask, projectId));
            Assert.Equal(basePath + "/land_consent_letter", string.Format(RouteConstants.ProjectLandConsentLetterTask, projectId));
            Assert.Equal(basePath + "/deed_of_novation_and_variation", string.Format(RouteConstants.ProjectDeedOfNovationAndVariationTask, projectId));
            Assert.Equal(basePath + "/church_supplemental_agreement", string.Format(RouteConstants.ProjectChurchSupplementalAgreementTask, projectId));
            Assert.Equal(basePath + "/master_funding_agreement", string.Format(RouteConstants.ProjectMasterFundingAgreementTask, projectId));
            Assert.Equal(basePath + "/deed_of_termination_for_the_master_funding_agreement", string.Format(RouteConstants.ProjectDeedOfTerminationForTheMasterFundingAgreementTask, projectId));
            Assert.Equal(basePath + "/deed_termination_church_agreement", string.Format(RouteConstants.ProjectDeedOfTerminationForChurchSupplementalAreementTask, projectId));
            Assert.Equal(basePath + "/closure_or_transfer_declaration", string.Format(RouteConstants.ProjectClosureOrTransferDeclarationTask, projectId));
            Assert.Equal(basePath + "/bank_details_changing", string.Format(RouteConstants.ProjectConfirmBankDetailsChangingForGeneralAnnualGrantPaymentTask, projectId));
            Assert.Equal(basePath + "/confirm_incoming_trust_has_completed_all_actions", string.Format(RouteConstants.ProjectConfirmIncomingTrustHasCompletedAllActionsTask, projectId));
            Assert.Equal(basePath + "/confirm_date_academy_transferred", string.Format(RouteConstants.ProjectConfirmDateAcademyTransferredTask, projectId));
            Assert.Equal(basePath + "/redact_and_send_documents", string.Format(RouteConstants.ProjectRedactAndSendDocumentsTask, projectId));
            Assert.Equal(basePath + "/declaration_of_expenditure_certificate", string.Format(RouteConstants.ProjectDeclarationOfExpenditureCertificateTask, projectId));
        }
        [Fact]
        public void TaskPageRoutes_ShouldFormatCorrectly()
        {
            var projectId = Guid.NewGuid();
            var projectPath = $"/projects/{projectId}";
            Assert.Equal(projectPath + "/complete", string.Format(RouteConstants.ProjectComplete, projectId));
            Assert.Equal(projectPath + "/dao-revocation", string.Format(RouteConstants.ProjectDaoRevocation, projectId));
        }

        [Fact]
        public void ProjectDaoRevocationRoutes_ShouldFormatCorrectly()
        {
            var projectId = Guid.NewGuid();
            var basePath = $"/projects/{projectId}/dao-revocation";

            Assert.Equal(basePath, string.Format(RouteConstants.ProjectDaoRevocation, projectId));
            Assert.Equal(basePath + "/confirm", string.Format(RouteConstants.ProjectDaoRevocationConfirm, projectId));
            Assert.Equal(basePath + "/reasons", string.Format(RouteConstants.ProjectDaoRevocationReason, projectId));
            Assert.Equal(basePath + "/minister", string.Format(RouteConstants.ProjectDaoRevocationMinister, projectId));
            Assert.Equal(basePath + "/date", string.Format(RouteConstants.ProjectDaoRevocationDate, projectId));
            Assert.Equal(basePath + "/check", string.Format(RouteConstants.ProjectDaoRevocationCheck, projectId));

            Assert.Equal(basePath + "/reasons/change", string.Format(RouteConstants.ChangeProjectDaoRevocationReason, projectId));
            Assert.Equal(basePath + "/minister/change", string.Format(RouteConstants.ChangeProjectDaoRevocationMinister, projectId));
            Assert.Equal(basePath + "/date/change", string.Format(RouteConstants.ChangeProjectDaoRevocationDate, projectId));
        }

        [Fact]
        public void ProjectAboutRoutes_ShouldFormatCorrectly()
        {
            var projectId = "proj99";
            var about = $"/projects/{projectId}/information";
            Assert.Equal(about, string.Format(RouteConstants.ProjectAbout, projectId));
            Assert.Equal(about + "/edit", string.Format(RouteConstants.ProjectEditAbout, projectId));
        }

        [Fact]
        public void Groups_ShouldBeCorrect()
        {
            Assert.Equal("/groups", RouteConstants.Groups);
        }

        [Fact]
        public void ServiceSupportProjects_ShouldBeCorrect()
        {
            Assert.Equal("/projects/service-support/without-academy-urn", RouteConstants.ServiceSupportProjects);
        }

        [Fact]
        public void LocalAuthorityRoutes_ShouldFormatCorrectly()
        {
            var localAuthorityId = Guid.NewGuid();
            Assert.Equal("/service-support/local-authorities", RouteConstants.ListLocalAuthorities);
            Assert.Equal($"/service-support/local-authorities/{localAuthorityId}", string.Format(RouteConstants.LocalAuthorityDetails, localAuthorityId));
            Assert.Equal("/service-support/local-authorities/new", RouteConstants.CreateNewLocalAuthority);
            Assert.Equal($"/service-support/local-authorities/{localAuthorityId}/edit", string.Format(RouteConstants.EditLocalAuthorityDetails, localAuthorityId));
            Assert.Equal($"/service-support/local-authorities/{localAuthorityId}/delete", string.Format(RouteConstants.DeleteLocalAuthorityDetails, localAuthorityId));
        }

        [Fact]
        public void HandoverProjects_ShouldBeCorrect()
        {
            Assert.Equal("/projects/all/handover", RouteConstants.ProjectsHandover);
        }

        [Fact]
        public void HandoverProjectCheck_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            Assert.Equal($"/projects/all/handover/{projectId}/check", string.Format(RouteConstants.ProjectsHandoverCheck, projectId));
        }

        [Fact]
        public void NewHandoverProject_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            Assert.Equal($"/projects/all/handover/{projectId}/new", string.Format(RouteConstants.ProjectsHandoverNew, projectId));
        }
        [Fact]
        public void ProjectCreated_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            Assert.Equal($"/projects/{projectId}/created", string.Format(RouteConstants.ProjectCreated, projectId));
        }
        [Fact]
        public void ProjectExternalContacts_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            Assert.Equal($"/projects/{projectId}/external-contacts", string.Format(RouteConstants.ProjectExternalContacts, projectId));
        }
        [Fact]
        public void NewProjectExternalContacts_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            Assert.Equal($"/projects/{projectId}/external-contacts/new", string.Format(RouteConstants.NewProjectExternalContacts, projectId));
        }
        [Fact]
        public void ProjectsExternalContactAdd_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            var contactType = "headteacher";
            Assert.Equal($"/projects/{projectId}/external-contacts/new/create-contact/{contactType}", string.Format(RouteConstants.ProjectsExternalContactAdd, projectId, contactType));
        }
        [Fact]
        public void ProjectsExternalContactAddTypeOther_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            Assert.Equal($"/projects/{projectId}/external-contacts/new/create-other-contact", string.Format(RouteConstants.ProjectsExternalContactAddTypeOther, projectId));
        }
        [Fact]
        public void ProjectsExternalContactDelete_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            Assert.Equal($"/projects/{projectId}/external-contacts/{contactId}/delete", string.Format(RouteConstants.ProjectsExternalContactDelete, projectId, contactId));
        }
        [Fact]
        public void ProjectsExternalContactEdit_ShouldBeCorrect()
        {
            var projectId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            Assert.Equal($"/projects/{projectId}/external-contacts/{contactId}/edit", string.Format(RouteConstants.ProjectsExternalContactEdit, projectId, contactId));
        }

    }
}
