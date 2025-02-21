using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Project = Dfe.Complete.Domain.Entities.Project;

namespace Dfe.Complete.Domain.Tests.Aggregates
{
    public class ProjectTests
    {
        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization),
            typeof(IgnoreVirtualMembersCustomisation))]
        public void Constructor_ShouldThrowArgumentNullException_WhenProjectUrnIsNull(Project projectCustomisation)
        {
            // Arrange
            projectCustomisation.Urn = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Project(
                    projectCustomisation.Id,
                    projectCustomisation.Urn,
                    projectCustomisation.CreatedAt,
                    projectCustomisation.UpdatedAt,
                    projectCustomisation.TasksDataType!.Value,
                    projectCustomisation.Type!.Value,
                    projectCustomisation.TasksDataId!.Value,
                    projectCustomisation.SignificantDate!.Value,
                    projectCustomisation.SignificantDateProvisional!.Value,
                    projectCustomisation.IncomingTrustUkprn,
                    projectCustomisation.OutgoingTrustUkprn,
                    projectCustomisation.Region,
                    projectCustomisation.TwoRequiresImprovement!.Value,
                    projectCustomisation.DirectiveAcademyOrder,
                    projectCustomisation.AdvisoryBoardDate!.Value,
                    projectCustomisation.AdvisoryBoardConditions!,
                    projectCustomisation.EstablishmentSharepointLink!,
                    projectCustomisation.IncomingTrustSharepointLink!,
                    projectCustomisation.OutgoingTrustSharepointLink,
                    projectCustomisation.GroupId,
                    projectCustomisation.Team,
                    projectCustomisation.RegionalDeliveryOfficerId,
                    projectCustomisation.AssignedToId,
                    projectCustomisation.AssignedAt,
                    projectCustomisation.NewTrustName,
                    projectCustomisation.NewTrustReferenceNumber,
                    projectCustomisation.LocalAuthorityId.Value
                ));

            Assert.Equal("urn", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization),
            typeof(IgnoreVirtualMembersCustomisation))]
        public void Constructor_ShouldThrowArgumentNullException_WhenProjectCreatedAtIsDefault(
            Project projCustomisation)
        {
            // Arrange
            projCustomisation.CreatedAt = default;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Project(
                    projCustomisation.Id,
                    projCustomisation.Urn!,
                    projCustomisation.CreatedAt,
                    projCustomisation.UpdatedAt,
                    projCustomisation.TasksDataType!.Value,
                    projCustomisation.Type!.Value,
                    projCustomisation.TasksDataId!.Value,
                    projCustomisation.SignificantDate!.Value,
                    projCustomisation.SignificantDateProvisional!.Value,
                    projCustomisation.IncomingTrustUkprn,
                    projCustomisation.OutgoingTrustUkprn,
                    projCustomisation.Region,
                    projCustomisation.TwoRequiresImprovement!.Value,
                    projCustomisation.DirectiveAcademyOrder,
                    projCustomisation.AdvisoryBoardDate!.Value,
                    projCustomisation.AdvisoryBoardConditions!,
                    projCustomisation.EstablishmentSharepointLink!,
                    projCustomisation.IncomingTrustSharepointLink!,
                    projCustomisation.OutgoingTrustSharepointLink,
                    projCustomisation.GroupId,
                    projCustomisation.Team,
                    projCustomisation.RegionalDeliveryOfficerId,
                    projCustomisation.AssignedToId,
                    projCustomisation.AssignedAt,
                    projCustomisation.NewTrustName,
                    projCustomisation.NewTrustReferenceNumber, 
                    projCustomisation.LocalAuthorityId.Value
                ));

            Assert.Equal("createdAt", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization),
            typeof(IgnoreVirtualMembersCustomisation))]
        public void Constructor_ShouldThrowArgumentNullException_WhenProjectUpdatedAtIsDefault(
            Project projCustomisation)
        {
            // Arrange
            projCustomisation.UpdatedAt = default;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Project(
                    projCustomisation.Id,
                    projCustomisation.Urn!,
                    projCustomisation.CreatedAt,
                    projCustomisation.UpdatedAt,
                    projCustomisation.TasksDataType!.Value,
                    projCustomisation.Type!.Value,
                    projCustomisation.TasksDataId!.Value,
                    projCustomisation.SignificantDate!.Value,
                    projCustomisation.SignificantDateProvisional!.Value,
                    projCustomisation.IncomingTrustUkprn,
                    projCustomisation.OutgoingTrustUkprn,
                    projCustomisation.Region,
                    projCustomisation.TwoRequiresImprovement!.Value,
                    projCustomisation.DirectiveAcademyOrder,
                    projCustomisation.AdvisoryBoardDate!.Value,
                    projCustomisation.AdvisoryBoardConditions!,
                    projCustomisation.EstablishmentSharepointLink!,
                    projCustomisation.IncomingTrustSharepointLink!,
                    projCustomisation.OutgoingTrustSharepointLink,
                    projCustomisation.GroupId,
                    projCustomisation.Team,
                    projCustomisation.RegionalDeliveryOfficerId,
                    projCustomisation.AssignedToId,
                    projCustomisation.AssignedAt,
                    projCustomisation.NewTrustName,
                    projCustomisation.NewTrustReferenceNumber,
                    projCustomisation.LocalAuthorityId.Value
                ));

            Assert.Equal("updatedAt", exception.ParamName);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization),
            typeof(IgnoreVirtualMembersCustomisation))]
        public void Constructor_ShouldCorrectlySetFields(Project projectCustomisation)
        {
            // Act
            var project = new Project(
                projectCustomisation.Id,
                projectCustomisation.Urn!,
                projectCustomisation.CreatedAt,
                projectCustomisation.UpdatedAt,
                projectCustomisation.TasksDataType!.Value,
                projectCustomisation.Type!.Value,
                projectCustomisation.TasksDataId!.Value,
                projectCustomisation.SignificantDate!.Value,
                projectCustomisation.SignificantDateProvisional!.Value,
                projectCustomisation.IncomingTrustUkprn,
                projectCustomisation.OutgoingTrustUkprn,
                projectCustomisation.Region,
                projectCustomisation.TwoRequiresImprovement!.Value,
                projectCustomisation.DirectiveAcademyOrder,
                projectCustomisation.AdvisoryBoardDate!.Value,
                projectCustomisation.AdvisoryBoardConditions!,
                projectCustomisation.EstablishmentSharepointLink!,
                projectCustomisation.IncomingTrustSharepointLink!,
                projectCustomisation.OutgoingTrustSharepointLink,
                projectCustomisation.GroupId,
                projectCustomisation.Team,
                projectCustomisation.RegionalDeliveryOfficerId,
                projectCustomisation.AssignedToId,
                projectCustomisation.AssignedAt,
                projectCustomisation.NewTrustName,
                projectCustomisation.NewTrustReferenceNumber, 
                projectCustomisation.LocalAuthorityId.Value
            );

            // Assert
            Assert.Equal(projectCustomisation.Urn, project.Urn);
            Assert.Equal(projectCustomisation.CreatedAt, project.CreatedAt);
            Assert.Equal(projectCustomisation.UpdatedAt, project.UpdatedAt);
            Assert.Equal(projectCustomisation.TasksDataType, project.TasksDataType);
            Assert.Equal(projectCustomisation.Type, project.Type);
            Assert.Equal(projectCustomisation.TasksDataId, project.TasksDataId);
            Assert.Equal(projectCustomisation.SignificantDate, project.SignificantDate);
            Assert.Equal(projectCustomisation.SignificantDateProvisional, project.SignificantDateProvisional);
            Assert.Equal(projectCustomisation.IncomingTrustUkprn, project.IncomingTrustUkprn);
            Assert.Equal(projectCustomisation.OutgoingTrustUkprn, project.OutgoingTrustUkprn);
            Assert.Equal(projectCustomisation.Region, project.Region);
            Assert.Equal(projectCustomisation.TwoRequiresImprovement, project.TwoRequiresImprovement);
            Assert.Equal(projectCustomisation.DirectiveAcademyOrder, project.DirectiveAcademyOrder);
            Assert.Equal(projectCustomisation.AdvisoryBoardDate, project.AdvisoryBoardDate);
            Assert.Equal(projectCustomisation.AdvisoryBoardConditions, project.AdvisoryBoardConditions);
            Assert.Equal(projectCustomisation.EstablishmentSharepointLink, project.EstablishmentSharepointLink);
            Assert.Equal(projectCustomisation.IncomingTrustSharepointLink, project.IncomingTrustSharepointLink);
            Assert.Equal(projectCustomisation.OutgoingTrustSharepointLink, project.OutgoingTrustSharepointLink);
            Assert.Equal(projectCustomisation.GroupId, project.GroupId);
            Assert.Equal(projectCustomisation.Team, project.Team);
            Assert.Equal(projectCustomisation.RegionalDeliveryOfficerId, project.RegionalDeliveryOfficerId);
            Assert.Equal(projectCustomisation.AssignedToId, project.AssignedToId);
            Assert.Equal(projectCustomisation.AssignedAt, project.AssignedAt);
            Assert.Equal(projectCustomisation.NewTrustName, project.NewTrustName);
            Assert.Equal(projectCustomisation.NewTrustReferenceNumber, project.NewTrustReferenceNumber);
            Assert.Equal(projectCustomisation.LocalAuthorityId, project.LocalAuthorityId);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization),
            typeof(IgnoreVirtualMembersCustomisation))]
        public void Factory_CreateConversionProject_ShouldCorrectlySetFields(Project projCustomisation)
        {
            // Arrange
            var handoverComment = "handover comment";

            // Act
            var project = Project.CreateConversionProject(
                projCustomisation.Id,
                projCustomisation.Urn!,
                projCustomisation.CreatedAt,
                projCustomisation.UpdatedAt,
                projCustomisation.TasksDataType!.Value,
                projCustomisation.Type!.Value,
                projCustomisation.TasksDataId!.Value,
                projCustomisation.SignificantDate!.Value,
                projCustomisation.SignificantDateProvisional!.Value,
                projCustomisation.IncomingTrustUkprn, // The factory requires non-null
                projCustomisation.Region,
                projCustomisation.TwoRequiresImprovement!.Value,
                projCustomisation.DirectiveAcademyOrder!.Value,
                projCustomisation.AdvisoryBoardDate!.Value,
                projCustomisation.AdvisoryBoardConditions!,
                projCustomisation.EstablishmentSharepointLink!,
                projCustomisation.IncomingTrustSharepointLink!,
                projCustomisation.GroupId,
                projCustomisation.Team!.Value,
                projCustomisation.RegionalDeliveryOfficerId,
                projCustomisation.AssignedToId,
                projCustomisation.AssignedAt,
                handoverComment, 
                projCustomisation.LocalAuthorityId.Value
            );

            // Assert
            Assert.Equal(projCustomisation.Urn, project.Urn);
            Assert.Equal(projCustomisation.CreatedAt, project.CreatedAt);
            Assert.Equal(projCustomisation.UpdatedAt, project.UpdatedAt);
            Assert.Equal(projCustomisation.TasksDataType, project.TasksDataType);
            Assert.Equal(projCustomisation.Type, project.Type);
            Assert.Equal(projCustomisation.TasksDataId, project.TasksDataId);
            Assert.Equal(projCustomisation.SignificantDate, project.SignificantDate);
            Assert.Equal(projCustomisation.SignificantDateProvisional, project.SignificantDateProvisional);
            Assert.Equal(projCustomisation.IncomingTrustUkprn, project.IncomingTrustUkprn);
            Assert.Equal(projCustomisation.Region, project.Region);
            Assert.Equal(projCustomisation.TwoRequiresImprovement, project.TwoRequiresImprovement);
            Assert.Equal(projCustomisation.DirectiveAcademyOrder, project.DirectiveAcademyOrder);
            Assert.Equal(projCustomisation.AdvisoryBoardDate, project.AdvisoryBoardDate);
            Assert.Equal(projCustomisation.AdvisoryBoardConditions, project.AdvisoryBoardConditions);
            Assert.Equal(projCustomisation.EstablishmentSharepointLink, project.EstablishmentSharepointLink);
            Assert.Equal(projCustomisation.IncomingTrustSharepointLink, project.IncomingTrustSharepointLink);
            Assert.Equal(handoverComment, project.Notes.FirstOrDefault()?.Body);
            Assert.Equal(projCustomisation.LocalAuthorityId, project.LocalAuthorityId);
            
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public void Factory_CreateMatConversionProject_ShouldCorrectlySetFields(Project projectCustomisation)
        {
            // Arrange
            var handoverComment = "MAT Conversion handover comment";

            // Act
            var project = Project.CreateMatConversionProject(
                projectCustomisation.Id,
                projectCustomisation.Urn!, 
                projectCustomisation.CreatedAt,
                projectCustomisation.UpdatedAt,
                projectCustomisation.TasksDataType!.Value, 
                projectCustomisation.Type!.Value,
                projectCustomisation.TasksDataId!.Value,
                projectCustomisation.Region, 
                projectCustomisation.Team!.Value,
                projectCustomisation.RegionalDeliveryOfficerId,
                projectCustomisation.AssignedToId,
                projectCustomisation.AssignedAt,
                projectCustomisation.EstablishmentSharepointLink!,
                projectCustomisation.IncomingTrustSharepointLink!,
                projectCustomisation.AdvisoryBoardDate!.Value,
                projectCustomisation.AdvisoryBoardConditions!,
                projectCustomisation.SignificantDate!.Value,
                projectCustomisation.SignificantDateProvisional!.Value,
                projectCustomisation.TwoRequiresImprovement!.Value,
                projectCustomisation.NewTrustName!,
                projectCustomisation.NewTrustReferenceNumber!,
                projectCustomisation.DirectiveAcademyOrder!.Value,
                handoverComment, projectCustomisation.LocalAuthorityId.Value
            );

            // Assert
            Assert.Equal(projectCustomisation.Urn, project.Urn);
            Assert.Equal(projectCustomisation.CreatedAt, project.CreatedAt);
            Assert.Equal(projectCustomisation.UpdatedAt, project.UpdatedAt);
            Assert.Equal(projectCustomisation.TasksDataType, project.TasksDataType);
            Assert.Equal(projectCustomisation.Type, project.Type);
            Assert.Equal(projectCustomisation.TasksDataId, project.TasksDataId);
            Assert.Equal(projectCustomisation.Region, project.Region);
            Assert.Equal(projectCustomisation.Team, project.Team);
            Assert.Equal(projectCustomisation.RegionalDeliveryOfficerId, project.RegionalDeliveryOfficerId);
            Assert.Equal(projectCustomisation.AssignedToId, project.AssignedToId);
            Assert.Equal(projectCustomisation.AssignedAt, project.AssignedAt);
            Assert.Equal(projectCustomisation.EstablishmentSharepointLink, project.EstablishmentSharepointLink);
            Assert.Equal(projectCustomisation.IncomingTrustSharepointLink, project.IncomingTrustSharepointLink);
            Assert.Equal(projectCustomisation.AdvisoryBoardDate, project.AdvisoryBoardDate);
            Assert.Equal(projectCustomisation.AdvisoryBoardConditions, project.AdvisoryBoardConditions);
            Assert.Equal(projectCustomisation.SignificantDate, project.SignificantDate);
            Assert.Equal(projectCustomisation.SignificantDateProvisional, project.SignificantDateProvisional);
            Assert.Equal(projectCustomisation.TwoRequiresImprovement, project.TwoRequiresImprovement);
            Assert.Equal(projectCustomisation.NewTrustName, project.NewTrustName);
            Assert.Equal(projectCustomisation.NewTrustReferenceNumber, project.NewTrustReferenceNumber);
            Assert.Equal(projectCustomisation.DirectiveAcademyOrder, project.DirectiveAcademyOrder);

            Assert.Equal(handoverComment, project.Notes.FirstOrDefault()?.Body);
            Assert.Equal(projectCustomisation.LocalAuthorityId, project.LocalAuthorityId);

        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
        public void Factory_CreateTransferProject_ShouldCorrectlySetFields(Project projCustomisation)
        {
            // Arrange
            var handoverComment = "handover comment";

            // Act
            var project = Project.CreateTransferProject(
                projCustomisation.Id, 
                projCustomisation.Urn!,
                projCustomisation.CreatedAt,
                projCustomisation.UpdatedAt,
                projCustomisation.TasksDataType!.Value,
                projCustomisation.Type!.Value,
                projCustomisation.TasksDataId!.Value,
                projCustomisation.Region,
                projCustomisation.Team!.Value,
                projCustomisation.RegionalDeliveryOfficerId,
                projCustomisation.AssignedToId,
                projCustomisation.AssignedAt,
                projCustomisation.IncomingTrustUkprn,
                projCustomisation.OutgoingTrustUkprn,
                projCustomisation.GroupId,
                projCustomisation.EstablishmentSharepointLink,
                projCustomisation.IncomingTrustSharepointLink,
                projCustomisation.OutgoingTrustSharepointLink,
                projCustomisation.AdvisoryBoardDate.Value,
                projCustomisation.AdvisoryBoardConditions,
                projCustomisation.SignificantDate!.Value,
                projCustomisation.SignificantDateProvisional!.Value,
                projCustomisation.TwoRequiresImprovement!.Value,
                handoverComment, 
                projCustomisation.LocalAuthorityId.Value
                );

            // Assert
            Assert.Equal(projCustomisation.Urn, project.Urn);
            Assert.Equal(projCustomisation.CreatedAt, project.CreatedAt);
            Assert.Equal(projCustomisation.UpdatedAt, project.UpdatedAt);
            Assert.Equal(projCustomisation.TasksDataType, project.TasksDataType);
            Assert.Equal(projCustomisation.Type, project.Type);
            Assert.Equal(projCustomisation.TasksDataId, project.TasksDataId);
            Assert.Equal(projCustomisation.SignificantDate, project.SignificantDate);
            Assert.Equal(projCustomisation.SignificantDateProvisional, project.SignificantDateProvisional);
            Assert.Equal(projCustomisation.IncomingTrustUkprn, project.IncomingTrustUkprn);
            Assert.Equal(projCustomisation.Region, project.Region);
            Assert.Equal(projCustomisation.TwoRequiresImprovement, project.TwoRequiresImprovement);
            Assert.Equal(projCustomisation.AdvisoryBoardDate, project.AdvisoryBoardDate);
            Assert.Equal(projCustomisation.AdvisoryBoardConditions, project.AdvisoryBoardConditions);
            Assert.Equal(projCustomisation.EstablishmentSharepointLink, project.EstablishmentSharepointLink);
            Assert.Equal(projCustomisation.IncomingTrustSharepointLink, project.IncomingTrustSharepointLink);
            Assert.Equal(handoverComment, project.Notes.FirstOrDefault()?.Body);
            Assert.Equal(projCustomisation.LocalAuthorityId, project.LocalAuthorityId);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public void Factory_CreateMatTransferProject_ShouldCorrectlySetFields(Project projectCustomisation)
        {
            // Arrange
            var handoverComment = "MAT Transfer handover comment";

            // Act
            var project = Project.CreateMatTransferProject(
                projectCustomisation.Id,
                projectCustomisation.Urn!,
                projectCustomisation.CreatedAt,
                projectCustomisation.UpdatedAt,
                projectCustomisation.OutgoingTrustUkprn,
                projectCustomisation.TasksDataType!.Value,
                projectCustomisation.Type!.Value,
                projectCustomisation.TasksDataId!.Value,
                projectCustomisation.Region,
                projectCustomisation.Team!.Value,
                projectCustomisation.RegionalDeliveryOfficerId,
                projectCustomisation.AssignedToId,
                projectCustomisation.AssignedAt,
                projectCustomisation.EstablishmentSharepointLink!,
                projectCustomisation.IncomingTrustSharepointLink!,
                projectCustomisation.OutgoingTrustSharepointLink!,
                projectCustomisation.AdvisoryBoardDate!.Value,
                projectCustomisation.AdvisoryBoardConditions!,
                projectCustomisation.SignificantDate!.Value,
                projectCustomisation.SignificantDateProvisional!.Value,
                projectCustomisation.TwoRequiresImprovement!.Value,
                projectCustomisation.NewTrustName!,
                projectCustomisation.NewTrustReferenceNumber!,
                handoverComment,
                projectCustomisation.LocalAuthorityId.Value
            );

            // Assert
            Assert.Equal(projectCustomisation.Urn, project.Urn);
            Assert.Equal(projectCustomisation.CreatedAt, project.CreatedAt);
            Assert.Equal(projectCustomisation.UpdatedAt, project.UpdatedAt);
            Assert.Equal(projectCustomisation.TasksDataType, project.TasksDataType);
            Assert.Equal(projectCustomisation.Type, project.Type);
            Assert.Equal(projectCustomisation.TasksDataId, project.TasksDataId);
            Assert.Equal(projectCustomisation.Region, project.Region);
            Assert.Equal(projectCustomisation.Team, project.Team);
            Assert.Equal(projectCustomisation.RegionalDeliveryOfficerId, project.RegionalDeliveryOfficerId);
            Assert.Equal(projectCustomisation.AssignedToId, project.AssignedToId);
            Assert.Equal(projectCustomisation.AssignedAt, project.AssignedAt);
            Assert.Equal(projectCustomisation.EstablishmentSharepointLink, project.EstablishmentSharepointLink);
            Assert.Equal(projectCustomisation.IncomingTrustSharepointLink, project.IncomingTrustSharepointLink);
            Assert.Equal(projectCustomisation.AdvisoryBoardDate, project.AdvisoryBoardDate);
            Assert.Equal(projectCustomisation.AdvisoryBoardConditions, project.AdvisoryBoardConditions);
            Assert.Equal(projectCustomisation.SignificantDate, project.SignificantDate);
            Assert.Equal(projectCustomisation.SignificantDateProvisional, project.SignificantDateProvisional);
            Assert.Equal(projectCustomisation.TwoRequiresImprovement, project.TwoRequiresImprovement);
            Assert.Equal(projectCustomisation.NewTrustName, project.NewTrustName);
            Assert.Equal(projectCustomisation.NewTrustReferenceNumber, project.NewTrustReferenceNumber);

            Assert.Equal(handoverComment, project.Notes.FirstOrDefault()?.Body);
            Assert.Equal(projectCustomisation.LocalAuthorityId, project.LocalAuthorityId);

        }


    }
}