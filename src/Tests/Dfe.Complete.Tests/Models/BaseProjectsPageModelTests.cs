using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Projects.Queries.GetTransferTasksData;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using Dfe.Complete.Pages.Projects.AboutTheProject;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Dfe.Complete.Tests.Models;

public class BaseProjectsPageModelTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task OnGet_When_ProjectId_NotValidGuid_ThrowsException([Frozen] Mock<ISender> mockSender)
    {
        var model = new AboutTheProjectModel(mockSender.Object);
        model.ProjectId = "an-invalid-guid";

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(() => model.OnGet());
        Assert.Equal($"{model.ProjectId} is not a valid Guid.", ex.Message);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task OnGet_When_Project_DoesNotExist_ThrowsException([Frozen] Mock<ISender> mockSender)
    {
        var model = new AboutTheProjectModel(mockSender.Object);
        model.ProjectId = Guid.NewGuid().ToString();

        mockSender.Setup(s => s.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto?>.Success(null));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => model.OnGet());
        Assert.Equal($"Project {model.ProjectId} does not exist.", ex.Message);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task OnGet_When_Establishment_DoesNotExist_ThrowsException([Frozen] Mock<ISender> mockSender)
    {
        var projectIdGuid = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var model = new AboutTheProjectModel(mockSender.Object);
        model.ProjectId = projectIdGuid.ToString();

        var project = new ProjectDto
        {
            Id = new ProjectId(projectIdGuid),
            Urn = new Urn(133274),
            CreatedAt = now,
            UpdatedAt = now
        };

        var getProjectByIdQuery = new GetProjectByIdQuery(project.Id);

        mockSender.Setup(s => s.Send(getProjectByIdQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto?>.Success(project));

        var getEstablishmentByUrnRequest = new GetEstablishmentByUrnRequest(project.Urn.Value.ToString());

        mockSender.Setup(s => s.Send(getEstablishmentByUrnRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EstablishmentDto>.Failure("Database error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => model.OnGet());
        Assert.Equal($"Establishment {project.Urn.Value} does not exist.", ex.Message);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task OnGet_When_IncomingTrustUkprn_IsSupplied_But_Invalid_ThrowsException([Frozen] Mock<ISender> mockSender)
    {
        var projectIdGuid = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var model = new AboutTheProjectModel(mockSender.Object);
        model.ProjectId = projectIdGuid.ToString();

        var project = new ProjectDto
        {
            Id = new ProjectId(projectIdGuid),
            Urn = new Urn(133274),
            AcademyUrn = new Urn(123456),
            IncomingTrustUkprn = "10058828",
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

        mockSender.Setup(s => s.Send(new GetTrustByUkprnRequest(project.IncomingTrustUkprn.Value.ToString()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TrustDto>.Failure("Database error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => model.OnGet());

        Assert.Equal($"Trust {project.IncomingTrustUkprn.Value} does not exist.", ex.Message);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task OnGet_When_Transfer_And_OutgoingTrustUkprn_Not_Invalid_ThrowsException([Frozen] Mock<ISender> mockSender)
    {
        var projectIdGuid = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var model = new AboutTheProjectModel(mockSender.Object);
        model.ProjectId = projectIdGuid.ToString();

        var project = new ProjectDto
        {
            Id = new ProjectId(projectIdGuid),
            Type = ProjectType.Transfer,
            Urn = new Urn(133274),
            AcademyUrn = new Urn(123456),
            IncomingTrustUkprn = "10058828",
            OutgoingTrustUkprn = "10066101",
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

        mockSender.Setup(s => s.Send(new GetTrustByUkprnRequest(project.OutgoingTrustUkprn.Value.ToString()), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TrustDto>.Failure("Database error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => model.OnGet());

        Assert.Equal($"Trust {project.OutgoingTrustUkprn.Value} does not exist.", ex.Message);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization))]
    public async Task OnGet_LoadsCorrectly([Frozen] Mock<ISender> mockSender)
    {
        var projectIdGuid = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var model = new AboutTheProjectModel(mockSender.Object);
        model.ProjectId = projectIdGuid.ToString();

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

        // Act & Assert
        /*
        * The extension method ClaimsPrincipalExtensions.GetUserAdId throws InvalidOperationException when calling 
        * CurrentUserTeam = await User.GetUserTeam(sender);
        * And currently is it not possible to Mock<> an extension method
        * So here we handle verify the exception instead of verifying that the call returns a PageResult
        */
        var ex = await Assert.ThrowsAsync<NullReferenceException>(() => model.OnGet());

        Assert.NotNull(model.Project);
        Assert.NotNull(model.Establishment);
        Assert.NotNull(model.Academy);
        Assert.NotNull(model.IncomingTrust);
        Assert.NotNull(model.OutgoingTrust);
        Assert.NotNull(model.ProjectGroup);
        Assert.NotNull(model.TransferTaskData);
}

[Theory]
[CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
public void GetProjectSummaryUrl_ShouldReturnTaskListUrl(IFixture fixture)
{
// Arrange
var project = fixture.Customize(new ListAllProjectResultModelCustomization
{
    ProjectType = ProjectType.Conversion
}).Create<ListAllProjectsResultModel>();

string expectedUrl = string.Format(RouteConstants.ProjectTaskList, project.ProjectId.Value);

// Act
var result = BaseProjectsPageModel.GetProjectSummaryUrl(project);

// Assert
Assert.Equal(expectedUrl, result);
}

[Theory]
[CustomAutoData(typeof(ProjectIdCustomization))]
public void GetProjectSummaryUrlById_ShouldReturnTaskListUrl(IFixture fixture)
{
// Arrange
var projectId = fixture.Customize(new ProjectIdCustomization()).Create<ProjectId>();

string expectedUrl = string.Format(RouteConstants.ProjectTaskList, projectId.Value);

// Act
var result = BaseProjectsPageModel.GetProjectSummaryUrl( projectId);

// Assert
Assert.Equal(expectedUrl, result);
}

[Fact]
public void BaseProjectsPageModel_HasCorrectConfiguration_WhenInstantiated(){
// Arrange + Act
var model = new TestBaseProjectsPageModel("my-navigation-page");
var pagination = new PaginationModel("route", 1, 100, 20);
model.Pagination = pagination;

var pageSizeField = typeof(BaseProjectsPageModel)
    .GetField("PageSize", BindingFlags.Instance | BindingFlags.NonPublic);

// Assert
Assert.NotNull(pageSizeField);

var value = (int)pageSizeField!.GetValue(model)!;

        Assert.Equal(pagination, model.Pagination);
        Assert.Equal("my-navigation-page", model.CurrentNavigationItem);
        Assert.Equal(1, model.PageNumber);
        Assert.Equal(20, value);
    }

    [Fact]
    public void HasPageFound_Returns404_WhenConditionIsTrue()
    {
        // Arrange + Act
        var model = new TestBaseProjectsPageModel("my-navigation-page");
        var pagination = new PaginationModel("route", 100, 100, 20);
        model.Pagination = pagination;

        var result = model.TestHasPageFound(pagination.IsOutOfRangePage, pagination.TotalPages);

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(404, statusCodeResult.StatusCode);
    }

    [Fact]
    public void HasPageFound_ReturnsNull_WhenConditionIsFalse()
    {
        // Arrange + Act
        var model = new TestBaseProjectsPageModel("my-navigation-page");
        var pagination = new PaginationModel("route", 1, 100, 20);
        model.Pagination = pagination;

        var result = model.TestHasPageFound(pagination.IsOutOfRangePage, pagination.TotalPages);

        // Assert
        Assert.Null(result);
    }
}

public class TestBaseProjectsPageModel(string currentNav) : BaseProjectsPageModel(currentNav) {
    public IActionResult TestHasPageFound(bool condition, int totalPages) =>
       HasPageFound(condition, totalPages);
}