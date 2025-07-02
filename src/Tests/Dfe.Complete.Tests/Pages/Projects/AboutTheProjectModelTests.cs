using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.AboutTheProject;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;

namespace Dfe.Complete.Tests.Pages.Projects
{
    public class AboutTheProjectModelTests
    {
        private UserDto GetUser()
        {
            return new UserDto { ActiveDirectoryUserId = "test-ad-id", FirstName = "Test", LastName = "User", Team = "Support team" };
        }

        private PageContext GetPageContext()
        {
            var expectedUser = GetUser();
            var claims = new List<Claim> { new Claim("objectidentifier", expectedUser?.ActiveDirectoryUserId!) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var httpContext = new DefaultHttpContext()
            {
                User = principal
            };

            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);

            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);

            return new PageContext(actionContext)
            {
                ViewData = viewData
            };
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task OnGet_When_AcademyUrn_IsNotSupplied_ThrowsException([Frozen] Mock<ISender> mockSender)
        {
            var projectIdGuid = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var model = new AboutTheProjectModel(mockSender.Object)
            {
                PageContext = GetPageContext(),
                ProjectId = projectIdGuid.ToString()
            };

            var project = new ProjectDto
            {
                Id = new ProjectId(projectIdGuid),
                Urn = new Urn(133274),
                AcademyUrn = new Urn(123456),
                CreatedAt = now,
                UpdatedAt = now
            };

            var getProjectByIdQuery = new GetProjectByIdQuery(project.Id);

            mockSender.Setup(s => s.Send(getProjectByIdQuery, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectDto?>.Success(project));

            var userDto = GetUser();
            var userResult = Result<UserDto?>.Success(userDto);

            mockSender
                .Setup(s => s.Send(It.Is<GetUserByAdIdQuery>(q => q.UserAdId == userDto.ActiveDirectoryUserId), default))
                .ReturnsAsync(userResult);

            var establishment = new EstablishmentDto
            {
                Ukprn = "10060668",
                Urn = project.Urn.Value.ToString(),
                Name = "Park View Primary School",
            };

            mockSender.Setup(s => s.Send(new GetEstablishmentByUrnRequest(project.Urn.Value.ToString()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EstablishmentDto>.Success(establishment));

            mockSender.Setup(s => s.Send(new GetEstablishmentByUrnRequest(project.AcademyUrn.Value.ToString()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EstablishmentDto>.Failure("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => model.OnGet());

            Assert.Equal($"Academy {project.AcademyUrn.Value} does not exist.", ex.Message);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task OnGet_Loads_Correctly([Frozen] Mock<ISender> mockSender)
        {
            var projectIdGuid = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var model = new AboutTheProjectModel(mockSender.Object)
            {
                PageContext = GetPageContext(),
                ProjectId = projectIdGuid.ToString()
            };

            var project = new ProjectDto
            {
                Id = new ProjectId(projectIdGuid),
                Type = ProjectType.Transfer,
                Urn = new Urn(133274),
                AcademyUrn = new Urn(123456),
                IncomingTrustUkprn = "10058828",
                OutgoingTrustUkprn = "10066101",
                GroupId = new ProjectGroupId(Guid.NewGuid()),
                TasksDataId = new TaskDataId(Guid.NewGuid()),
                CreatedAt = now,
                UpdatedAt = now
            };

            var getProjectByIdQuery = new GetProjectByIdQuery(project.Id);

            mockSender.Setup(s => s.Send(getProjectByIdQuery, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectDto?>.Success(project));

            var establishment = new EstablishmentDto
            {
                Ukprn = "10060668",
                Urn = project.Urn.Value.ToString(),
                Name = "Park View Primary School",
            };
            mockSender.Setup(s => s.Send(new GetEstablishmentByUrnRequest(project.Urn.Value.ToString()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EstablishmentDto>.Success(establishment));

            var academy = new EstablishmentDto
            {
                Ukprn = "10055198",
                Urn = project.AcademyUrn.Value.ToString(),
                Name = "Elmstead Primary School",
            };
            mockSender.Setup(s => s.Send(new GetEstablishmentByUrnRequest(project.AcademyUrn.Value.ToString()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EstablishmentDto>.Success(academy));

            var incomingTrust = new TrustDto
            {
                Name = "Test Incoming Trust",
                Ukprn = project.IncomingTrustUkprn.Value.ToString()
            };
            mockSender.Setup(s => s.Send(new GetTrustByUkprnRequest(project.IncomingTrustUkprn.Value.ToString()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<TrustDto>.Success(incomingTrust));

            var outgoingTrust = new TrustDto
            {
                Name = "Test Outgoing Trust",
                Ukprn = project.OutgoingTrustUkprn.Value.ToString()
            };
            mockSender.Setup(s => s.Send(new GetTrustByUkprnRequest(project.OutgoingTrustUkprn.Value.ToString()), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<TrustDto>.Success(outgoingTrust));

            var userDto = GetUser();
            var userResult = Result<UserDto?>.Success(userDto);

            mockSender
                .Setup(s => s.Send(It.Is<GetUserByAdIdQuery>(q => q.UserAdId == userDto.ActiveDirectoryUserId), default))
                .ReturnsAsync(userResult);

            var group = new ProjectGroupDto
            {
                Id = project.GroupId,
                GroupIdentifier = "GRP_12345678"
            };
            mockSender.Setup(s => s.Send(new GetProjectGroupByIdQuery(project.GroupId), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<ProjectGroupDto>.Success(group));

            var transferTask = new TransferTaskDataDto
            {
                Id = project.TasksDataId,
                CreatedAt = now,
                UpdatedAt = now,
                // The below is not really necessary, but required for code coverage
                // These properties are not yet in use so, failing the code coverage test
                HandoverReview = false,
                HandoverNotes = false,
                HandoverMeeting = false,
                HandoverNotApplicable = false,
                StakeholderKickOffIntroductoryEmails = false,
                StakeholderKickOffSetupMeeting = false,
                StakeholderKickOffMeeting = false,
                MasterFundingAgreementReceived = false,
                MasterFundingAgreementCleared = false,
                MasterFundingAgreementSigned = false,
                MasterFundingAgreementSaved = false,
                MasterFundingAgreementSignedSecretaryState = false,
                MasterFundingAgreementNotApplicable = false,
                DeedOfNovationAndVariationReceived = false,
                DeedOfNovationAndVariationCleared = false,
                DeedOfNovationAndVariationSignedOutgoingTrust = false,
                DeedOfNovationAndVariationSignedIncomingTrust = false,
                DeedOfNovationAndVariationSaved = false,
                DeedOfNovationAndVariationSignedSecretaryState = false,
                DeedOfNovationAndVariationSaveAfterSign = false,
                ArticlesOfAssociationReceived = false,
                ArticlesOfAssociationCleared = false,
                ArticlesOfAssociationSigned = false,
                ArticlesOfAssociationSaved = false,
                ArticlesOfAssociationNotApplicable = false,
                CommercialTransferAgreementConfirmAgreed = false,
                CommercialTransferAgreementConfirmSigned = false,
                CommercialTransferAgreementSaveConfirmationEmails = false,
                SupplementalFundingAgreementReceived = false,
                SupplementalFundingAgreementCleared = false,
                SupplementalFundingAgreementSaved = false,
                DeedOfVariationReceived = false,
                DeedOfVariationCleared = false,
                DeedOfVariationSigned = false,
                DeedOfVariationSaved = false,
                DeedOfVariationSent = false,
                DeedOfVariationSignedSecretaryState = false,
                DeedOfVariationNotApplicable = false,
                LandConsentLetterDrafted = false,
                LandConsentLetterSigned = false,
                LandConsentLetterSent = false,
                LandConsentLetterSaved = false,
                LandConsentLetterNotApplicable = false,
                RpaPolicyConfirm = false,
                FormMReceivedFormM = false,
                FormMReceivedTitlePlans = false,
                FormMCleared = false,
                FormMSigned = false,
                FormMSaved = false,
                ChurchSupplementalAgreementReceived = false,
                ChurchSupplementalAgreementCleared = false,
                ChurchSupplementalAgreementSignedIncomingTrust = false,
                ChurchSupplementalAgreementSignedDiocese = false,
                ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese = false,
                ChurchSupplementalAgreementSignedSecretaryState = false,
                ChurchSupplementalAgreementSavedAfterSigningBySecretaryState = false,
                ChurchSupplementalAgreementNotApplicable = false,
                DeedOfTerminationForTheMasterFundingAgreementReceived = false,
                DeedOfTerminationForTheMasterFundingAgreementCleared = false,
                DeedOfTerminationForTheMasterFundingAgreementSigned = false,
                DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint = false,
                DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam = false,
                DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState = false,
                DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder = false,
                DeedOfTerminationForTheMasterFundingAgreementNotApplicable = false,
                DeedTerminationChurchAgreementReceived = false,
                DeedTerminationChurchAgreementCleared = false,
                DeedTerminationChurchAgreementSignedOutgoingTrust = false,
                DeedTerminationChurchAgreementSignedDiocese = false,
                DeedTerminationChurchAgreementSaved = false,
                DeedTerminationChurchAgreementSignedSecretaryState = false,
                DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState = false,
                DeedTerminationChurchAgreementNotApplicable = false,
                ClosureOrTransferDeclarationNotApplicable = false,
                ClosureOrTransferDeclarationReceived = false,
                ClosureOrTransferDeclarationCleared = false,
                ClosureOrTransferDeclarationSaved = false,
                ClosureOrTransferDeclarationSent = false,
                ConfirmIncomingTrustHasCompletedAllActionsEmailed = false,
                ConfirmIncomingTrustHasCompletedAllActionsSaved = false,
                RedactAndSendDocumentsSendToEsfa = false,
                RedactAndSendDocumentsRedact = false,
                RedactAndSendDocumentsSaved = false,
                RedactAndSendDocumentsSendToFundingTeam = false,
                RedactAndSendDocumentsSendToSolicitors = false,
                RequestNewUrnAndRecordNotApplicable = false,
                RequestNewUrnAndRecordComplete = false,
                RequestNewUrnAndRecordReceive = false,
                RequestNewUrnAndRecordGive = false,
                InadequateOfsted = false,
                FinancialSafeguardingGovernanceIssues = false,
                OutgoingTrustToClose = false,
                BankDetailsChangingYesNo = false,
                CheckAndConfirmFinancialInformationNotApplicable = false,
                CheckAndConfirmFinancialInformationAcademySurplusDeficit = "",
                CheckAndConfirmFinancialInformationTrustSurplusDeficit = "",
                ConfirmDateAcademyTransferredDateTransferred = DateOnly.MinValue,
                SponsoredSupportGrantNotApplicable = false,
                SponsoredSupportGrantType = "",
                DeclarationOfExpenditureCertificateDateReceived = DateOnly.MinValue,
                DeclarationOfExpenditureCertificateCorrect = false,
                DeclarationOfExpenditureCertificateSaved = false,
                DeclarationOfExpenditureCertificateNotApplicable = false,
                ConditionsMetCheckAnyInformationChanged = false,
                ConditionsMetBaselineSheetApproved = false,
                FormMNotApplicable = false,
                ArticlesOfAssociationSent = false,
                CommercialTransferAgreementQuestionsReceived = false,
                CommercialTransferAgreementQuestionsChecked = false
            };
            mockSender.Setup(s => s.Send(new GetTransferTasksDataByIdQuery(project.TasksDataId), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<TransferTaskDataDto>.Success(transferTask));

            //Act
            await model.OnGet();

            // Assert
            Assert.NotNull(model.Project);
            Assert.Equal(project.Id, model.Project.Id);

            Assert.NotNull(model.Establishment);
            Assert.Equal(establishment.Ukprn, model.Establishment.Ukprn);
            Assert.Equal(establishment.Urn, model.Establishment.Urn);

            Assert.NotNull(model.Academy);
            Assert.Equal(academy.Ukprn, model.Academy.Ukprn);
            Assert.Equal(academy.Urn, model.Academy.Urn);

            Assert.NotNull(model.IncomingTrust);
            Assert.Equal(incomingTrust.Ukprn, model.IncomingTrust.Ukprn);

            Assert.NotNull(model.OutgoingTrust);
            Assert.Equal(outgoingTrust.Ukprn, model.OutgoingTrust.Ukprn);

            Assert.NotNull(model.ProjectGroup);
            Assert.Equal(group.Id, model.ProjectGroup.Id);

            Assert.NotNull(model.TransferTaskData);
            Assert.Equal(transferTask.Id, model.TransferTaskData.Id);
        }
    }
}
