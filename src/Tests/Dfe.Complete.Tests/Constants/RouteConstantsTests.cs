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
        public void ConversionProjectsByMonths_ShouldFormatCorrectly()
        {
            var fromYear = "2024";
            var fromMonth = "01";
            var toYear = "2025";
            var toMonth = "12";
            var expected = $"/projects/all/by-month/conversions/from/{fromYear}/{fromMonth}/to/{toYear}/{toMonth}";
            Assert.Equal(expected, string.Format(RouteConstants.ConversionProjectsByMonths, fromYear, fromMonth, toYear, toMonth));
        }

        [Fact]
        public void TransfersProjectsByMonths_ShouldFormatCorrectly()
        {
            var fromYear = "2024";
            var fromMonth = "01";
            var toYear = "2025";
            var toMonth = "12";
            var expected = $"/projects/all/by-month/transfers/from/{fromYear}/{fromMonth}/to/{toYear}/{toMonth}";
            Assert.Equal(expected, string.Format(RouteConstants.TransfersProjectsByMonths, fromYear, fromMonth, toYear, toMonth));
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
            Assert.Equal(basePath + "/stakeholder-kickoff", string.Format(RouteConstants.ProjectStakeholderKickoffTask, projectId));
            Assert.Equal(basePath + "/land-questionnaire", string.Format(RouteConstants.ProjectLandQuestionnaireTask, projectId));
            Assert.Equal(basePath + "/land-registry", string.Format(RouteConstants.ProjectLandRegistryTask, projectId));
            Assert.Equal(basePath + "/supplemental-funding-agreement", string.Format(RouteConstants.ProjectSupplementalFundingAgreementTask, projectId));
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
        
    }
}
