﻿namespace Dfe.Complete.Constants
{
    public static class RouteConstants
    {
        public const string YourProjectsInProgress = "/projects/yours/in-progress";
        public const string ProjectsInProgress = "/projects/all/in-progress/all";
        public const string ProjectsByTrust = "/projects/all/trusts";
        public const string TrustProjects = "/projects/all/trusts/ukprn/{0}";
        public const string TrustMATProjects = "/projects/all/trusts/reference/{0}";

        public const string ProjectsByRegion = "/projects/all/regions/{0}";

        // Your team projects
        public const string TeamProjectsInProgress = "/projects/team/in-progress";
        public const string TeamProjectsCompleted = "/projects/team/completed";

        // Project
        public const string Project = "/projects/{0}";
        public const string CreateNewProject = "/projects/CreateNewProject";

        public const string SelectCreateProjectType = "/projects/new";
        
        
        public const string ProjectViewNotes = Project + "/notes";
        public const string ProjectAddNote = ProjectViewNotes + "/edit";
        public const string ProjectEditNote = ProjectViewNotes + "/{1}/edit";

        // Conversion
        public const string ConversionProject = "/conversion-projects/{0}";
        public const string CreateNewConversionProject = "/projects/conversions/new";
        public const string ConversionProjectTaskList = ConversionProject + "/tasks";
        public const string ConversionProjectAbout = ConversionProject + "/information";

        public const string ConversionViewHandoverWithDeliveryOfficerTask = ConversionProjectTaskList + "/handover";
        public const string ConversionEditHandoverWithDeliveryOfficerTask = ConversionViewHandoverWithDeliveryOfficerTask + "/edit";

        public const string ConversionStakeholderKickoffTask = ConversionProjectTaskList + "/stakeholder-kickoff";
        public const string ConversionEditStakeholderKickoffTask = ConversionStakeholderKickoffTask + "/edit";

        public const string ConversionLandQuestionnaireTask = ConversionProjectTaskList + "/land-questionnaire";
        public const string ConversionEditLandQuestionnaireTask = ConversionLandQuestionnaireTask + "/edit";

        public const string ConversionLandRegistryTask = ConversionProjectTaskList + "/land-registry";
        public const string ConversionEditLandRegistryTask = ConversionLandRegistryTask + "/edit";

        public const string ConversionSupplementalFundingAgreementTask = ConversionProjectTaskList + "/supplemental-funding-agreement";
        public const string ConversionEditSupplementalFundingAgreementTask = ConversionSupplementalFundingAgreementTask + "/edit";

        // Transfer
        public const string TransferProject = "/transfer-projects/{0}";
        public const string TransferProjectTaskList = TransferProject + "/tasks";
        public const string TransferProjectAbout = TransferProject + "/information";
        public const string TransferProjectEditAbout = TransferProjectAbout + "/edit";

        public const string TransferViewHandoverWithDeliveryOfficerTask = TransferProjectTaskList + "/handover";
        public const string TransferEditHandoverWithDeliveryOfficerTask = TransferViewHandoverWithDeliveryOfficerTask + "/edit";

        public const string TransferViewStakeholderKickoffTask = TransferProjectTaskList + "/stakeholder-kickoff";
        public const string TransferEditStakeholderKickoffTask = TransferViewStakeholderKickoffTask + "/edit";
    }
}