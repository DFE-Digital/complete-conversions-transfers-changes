namespace Dfe.Complete.Constants
{
    public static class RouteConstants
    {
        // All projects

        private const string AllProjectsPrefix = "/projects/all";
        public const string ProjectsInProgress = AllProjectsPrefix + "/in-progress/all";
        public const string ProjectsByTrust = AllProjectsPrefix + "/trusts";
        public const string TrustProjects = AllProjectsPrefix + "/trusts/ukprn/{0}";
        public const string TrustMATProjects = AllProjectsPrefix + "/trusts/reference/{0}";
        public const string ConversionProjectsByMonth = AllProjectsPrefix + "/by-month/conversions/{0}/{1}";
        public const string TransfersProjectsByMonth = AllProjectsPrefix + "/by-month/transfers/{0}/{1}";
        public const string ConversionProjectsByMonths = AllProjectsPrefix + "/by-month/conversions/from/{0}/{1}/to/{2}/{3}";
        public const string TransfersProjectsByMonths = AllProjectsPrefix + "/by-month/transfers/from/{0}/{1}/to/{2}/{3}";
        public const string ProjectsByRegion = AllProjectsPrefix + "/regions/{0}";
        public const string CompletedProjects = AllProjectsPrefix + "/completed";
        
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
        
        
        
        public const string ProjectViewNotes = Project + "/notes";
        public const string ProjectAddNote = ProjectViewNotes + "/edit";
        public const string ProjectEditNote = ProjectViewNotes + "/{1}/edit";
        
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
        public const string ServiceSupportProjects = "/projects/service-support/without-academy-urn";
    }
}