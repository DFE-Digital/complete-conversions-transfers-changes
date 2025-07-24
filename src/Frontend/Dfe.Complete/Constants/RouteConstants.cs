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
        public const string ConversionProjectsByMonths = AllProjectsPrefix + "/by-month/conversions/from/{0}/{1}/to/{2}/{3}";
        public const string TransfersProjectsByMonths = AllProjectsPrefix + "/by-month/transfers/from/{0}/{1}/to/{2}/{3}";
        public const string CompletedProjects = AllProjectsPrefix + "/completed";
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

        public const string SelectCreateProjectType = "/projects/new";

        public const string ProjectConversionEdit = "/projects/conversions/{0}";
        public const string ProjectTransferEdit = "/projects/transfers/{0}";

        public const string ProjectViewNotes = Project + "/notes";
        public const string ProjectAddNote = ProjectViewNotes + "/new";
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
        
        public const string ProjectHandoverWithDeliveryOfficerTask = ProjectTaskList + "/handover";
        public const string ProjectStakeholderKickoffTask = ProjectTaskList + "/stakeholder-kickoff";
        public const string ProjectLandQuestionnaireTask = ProjectTaskList + "/land-questionnaire";
        public const string ProjectLandRegistryTask = ProjectTaskList + "/land-registry";
        public const string ProjectSupplementalFundingAgreementTask = ProjectTaskList + "/supplemental-funding-agreement";

        // Groups
        public const string Groups = "/groups";

        // Service support
        public const string ServiceSupportProjectsWithoutAcademyUrn = "/projects/service-support/without-academy-urn";
        public const string ServiceSupportProjectsWithAcademyUrn = "/projects/service-support/with-academy-urn";
        public const string ServiceSupportAssignAcademyUrn = "/projects/{0}/academy-urn";
        public const string ServiceSupportProjects = "/projects/service-support/without-academy-urn";

        //Local authorities
        public const string ListLocalAuthorities = "/service-support/local-authorities";
        public const string LocalAuthorityDetails = "/service-support/local-authorities/{0}";
        public const string CreateNewLocalAuthority = "/service-support/local-authorities/new";
        public const string EditLocalAuthorityDetails = "/service-support/local-authorities/{0}/edit";
        public const string DeleteLocalAuthorityDetails = "/service-support/local-authorities/{0}/delete";
    }
}