using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Domain.Tests.Aggregates
{
    public class ProjectTests
    {
        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization))]
        public void Constructor_ShouldThrowArgumentNullException_WhenProjectUrnIsNull(
            DateTime createdAt,
            DateTime updatedAt,
            TaskType taskType,
            ProjectType projectType,
            Guid tasksDataId,
            DateOnly significantDate,
            bool isSignificantDateProvisional,
            Ukprn incomingTrustUkprn,
            Region region,
            bool isDueTo2RI,
            bool hasAcademyOrderBeenIssued,
            DateOnly advisoryBoardDate,
            string advisoryBoardConditions,
            string establishmentSharepointLink,
            string incomingTrustSharepointLink
            )
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Project(null,
                                 createdAt,
                                 updatedAt,
                                 taskType,
                                 projectType,
                                 tasksDataId,
                                 significantDate,
                                 isSignificantDateProvisional,
                                 incomingTrustUkprn,
                                 region,
                                 isDueTo2RI,
                                 hasAcademyOrderBeenIssued,
                                 advisoryBoardDate,
                                 advisoryBoardConditions,
                                 establishmentSharepointLink,
                                 incomingTrustSharepointLink));

            Assert.Equal("urn", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization))]
        public void Constructor_ShouldThrowArgumentNullException_WhenProjectCreatedAtIsDefault(
            Urn urn,
            DateTime updatedAt,
            TaskType taskType,
            ProjectType projectType,
            Guid tasksDataId,
            DateOnly significantDate,
            bool isSignificantDateProvisional,
            Ukprn incomingTrustUkprn,
            Region region,
            bool isDueTo2RI,
            bool hasAcademyOrderBeenIssued,
            DateOnly advisoryBoardDate,
            string advisoryBoardConditions,
            string establishmentSharepointLink,
            string incomingTrustSharepointLink
            )
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Project(urn,
                                 default,
                                 updatedAt,
                                 taskType,
                                 projectType,
                                 tasksDataId,
                                 significantDate,
                                 isSignificantDateProvisional,
                                 incomingTrustUkprn,
                                 region,
                                 isDueTo2RI,
                                 hasAcademyOrderBeenIssued,
                                 advisoryBoardDate,
                                 advisoryBoardConditions,
                                 establishmentSharepointLink,
                                 incomingTrustSharepointLink));

            Assert.Equal("createdAt", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization))]
        public void Constructor_ShouldThrowArgumentNullException_WhenProjectUpdatedAtIsDefault(
            Urn urn,
            DateTime createdAt,
            TaskType taskType,
            ProjectType projectType,
            Guid tasksDataId,
            DateOnly significantDate,
            bool isSignificantDateProvisional,
            Ukprn incomingTrustUkprn,
            Region region,
            bool isDueTo2RI,
            bool hasAcademyOrderBeenIssued,
            DateOnly advisoryBoardDate,
            string advisoryBoardConditions,
            string establishmentSharepointLink,
            string incomingTrustSharepointLink
            )
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Project(urn,
                                 createdAt,
                                 default,
                                 taskType,
                                 projectType,
                                 tasksDataId,
                                 significantDate,
                                 isSignificantDateProvisional,
                                 incomingTrustUkprn,
                                 region,
                                 isDueTo2RI,
                                 hasAcademyOrderBeenIssued,
                                 advisoryBoardDate,
                                 advisoryBoardConditions,
                                 establishmentSharepointLink,
                                 incomingTrustSharepointLink));

            Assert.Equal("updatedAt", exception.ParamName);
        }


        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization))]
        public void Constructor_ShouldCorrectlySetFields(
            Urn urn,
            DateTime createdAt,
            DateTime updatedAt,
            TaskType taskType,
            ProjectType projectType,
            Guid tasksDataId,
            DateOnly significantDate,
            bool isSignificantDateProvisional,
            Ukprn incomingTrustUkprn,
            Region region,
            bool isDueTo2RI,
            bool hasAcademyOrderBeenIssued,
            DateOnly advisoryBoardDate,
            string advisoryBoardConditions,
            string establishmentSharepointLink,
            string incomingTrustSharepointLink
            )
        {
            // Act & Assert
            var project = new Project(urn,
                                 createdAt,
                                 updatedAt,
                                 taskType,
                                 projectType,
                                 tasksDataId,
                                 significantDate,
                                 isSignificantDateProvisional,
                                 incomingTrustUkprn,
                                 region,
                                 isDueTo2RI,
                                 hasAcademyOrderBeenIssued,
                                 advisoryBoardDate,
                                 advisoryBoardConditions,
                                 establishmentSharepointLink,
                                 incomingTrustSharepointLink);

            Assert.Equal(urn, project.Urn);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization))]
        public void Factory_Create_ShouldCorrectlySetFields(
          Urn urn,
          DateTime createdAt,
          DateTime updatedAt,
          TaskType taskType,
          ProjectType projectType,
          Guid tasksDataId,
          DateOnly significantDate,
          bool isSignificantDateProvisional,
          Ukprn incomingTrustUkprn,
          Region region,
          bool isDueTo2RI,
          bool hasAcademyOrderBeenIssued,
          DateOnly advisoryBoardDate,
          string advisoryBoardConditions,
          string establishmentSharepointLink,
          string incomingTrustSharepointLink,
          string groupReferenceNumber,
          DateOnly provisionalConversionDate,
          bool handingOverToRegionalCaseworkService,
          string handoverComments
          )
        {
            // Act & Assert
            var project = Project.CreateConversionProject(urn,
                                 createdAt,
                                 updatedAt,
                                 taskType,
                                 projectType,
                                 tasksDataId,
                                 significantDate,
                                 isSignificantDateProvisional,
                                 incomingTrustUkprn,
                                 region,
                                 isDueTo2RI,
                                 hasAcademyOrderBeenIssued,
                                 advisoryBoardDate,
                                 advisoryBoardConditions,
                                 establishmentSharepointLink,
                                 incomingTrustSharepointLink,
                                 groupReferenceNumber,
                                 provisionalConversionDate,
                                 handingOverToRegionalCaseworkService,
                                 handoverComments);

            Assert.Equal(urn, project.Urn);
            Assert.Equal(createdAt, project.CreatedAt);
            Assert.Equal(updatedAt, project.UpdatedAt);
            Assert.Equal(taskType, project.TasksDataType);
            Assert.Equal(projectType, project.Type);
            Assert.Equal(tasksDataId, project.TasksDataId);
            Assert.Equal(significantDate, project.SignificantDate);
            Assert.Equal(isSignificantDateProvisional, project.SignificantDateProvisional);
            Assert.Equal(incomingTrustUkprn, project.IncomingTrustUkprn);
            Assert.Equal(region, project.Region);
            Assert.Equal(isDueTo2RI, project.TwoRequiresImprovement);
            Assert.Equal(hasAcademyOrderBeenIssued, project.DirectiveAcademyOrder);
            Assert.Equal(advisoryBoardDate, project.AdvisoryBoardDate);
            Assert.Equal(advisoryBoardConditions, project.AdvisoryBoardConditions);
            Assert.Equal(establishmentSharepointLink, project.EstablishmentSharepointLink);
            Assert.Equal(incomingTrustSharepointLink, project.IncomingTrustSharepointLink);


        }
    }
}
