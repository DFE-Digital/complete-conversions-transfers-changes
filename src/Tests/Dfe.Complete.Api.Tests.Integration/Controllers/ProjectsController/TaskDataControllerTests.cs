using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Utils;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory; 
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController
{
    public class TaskDataControllerTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetTransferTasksDataByIdAsync_ShouldReturn_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var task = fixture.Build<TransferTasksData>()
                .With(tt => tt.Id, new Domain.ValueObjects.TaskDataId(Guid.NewGuid()))
                .Create();

            await dbContext.TransferTasksData.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await tasksDataClient.GetTransferTasksDataByIdAsync(task.Id.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id.Value, result.Id!.Value);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateHandoverWithDeliveryOfficerTaskDataByProjectIdAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            Complete.Client.Contracts.UpdateHandoverWithDeliveryOfficerTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>(); 

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData); 

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.NotApplicable = true;

            // Act
            await tasksDataClient.UpdateHandoverWithDeliveryOfficerTaskDataByTaskDataIdAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id); 
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.HandoverNotApplicable);
            Assert.Null(existingTaskData.HandoverReview);
            Assert.Null(existingTaskData.HandoverMeeting);
            Assert.Null(existingTaskData.HandoverNotes);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateHandoverWithDeliveryOfficerTaskDataByProjectIdAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateHandoverWithDeliveryOfficerTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>(); 

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData); 

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Transfer;
            command.HandoverMeetings = true;
            command.HandoverReview = false;
            command.HandoverNotes = true; 

            // Act
            await tasksDataClient.UpdateHandoverWithDeliveryOfficerTaskDataByTaskDataIdAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.HandoverNotApplicable);
            Assert.False(existingTaskData.HandoverReview);
            Assert.True(existingTaskData.HandoverMeeting);
            Assert.True(existingTaskData.HandoverNotes);
        } 
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetConversionTasksDataByIdAsync_ShouldReturn_A_TaskData(
        CustomWebApplicationDbContextFactory<Program> factory,
        ITasksDataClient tasksDatClient,
        IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var task = fixture.Build<ConversionTasksData>()
                .With(tt => tt.Id, new Domain.ValueObjects.TaskDataId(Guid.NewGuid()))
                .Create();

            await dbContext.ConversionTasksData.AddAsync(task);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await tasksDatClient.GetConversionTasksDataByIdAsync(task.Id.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id.Value, result.Id!.Value);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateDeedOfVariationTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            Complete.Client.Contracts.UpdateDeedOfVariationTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.Cleared = true;
            command.Saved = false;
            command.Received = true;
            command.Signed = true;
            command.Sent = false;
            command.SignedSecretaryState = true;

            // Act
            await tasksDataClient.UpdateDeedOfVariationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.DeedOfVariationCleared);
            Assert.True(existingTaskData.DeedOfVariationReceived);
            Assert.False(existingTaskData.DeedOfVariationSaved);
            Assert.False(existingTaskData.DeedOfVariationSent);
            Assert.True(existingTaskData.DeedOfVariationSigned);
            Assert.True(existingTaskData.DeedOfVariationSignedSecretaryState);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateDeedOfVariationTaskAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            Complete.Client.Contracts.UpdateDeedOfVariationTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Transfer;
            command.Cleared = true;
            command.Saved = false;
            command.Received = true;
            command.Signed = true;
            command.Sent = false;
            command.SignedSecretaryState = true;

            // Act
            await tasksDataClient.UpdateDeedOfVariationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.DeedOfVariationCleared);
            Assert.True(existingTaskData.DeedOfVariationReceived);
            Assert.False(existingTaskData.DeedOfVariationSaved);
            Assert.False(existingTaskData.DeedOfVariationSent);
            Assert.True(existingTaskData.DeedOfVariationSigned);
            Assert.True(existingTaskData.DeedOfVariationSignedSecretaryState);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateDeedOfNovationAndVariationAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateDeedOfNovationAndVariationTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value }; 
            command.Cleared = true;
            command.Saved = false;
            command.Received = true;
            command.SignedOutgoingTrust = true;
            command.SignedIncomingTrust = false;
            command.SignedSecretaryState = true;
            command.SavedAfterSign = true;

            // Act
            await tasksDataClient.UpdateDeedOfNovationAndVariationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.DeedOfNovationAndVariationCleared);
            Assert.True(existingTaskData.DeedOfNovationAndVariationReceived);
            Assert.False(existingTaskData.DeedOfNovationAndVariationSaved);
            Assert.True(existingTaskData.DeedOfNovationAndVariationSignedOutgoingTrust);
            Assert.False(existingTaskData.DeedOfNovationAndVariationSignedIncomingTrust);
            Assert.True(existingTaskData.DeedOfNovationAndVariationSignedSecretaryState);
            Assert.True(existingTaskData.DeedOfNovationAndVariationSaveAfterSign);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateSupplementalFundingAgreementTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateSupplementalFundingAgreementTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.Cleared = true;
            command.Saved = false;
            command.Received = true;
            command.Signed = true;
            command.Sent = false;
            command.SignedSecretaryState = true;

            // Act
            await tasksDataClient.UpdateSupplementalFundingAgreementTaskAsync(command);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.SupplementalFundingAgreementCleared);
            Assert.True(existingTaskData.SupplementalFundingAgreementReceived);
            Assert.False(existingTaskData.SupplementalFundingAgreementSaved);
            Assert.False(existingTaskData.SupplementalFundingAgreementSent);
            Assert.True(existingTaskData.SupplementalFundingAgreementSigned);
            Assert.True(existingTaskData.SupplementalFundingAgreementSignedSecretaryState);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateSupplementalFundingAgreementTaskAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateSupplementalFundingAgreementTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Transfer;
            command.Cleared = true;
            command.Saved = false;
            command.Received = true;

            // Act
            await tasksDataClient.UpdateSupplementalFundingAgreementTaskAsync(command);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.SupplementalFundingAgreementCleared);
            Assert.True(existingTaskData.SupplementalFundingAgreementReceived);
            Assert.False(existingTaskData.SupplementalFundingAgreementSaved); 
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateArticleOfAssociationTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateArticleOfAssociationTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.NotApplicable = true;

            // Act
            await tasksDataClient.UpdateArticleOfAssociationTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ArticlesOfAssociationNotApplicable);
            Assert.Null(existingTaskData.ArticlesOfAssociationCleared);
            Assert.Null(existingTaskData.ArticlesOfAssociationReceived);
            Assert.Null(existingTaskData.ArticlesOfAssociationSaved);
            Assert.Null(existingTaskData.ArticlesOfAssociationSent);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateRedactAndSendDocumentsTaskAsync_ShouldUpdate_ConversionTaskData(
           CustomWebApplicationDbContextFactory<Program> factory,
           ITasksDataClient tasksDataClient,
           UpdateRedactAndSendDocumentsTaskCommand command,
           IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.Redact = true;
            command.Saved = true; 
            command.SendToSolicitors = true;
            command.Send = false;

            // Act
            await tasksDataClient.UpdateRedactAndSendDocumentsTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.RedactAndSendRedact);
            Assert.True(existingTaskData.RedactAndSendSaveRedaction);
            Assert.False(existingTaskData.RedactAndSendSendRedaction);
            Assert.True(existingTaskData.RedactAndSendSendSolicitors);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateRedactAndSendDocumentsTaskAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateRedactAndSendDocumentsTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Transfer;
            command.Redact = true;
            command.Saved = false;
            command.SendToEsfa = true;
            command.SendToSolicitors = true;
            command.Send = false;

            // Act
            await tasksDataClient.UpdateRedactAndSendDocumentsTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.RedactAndSendDocumentsRedact);
            Assert.False(existingTaskData.RedactAndSendDocumentsSaved);
            Assert.True(existingTaskData.RedactAndSendDocumentsSendToEsfa);
            Assert.False(existingTaskData.RedactAndSendDocumentsSendToFundingTeam);
            Assert.True(existingTaskData.RedactAndSendDocumentsSendToSolicitors);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateConfirmProposedCapacityOfTheAcademyTaskAsync_WithFalseNotApplicable_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateConfirmProposedCapacityOfTheAcademyTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value }; 
            command.SevenToElevenYears = "3";
            command.TwelveOrAboveYears = "3";
            command.ReceptionToSixYears = "3";
            command.NotApplicable = false;

            // Act
            await tasksDataClient.UpdateConfirmProposedCapacityOfTheAcademyTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.ProposedCapacityOfTheAcademyNotApplicable);
            Assert.Equal(existingTaskData.ProposedCapacityOfTheAcademyReceptionToSixYears, command.ReceptionToSixYears);
            Assert.Equal(existingTaskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears, command.TwelveOrAboveYears);
            Assert.Equal(existingTaskData.ProposedCapacityOfTheAcademySevenToElevenYears, command.SevenToElevenYears);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateConfirmProposedCapacityOfTheAcademyTaskAsync_WithTrueNotApplicable_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateConfirmProposedCapacityOfTheAcademyTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.SevenToElevenYears = "3";
            command.TwelveOrAboveYears = "3";
            command.ReceptionToSixYears = "3";
            command.NotApplicable = true;

            // Act
            await tasksDataClient.UpdateConfirmProposedCapacityOfTheAcademyTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.ProposedCapacityOfTheAcademyNotApplicable);
            Assert.Null(existingTaskData.ProposedCapacityOfTheAcademyReceptionToSixYears);
            Assert.Null(existingTaskData.ProposedCapacityOfTheAcademyTwelveOrAboveYears);
            Assert.Null(existingTaskData.ProposedCapacityOfTheAcademySevenToElevenYears);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateDeclarationOfExpenditureCertificateTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateDeclarationOfExpenditureCertificateTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.CheckCertificate = true;
            command.Saved = false;
            command.NotApplicable = false;
            command.DateReceived = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            await tasksDataClient.UpdateDeclarationOfExpenditureCertificateTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.ReceiveGrantPaymentCertificateNotApplicable);
            Assert.True(existingTaskData.ReceiveGrantPaymentCertificateCheckCertificate);
            Assert.NotNull(existingTaskData.ReceiveGrantPaymentCertificateDateReceived);
            Assert.False(existingTaskData.ReceiveGrantPaymentCertificateSaveCertificate); 
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateDeclarationOfExpenditureCertificateTaskAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateDeclarationOfExpenditureCertificateTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Transfer;
            command.CheckCertificate = true;
            command.Saved = false;
            command.NotApplicable = false;
            command.DateReceived = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            await tasksDataClient.UpdateDeclarationOfExpenditureCertificateTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.DeclarationOfExpenditureCertificateSaved);
            Assert.True(existingTaskData.DeclarationOfExpenditureCertificateCorrect);
            Assert.NotNull(existingTaskData.DeclarationOfExpenditureCertificateDateReceived);
            Assert.False(existingTaskData.DeclarationOfExpenditureCertificateNotApplicable);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateConfirmTransferHasAuthorityToProceedTaskAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateConfirmTransferHasAuthorityToProceedTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            var testUser = await dbContext.Users.FirstOrDefaultAsync();
            var establishment = fixture.Create<Domain.Entities.GiasEstablishment>();
            var localAuthority = fixture.Create<Domain.Entities.LocalAuthority>();
            await dbContext.LocalAuthorities.AddAsync(localAuthority);
            await dbContext.GiasEstablishments.AddAsync(establishment);
            await dbContext.LocalAuthorities.AddAsync(localAuthority);
            await dbContext.GiasEstablishments.AddAsync(establishment);
            var project = fixture.Customize(
                new ProjectCustomization()
                {
                    Id = new Domain.ValueObjects.ProjectId(Guid.NewGuid()),
                    Urn = establishment.Urn ?? new Domain.ValueObjects.Urn(123456),
                    LocalAuthorityId = localAuthority.Id,
                    RegionalDeliveryOfficerId = testUser!.Id,
                    TasksDataId = taskData.Id,
                    Type = Domain.Enums.ProjectType.Transfer,
                    AllConditionsMet = false,
                    TasksDataType = Domain.Enums.TaskType.Transfer,
                }
                ).Create<Domain.Entities.Project>();
            dbContext.Projects.Add(project);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.AnyInformationChanged = true;
            command.ConfirmToProceed = true;
            command.BaselineSheetApproved = false;

            // Act
            await tasksDataClient.UpdateConfirmTransferHasAuthorityToProceedTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.ConditionsMetBaselineSheetApproved);
            Assert.True(existingTaskData.ConditionsMetCheckAnyInformationChanged);

            var existingProject = await dbContext.Projects.SingleOrDefaultAsync(x => x.TasksDataId == taskData.Id);
            Assert.NotNull(existingProject);
            Assert.True(existingProject.AllConditionsMet);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateConfirmAllConditionsMetTaskAsync_ShouldUpdate_TransferTaskData(
           CustomWebApplicationDbContextFactory<Program> factory,
           ITasksDataClient tasksDataClient,
           UpdateConfirmAllConditionsMetTaskCommand command,
           IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            var testUser = await dbContext.Users.FirstOrDefaultAsync();
            var establishment = fixture.Create<Domain.Entities.GiasEstablishment>();
            var localAuthority = fixture.Create<Domain.Entities.LocalAuthority>();
            await dbContext.LocalAuthorities.AddAsync(localAuthority);
            await dbContext.GiasEstablishments.AddAsync(establishment);
            await dbContext.LocalAuthorities.AddAsync(localAuthority);
            await dbContext.GiasEstablishments.AddAsync(establishment);
            var project = fixture.Customize(
                new ProjectCustomization()
                {
                    Id = new Domain.ValueObjects.ProjectId(Guid.NewGuid()),
                    Urn = establishment.Urn ?? new Domain.ValueObjects.Urn(123456),
                    LocalAuthorityId = localAuthority.Id,
                    RegionalDeliveryOfficerId = testUser!.Id,
                    TasksDataId = taskData.Id,
                    Type = Domain.Enums.ProjectType.Transfer,
                    AllConditionsMet = false,
                    TasksDataType = Domain.Enums.TaskType.Transfer,
                }
                ).Create<Domain.Entities.Project>();
            dbContext.Projects.Add(project);

            await dbContext.SaveChangesAsync();
            command.ProjectId = new ProjectId { Value = project.Id.Value };
            command.Confirm = true;

            // Act
            await tasksDataClient.UpdateConfirmAllConditionsMetTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear(); 
            var existingProject = await dbContext.Projects.SingleOrDefaultAsync(x => x.Id == project.Id);
            Assert.NotNull(existingProject);
            Assert.True(existingProject.AllConditionsMet);
        }
        
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateConfirmAcademyOpenedDateTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateConfirmAcademyOpenedDateTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value }; 
            command.AcademyOpenedDate = new DateTime(2025, 1, 1);

            // Act
            await tasksDataClient.UpdateConfirmAcademyOpenedDateTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.Equal(existingTaskData.ConfirmDateAcademyOpenedDateOpened, command.AcademyOpenedDate.ToDateOnly());
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateChurchSupplementalAgreementTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateChurchSupplementalAgreementTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Conversion;
            command.NotApplicable = false;
            command.Received = true;
            command.Cleared = true;
            command.Signed = true;
            command.SignedByDiocese = true;
            command.Saved = false;
            command.SentOrSaved = true;
            command.SignedBySecretaryState = true;

            // Act
            await tasksDataClient.UpdateChurchSupplementalAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.ChurchSupplementalAgreementNotApplicable);
            Assert.True(existingTaskData.ChurchSupplementalAgreementReceived); 
            Assert.True(existingTaskData.ChurchSupplementalAgreementCleared);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSigned);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedDiocese);
            Assert.False(existingTaskData.ChurchSupplementalAgreementSaved);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSent);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedSecretaryState);
        }
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateChurchSupplementalAgreementTaskAsync_ShouldUpdate_TransferTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateChurchSupplementalAgreementTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.ProjectType = ProjectType.Transfer;
            command.NotApplicable = false;
            command.Received = true;
            command.Cleared = true;
            command.Signed = true;
            command.SignedByDiocese = true;
            command.Saved = false;
            command.SentOrSaved = true;
            command.SignedBySecretaryState = true;

            // Act
            await tasksDataClient.UpdateChurchSupplementalAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.ChurchSupplementalAgreementNotApplicable);
            Assert.True(existingTaskData.ChurchSupplementalAgreementReceived);
            Assert.True(existingTaskData.ChurchSupplementalAgreementCleared);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedIncomingTrust);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedDiocese);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSavedAfterSigningBySecretaryState);
            Assert.False(existingTaskData.ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese);
            Assert.True(existingTaskData.ChurchSupplementalAgreementSignedSecretaryState);

        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateCommercialTransferAgreementTaskAsync_ShouldUpdate_TransferTaskData(
          CustomWebApplicationDbContextFactory<Program> factory,
          ITasksDataClient tasksDataClient,
          UpdateCommercialAgreementTaskCommand command,
          IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<TransferTasksData>();
            dbContext.TransferTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.Agreed = true;          
            command.QuestionsReceived = true;
            command.QuestionsChecked = true;
            command.Saved = false;
            command.Signed = false;
            command.ProjectType = ProjectType.Transfer;      

            // Act
            await tasksDataClient.UpdateCommercialTransferAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.TransferTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.CommercialTransferAgreementConfirmAgreed);            
            Assert.True(existingTaskData.CommercialTransferAgreementQuestionsReceived);
            Assert.True(existingTaskData.CommercialTransferAgreementQuestionsChecked);
            Assert.False(existingTaskData.CommercialTransferAgreementSaveConfirmationEmails);
            Assert.False(existingTaskData.CommercialTransferAgreementConfirmSigned);            
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateCommercialTransferAgreementTaskAsync_ShouldUpdate_ConversionTaskData(
         CustomWebApplicationDbContextFactory<Program> factory,
         ITasksDataClient tasksDataClient,
         UpdateCommercialAgreementTaskCommand command,
         IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.Agreed = true;
            command.QuestionsReceived = true;
            command.QuestionsChecked = true;
            command.Saved = false;
            command.Signed = false;
            command.ProjectType = ProjectType.Conversion;

            // Act
            await tasksDataClient.UpdateCommercialTransferAgreementTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.True(existingTaskData.CommercialTransferAgreementAgreed);
            Assert.True(existingTaskData.CommercialTransferAgreementQuestionsReceived);
            Assert.True(existingTaskData.CommercialTransferAgreementQuestionsChecked);
            Assert.False(existingTaskData.CommercialTransferAgreementSaved);
            Assert.False(existingTaskData.CommercialTransferAgreementSigned);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task UpdateLandQuestionnaireTaskAsync_ShouldUpdate_ConversionTaskData(
            CustomWebApplicationDbContextFactory<Program> factory,
            ITasksDataClient tasksDataClient,
            UpdateLandQuestionnaireTaskCommand command,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole), new Claim(ClaimTypes.Role, ApiRoles.UpdateRole), new Claim(ClaimTypes.Role, ApiRoles.WriteRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var taskData = fixture.Create<ConversionTasksData>();
            dbContext.ConversionTasksData.Add(taskData);

            await dbContext.SaveChangesAsync();
            command.TaskDataId = new TaskDataId { Value = taskData.Id.Value };
            command.Received = true;
            command.Signed = true;
            command.Saved = false;
            command.Cleared = false;

            // Act
            await tasksDataClient.UpdateLandQuestionnaireTaskAsync(command, default);

            // Assert
            dbContext.ChangeTracker.Clear();
            var existingTaskData = await dbContext.ConversionTasksData.SingleOrDefaultAsync(x => x.Id == taskData.Id);
            Assert.NotNull(existingTaskData);
            Assert.False(existingTaskData.LandQuestionnaireCleared);
            Assert.True(existingTaskData.LandQuestionnaireReceived);
            Assert.True(existingTaskData.LandQuestionnaireSigned);
            Assert.False(existingTaskData.LandQuestionnaireSaved);
        }
    }
}
