namespace Dfe.Complete.Constants
{
    public static class RouteConstants
    {
        // All projects

        private const string AllProjectsPrefix = "/projects/all";
        public const string ProjectsHandover = AllProjectsPrefix + "/handover";
        public const string ProjectsInProgress = AllProjectsPrefix + "/in-progress/all";
        public const string ProjectsByRegion = AllProjectsPrefix + "/regions";
        public const string ProjectsForRegion = AllProjectsPrefix + "/regions/{0}";
        public const string ProjectsByUser = AllProjectsPrefix + "/users";
        public const string ProjectsByLocalAuthority = AllProjectsPrefix + "/local-authorities";
        public const string ProjectsByTrust = AllProjectsPrefix + "/trusts";
        public const string TrustProjects = AllProjectsPrefix + "/trusts/ukprn/{0}";
        public const string TrustMATProjects = AllProjectsPrefix + "/trusts/reference/{0}";
        public const string ConversionProjectsByMonth = AllProjectsPrefix + "/by-month/conversions/{0}/{1}";
        public const string TransfersProjectsByMonth = AllProjectsPrefix + "/by-month/transfers/{0}/{1}"; 
        public const string CompletedProjects = AllProjectsPrefix + "/completed";
        public const string ProjectsHandoverCheck = ProjectsHandover + "/{0}/check";
        public const string ProjectsHandoverNew = ProjectsHandover + "/{0}/new";

        public const string Statistics = AllProjectsPrefix + "/statistics";
        public const string Export = AllProjectsPrefix + "/export";
        public const string Reports = AllProjectsPrefix + "/reports";

        // Your projects
        private const string YourProjectsPrefix = "/projects/yours";
        public const string YourProjectsInProgress = YourProjectsPrefix + "/in-progress";
        public const string YourProjectsCompleted = YourProjectsPrefix + "/completed";

        // Your team projects
        public const string TeamProjectsInProgress = "/projects/team/in-progress";
        public const string TeamProjectsNew = "/projects/team/new";
        public const string TeamProjectsUsers = "/projects/team/users";
        public const string TeamProjectsHandedOver = "/projects/team/handed-over";
        public const string TeamProjectsCompleted = "/projects/team/completed";
        public const string TeamProjectsUnassigned = "/projects/team/unassigned";

        // Project
        public const string Project = "/projects/{0}";
        public const string CreateNewProject = "/projects/CreateNewProject";
        public const string ProjectCreated = "/projects/{0}/created";
        public const string ProjectDelete = "/projects/{0}/confirm_delete";

        public const string SelectCreateProjectType = "/projects/new";

        public const string ProjectConversionEdit = "/projects/conversions/{0}/edit#{1}";
        public const string ProjectTransferEdit = "/projects/transfers/{0}/edit#{1}";

        public const string ProjectViewNotes = Project + "/notes";
        public const string ProjectAddNote = ProjectViewNotes + "/new";
        public const string ProjectAddTaskNote = ProjectViewNotes + "/new?task_identifier={1}";
        public const string ProjectEditNote = ProjectViewNotes + "/{1}/edit";

        public const string ProjectInternalContacts = Project + "/internal-contacts";
        public const string ProjectInternalContactsEditAssignedUser = ProjectInternalContacts + "/assigned-user/edit";
        public const string ProjectInternalContactsEditAddedByUser = ProjectInternalContacts + "/added-by-user/edit";
        public const string ProjectInternalContactsEditAssignedTeam = ProjectInternalContacts + "/team/edit";
        public const string ProjectDeleteNote = ProjectViewNotes + "/{1}/delete";

        public const string ProjectTaskList = Project + "/tasks";

        public const string ProjectAbout = Project + "/information";
        public const string ProjectEditAbout = ProjectAbout + "/edit";
        public const string ProjectDateHistory = Project + "/date-history";
        public const string ProjectTask = ProjectTaskList + "/{1}";
        public const string ChangeProjectDateHistory = Project + "/date-history/new";
        public const string ChangeProjectDateHistoryReason = Project + "/date-history/reason";
        public const string ChangeProjectDateHistoryConfirm = Project + "/date-history/reasons/later";


        public const string ProjectHandoverWithDeliveryOfficerTask = ProjectTaskList + "/handover";
        public const string ProjectStakeholderKickoffTask = ProjectTaskList + "/stakeholder_kick_off";
        public const string ProjectLandQuestionnaireTask = ProjectTaskList + "/land_questionnaire";
        public const string ProjectLandRegistryTask = ProjectTaskList + "/land_registry";
        public const string ProjectSupplementalFundingAgreementTask = ProjectTaskList + "/supplemental_funding_agreement";
        public const string ProjectRiskProtectionArrangementTask = ProjectTaskList + "/risk_protection_arrangement";
        public const string ProjectRiskProtectionArrangementPolicyTask = ProjectTaskList + "/rpa_policy";
        public const string ProjectCheckAccuracyOfHigherNeedsTask = ProjectTaskList + "/check_accuracy_of_higher_needs";
        public const string ProjectCompleteNotificationOfChangeTask = ProjectTaskList + "/complete_notification_of_change";
        public const string ProjectProcessConversionSupportGrantTask = ProjectTaskList + "/conversion_grant";
        public const string ProjectConfirmAndProcessSponsoredSupportGrantTask = ProjectTaskList + "/sponsored_support_grant";
        public const string ProjectConfirmAcademyNameTask = ProjectTaskList + "/academy_details";
        public const string ProjectConfirmHeadTeacherDetailsTask = ProjectTaskList + "/confirm_headteacher_contact";
        public const string ProjectConfirmChairOfGovernorsDetailsTask = ProjectTaskList + "/confirm_chair_of_governors_contact";
        public const string ProjectConfirmIncomingTrustCeoDetailsTask = ProjectTaskList + "/confirm_incoming_trust_ceo_contact";
        public const string ProjectConfirmMainContactTask = ProjectTaskList + "/main_contact";
        public const string ProjectConfirmProposedCapacityOfTheAcademyTask = ProjectTaskList + "/proposed_capacity_of_the_academy";
        public const string ProjectArticlesOfAssociationTask = ProjectTaskList + "/articles_of_association";
        public const string ProjectDeedOfVariationTask = ProjectTaskList + "/deed_of_variation";
        public const string ProjectTrustModificationOrderTask = ProjectTaskList + "/trust_modification_order";
        public const string ProjectDirectionToTransferTask = ProjectTaskList + "/direction_to_transfer";
        public const string ProjectOneHundredAndTwentyFiveYearLeaseTask = ProjectTaskList + "/one_hundred_and_twenty_five_year_lease";
        public const string ProjectSubleasesTask = ProjectTaskList + "/subleases";
        public const string ProjectTenancyAtWillTask = ProjectTaskList + "/tenancy_at_will";
        public const string ProjectCommercialTransferAgreementTask = ProjectTaskList + "/commercial_transfer_agreement";
        public const string ProjectConfirmTheSchoolHasCompletedAllActionsTask = ProjectTaskList + "/school_completed";
        public const string ProjectConfirmAllConditionsHaveBeenMetTask = ProjectTaskList + "/conditions_met";
        public const string ProjectShareTheInformationAboutOpeningTask = ProjectTaskList + "/share_information";
        public const string ProjectConfirmDateAcademyOpenedTask = ProjectTaskList + "/confirm_date_academy_opened";
        public const string ProjectRedactAndSendTask = ProjectTaskList + "/redact_and_send";
        public const string ProjectReceiveDeclarationOfExpenditureCertificateTask = ProjectTaskList + "/receive_grant_payment_certificate";
        public const string ProjectComplete = Project + "/complete";
        public const string ProjectDaoRevocation = Project + "/dao-revocation";
        public const string ProjectDaoRevocationConfirm = ProjectDaoRevocation + "/confirm";
        public const string ProjectDaoRevocationReason = ProjectDaoRevocation + "/reasons";
        public const string ChangeProjectDaoRevocationReason = ProjectDaoRevocationReason + "/change";
        public const string ProjectDaoRevocationMinister = ProjectDaoRevocation + "/minister";
        public const string ChangeProjectDaoRevocationMinister = ProjectDaoRevocationMinister + "/change";
        public const string ProjectDaoRevocationDate = ProjectDaoRevocation + "/date";
        public const string ChangeProjectDaoRevocationDate = ProjectDaoRevocationDate + "/change";
        public const string ProjectDaoRevocationCheck = ProjectDaoRevocation + "/check";
        public const string ProjectConfirmOutingTrustCeoDetailsTask = ProjectTaskList + "/confirm_outgoing_trust_ceo_contact";
        public const string ProjectRequestNewURNAndRecordForTheAcademyTask = ProjectTaskList + "/request_new_urn_and_record";
        public const string ProjectCheckAndConfirmAcademyAndTrustFinancialInfoTask = ProjectTaskList + "/check_and_confirm_financial_information";
        public const string ProjectConfirmTransferGrantFundingLevelTask = ProjectTaskList + "/confirm_new_urn_and_record";

        public const string ProjectFormMTask = ProjectTaskList + "/form_m";
        public const string ProjectLandConsentLetterTask = ProjectTaskList + "/land_consent_letter";
        public const string ProjectDeedOfNovationAndVariationTask = ProjectTaskList + "/deed_of_novation_and_variation";
        public const string ProjectChurchSupplementalAgreementTask = ProjectTaskList + "/church_supplemental_agreement";
        public const string ProjectMasterFundingAgreementTask = ProjectTaskList + "/master_funding_agreement";
        public const string ProjectDeedOfTerminationForTheMasterFundingAgreementTask = ProjectTaskList + "/deed_of_termination_for_the_master_funding_agreement";
        public const string ProjectDeedOfTerminationForChurchSupplementalAreementTask = ProjectTaskList + "/deed_termination_church_agreement";
        public const string ProjectClosureOrTransferDeclarationTask = ProjectTaskList + "/closure_or_transfer_declaration";
        public const string ProjectConfirmBankDetailsChangingForGeneralAnnualGrantPaymentTask = ProjectTaskList + "/bank_details_changing";
        public const string ProjectConfirmIncomingTrustHasCompletedAllActionsTask = ProjectTaskList + "/confirm_incoming_trust_has_completed_all_actions";
        public const string ProjectConfirmDateAcademyTransferredTask = ProjectTaskList + "/confirm_date_academy_transferred";
        public const string ProjectRedactAndSendDocumentsTask = ProjectTaskList + "/redact_and_send_documents";
        public const string ProjectDeclarationOfExpenditureCertificateTask = ProjectTaskList + "/declaration_of_expenditure_certificate";

        // Groups
        public const string Groups = "/groups";
        public const string GroupDetails = "/groups/{0}";

        // Service support
        public const string ServiceSupportProjectsWithoutAcademyUrn = "/projects/service-support/without-academy-urn";
        public const string ServiceSupportProjectsWithAcademyUrn = "/projects/service-support/with-academy-urn";
        public const string ServiceSupportAssignAcademyUrn = "/projects/{0}/academy-urn";
        public const string ServiceSupportProjects = "/projects/service-support/without-academy-urn";
        public const string ServiceSupportUsers = "/service-support/users";
        public const string ServiceSupportUsersNew = "/service-support/users/new";
        public const string ServiceSupportUsersEdit = "/service-support/users/{0}/edit";


        //Local authorities
        public const string ListLocalAuthorities = "/service-support/local-authorities";
        public const string LocalAuthorityDetails = "/service-support/local-authorities/{0}";
        public const string CreateNewLocalAuthority = "/service-support/local-authorities/new";
        public const string EditLocalAuthorityDetails = "/service-support/local-authorities/{0}/edit";
        public const string DeleteLocalAuthorityDetails = "/service-support/local-authorities/{0}/delete";

        // Project External contacts        
        public const string ProjectExternalContacts = Project + "/external-contacts";
        public const string NewProjectExternalContacts = ProjectExternalContacts + "/new";
        public const string ProjectsExternalContactAdd = NewProjectExternalContacts + "/create-contact/{1}";
        public const string ProjectsExternalContactAddTypeOther = NewProjectExternalContacts + "/create-other-contact";
        public const string ProjectsExternalContactDelete = ProjectExternalContacts + "/{1}/delete";
        public const string ProjectsExternalContactEdit = ProjectExternalContacts + "/{1}/edit";

        // Error Page
        public const string ErrorPage = "/error";
    }
}