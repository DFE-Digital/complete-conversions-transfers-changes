using System.Net;
using System.Security.Claims;
using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using DfE.CoreLibs.Testing.Mocks.WireMock;
using Microsoft.EntityFrameworkCore;
using Dfe.AcademiesApi.Client.Contracts;
using GiasEstablishment = Dfe.Complete.Domain.Entities.GiasEstablishment;
using User = Dfe.Complete.Domain.Entities.User;
using Project = Dfe.Complete.Domain.Entities.Project;
using ProjectGroup = Dfe.Complete.Domain.Entities.ProjectGroup;
using ConversionTasksData = Dfe.Complete.Domain.Entities.ConversionTasksData;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController;

public class ConversionsControllerTests
{
    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateConversion_ValidRequest_ShouldReturnConversionProjectId(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization() { LocalAuthority = localAuthority })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = giasEstablishment.Urn?.Value?.ToString(),
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test advisory board conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        var establishmentDto = fixture.Create<EstablishmentDto>();
        establishmentDto.Urn = giasEstablishment.Urn?.Value?.ToString();
        establishmentDto.LocalAuthorityCode = localAuthority.Code;

        var trustDto = fixture.Create<TrustDto>();
        trustDto.Ukprn = conversionRequest.IncomingTrustUkprn;

        Assert.NotNull(factory.WireMockServer);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/establishments/{conversionRequest.Urn}", establishmentDto);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/trusts/{conversionRequest.IncomingTrustUkprn}", trustDto);

        // Act
        var result = await projectsClient.CreateConversionAsync(conversionRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ConversionProjectId > 0);

        // Verify side effects
        var createdProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Urn.Value == int.Parse(conversionRequest.Urn));
        Assert.NotNull(createdProject);
        Assert.Equal(Domain.Enums.ProjectState.Inactive, createdProject.State);

        var createdUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == conversionRequest.CreatedByEmail);
        Assert.NotNull(createdUser);
        Assert.Equal(conversionRequest.CreatedByFirstName, createdUser.FirstName);
        Assert.Equal(conversionRequest.CreatedByLastName, createdUser.LastName);

        var createdTasksData = await dbContext.ConversionTasksData.FirstOrDefaultAsync(t => t.Id == createdProject.TasksDataId);
        Assert.NotNull(createdTasksData);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateConversion_ValidRequestWithGroupId_ShouldReturnConversionProjectId(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization() { LocalAuthority = localAuthority })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);

        var existingGroup = await dbContext.ProjectGroups.FirstOrDefaultAsync();
        Assert.NotNull(existingGroup);
        existingGroup.GroupIdentifier = "GRP_12345678";
        existingGroup.TrustUkprn = "12345678";
        dbContext.ProjectGroups.Update(existingGroup);
        await dbContext.SaveChangesAsync();

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = giasEstablishment.Urn?.Value?.ToString(),
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test advisory board conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = false,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345,
            GroupId = "GRP_12345678"
        };

        var establishmentDto = fixture.Create<EstablishmentDto>();
        establishmentDto.Urn = giasEstablishment.Urn?.Value?.ToString();
        establishmentDto.LocalAuthorityCode = localAuthority.Code;

        var trustDto = fixture.Create<TrustDto>();
        trustDto.Ukprn = conversionRequest.IncomingTrustUkprn;

        Assert.NotNull(factory.WireMockServer);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/establishments/{conversionRequest.Urn}", establishmentDto);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/trusts/{conversionRequest.IncomingTrustUkprn}", trustDto);

        // Act
        var result = await projectsClient.CreateConversionAsync(conversionRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ConversionProjectId > 0);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_NoAuthToken_ShouldReturn401(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = []; // No claims = no auth

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.Unauthorized, (HttpStatusCode)exception.StatusCode);

        // Verify no side effects
        var dbContext = factory.GetDbContext<CompleteContext>();
        var projectCount = await dbContext.Projects.CountAsync();
        Assert.Equal(0, projectCount);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_InvalidRole_ShouldReturn401(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, "InvalidRole")];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.Unauthorized, (HttpStatusCode)exception.StatusCode);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_MissingUrn_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = null, // Missing URN
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("urn", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_InvalidUrnFormat_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "12345", // Invalid - not 6 digits
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_UrnNotInGias_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        Assert.NotNull(factory.WireMockServer);
        factory.WireMockServer.AddNotFoundResponse($"/v4/establishments/{conversionRequest.Urn}");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateConversion_DuplicateUrn_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        // Create existing project with same URN
        var existingProject = fixture.Create<Project>();
        existingProject.Urn = new Domain.ValueObjects.Urn(123456);
        existingProject.Type = Domain.Enums.ProjectType.Conversion;
        await dbContext.Projects.AddAsync(existingProject);
        await dbContext.SaveChangesAsync();

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456", // Duplicate URN
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_MissingIncomingTrustUkprn_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = null, // Missing UKPRN
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("incoming_trust_ukprn", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_InvalidIncomingTrustUkprnFormat_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "1234567", // Invalid - not 8 digits
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_IncomingTrustNotInAcademiesApi_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        var establishmentDto = fixture.Create<EstablishmentDto>();
        establishmentDto.Urn = conversionRequest.Urn;

        Assert.NotNull(factory.WireMockServer);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/establishments/{conversionRequest.Urn}", establishmentDto);
        factory.WireMockServer.AddNotFoundResponse($"/v4/trusts/{conversionRequest.IncomingTrustUkprn}");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_AdvisoryBoardDateInFuture_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30)), // Future date
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("advisory_board_date", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_EmptyAdvisoryBoardConditions_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "", // Empty conditions
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("advisory_board_conditions", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_ProvisionalConversionDateNotFirstOfMonth_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 15), // Not first of month
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("provisional_conversion_date", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_InvalidEmailDomain_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@gmail.com", // Wrong domain
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("created_by_email", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_EmptyFirstName_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "", // Empty first name
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("created_by_first_name", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_EmptyLastName_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "", // Empty last name
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("created_by_last_name", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_InvalidGroupIdFormat_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345,
            GroupId = "INVALID_FORMAT" // Invalid format
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
        Assert.Contains("group_id", exception.Response.ToLower());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateConversion_GroupIdTrustMismatch_ShouldReturn400(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var existingGroup = await dbContext.ProjectGroups.FirstOrDefaultAsync();
        Assert.NotNull(existingGroup);
        existingGroup.GroupIdentifier = "GRP_12345678";
        existingGroup.TrustUkprn = "87654321"; // Different UKPRN
        dbContext.ProjectGroups.Update(existingGroup);
        await dbContext.SaveChangesAsync();

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678", // Different from group's trust UKPRN
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345,
            GroupId = "GRP_12345678"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("validation_errors", exception.Response);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization))]
    public async Task CreateConversion_ValidationFailure_ShouldNotCreateAnyRecords(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var initialProjectCount = await dbContext.Projects.CountAsync();
        var initialUserCount = await dbContext.Users.CountAsync();
        var initialTasksCount = await dbContext.ConversionTasksData.CountAsync();

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "12345", // Invalid URN format
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

        // Verify no records were created
        var finalProjectCount = await dbContext.Projects.CountAsync();
        var finalUserCount = await dbContext.Users.CountAsync();
        var finalTasksCount = await dbContext.ConversionTasksData.CountAsync();

        Assert.Equal(initialProjectCount, finalProjectCount);
        Assert.Equal(initialUserCount, finalUserCount);
        Assert.Equal(initialTasksCount, finalTasksCount);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateConversion_ExistingUser_ShouldReuseUser(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        // Create existing user
        var existingUser = fixture.Create<User>();
        existingUser.Email = "existing.user@education.gov.uk";
        existingUser.FirstName = "Existing";
        existingUser.LastName = "User";
        await dbContext.Users.AddAsync(existingUser);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization() { LocalAuthority = localAuthority })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = giasEstablishment.Urn?.Value?.ToString(),
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "existing.user@education.gov.uk", // Same email as existing user
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        var establishmentDto = fixture.Create<EstablishmentDto>();
        establishmentDto.Urn = giasEstablishment.Urn?.Value?.ToString();
        establishmentDto.LocalAuthorityCode = localAuthority.Code;

        var trustDto = fixture.Create<TrustDto>();
        trustDto.Ukprn = conversionRequest.IncomingTrustUkprn;

        Assert.NotNull(factory.WireMockServer);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/establishments/{conversionRequest.Urn}", establishmentDto);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/trusts/{conversionRequest.IncomingTrustUkprn}", trustDto);

        var initialUserCount = await dbContext.Users.CountAsync();

        // Act
        var result = await projectsClient.CreateConversionAsync(conversionRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ConversionProjectId > 0);

        var finalUserCount = await dbContext.Users.CountAsync();
        Assert.Equal(initialUserCount, finalUserCount); // No new user created

        var createdProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Urn.Value == int.Parse(conversionRequest.Urn));
        Assert.NotNull(createdProject);
        Assert.Equal(existingUser.Id, createdProject.CreatedBy);
        Assert.Equal(existingUser.Id, createdProject.AssignedTo);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateConversion_RegionAndLocalAuthorityDerivedFromEstablishment_ShouldSetCorrectly(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization() { LocalAuthority = localAuthority })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = giasEstablishment.Urn?.Value?.ToString(),
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        var establishmentDto = fixture.Create<EstablishmentDto>();
        establishmentDto.Urn = giasEstablishment.Urn?.Value?.ToString();
        establishmentDto.LocalAuthorityCode = localAuthority.Code;

        var trustDto = fixture.Create<TrustDto>();
        trustDto.Ukprn = conversionRequest.IncomingTrustUkprn;

        Assert.NotNull(factory.WireMockServer);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/establishments/{conversionRequest.Urn}", establishmentDto);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/trusts/{conversionRequest.IncomingTrustUkprn}", trustDto);

        // Act
        var result = await projectsClient.CreateConversionAsync(conversionRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ConversionProjectId > 0);

        var createdProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Urn.Value == int.Parse(conversionRequest.Urn));
        Assert.NotNull(createdProject);
        Assert.Equal(localAuthority.Id, createdProject.LocalAuthorityId);
        Assert.Equal(giasEstablishment.Region, createdProject.Region);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateConversion_InternalError_ShouldReturn500WithSpecificMessage(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = "123456",
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345
        };

        // Simulate internal server error by not mocking any API responses
        Assert.NotNull(factory.WireMockServer);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionAsync(conversionRequest));

        Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)exception.StatusCode);
        Assert.Contains("Conversion project could not be created via API", exception.Response);
        Assert.Contains("urn: 123456", exception.Response);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateConversion_NewGroupId_ShouldCreateGroup(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization() { LocalAuthority = localAuthority })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var conversionRequest = new ConversionProjectRequest
        {
            Urn = giasEstablishment.Urn?.Value?.ToString(),
            IncomingTrustUkprn = "12345678",
            AdvisoryBoardDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
            AdvisoryBoardConditions = "Test conditions",
            ProvisionalConversionDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
            DirectiveAcademyOrder = true,
            CreatedByEmail = "test.user@education.gov.uk",
            CreatedByFirstName = "Test",
            CreatedByLastName = "User",
            PrepareId = 12345,
            GroupId = "GRP_98765432" // New group ID
        };

        var establishmentDto = fixture.Create<EstablishmentDto>();
        establishmentDto.Urn = giasEstablishment.Urn?.Value?.ToString();
        establishmentDto.LocalAuthorityCode = localAuthority.Code;

        var trustDto = fixture.Create<TrustDto>();
        trustDto.Ukprn = conversionRequest.IncomingTrustUkprn;

        Assert.NotNull(factory.WireMockServer);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/establishments/{conversionRequest.Urn}", establishmentDto);
        factory.WireMockServer.AddGetWithJsonResponse($"/v4/trusts/{conversionRequest.IncomingTrustUkprn}", trustDto);

        var initialGroupCount = await dbContext.ProjectGroups.CountAsync();

        // Act
        var result = await projectsClient.CreateConversionAsync(conversionRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ConversionProjectId > 0);

        var finalGroupCount = await dbContext.ProjectGroups.CountAsync();
        Assert.Equal(initialGroupCount + 1, finalGroupCount);

        var createdGroup = await dbContext.ProjectGroups.FirstOrDefaultAsync(g => g.GroupIdentifier == conversionRequest.GroupId);
        Assert.NotNull(createdGroup);
        Assert.Equal(conversionRequest.IncomingTrustUkprn, createdGroup.TrustUkprn);

        var createdProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Urn.Value == int.Parse(conversionRequest.Urn));
        Assert.NotNull(createdProject);
        Assert.Equal(createdGroup.Id, createdProject.GroupId);
    }
}

// Request model for the new conversion endpoint
public class ConversionProjectRequest
{
    public string? Urn { get; set; }
    public string? IncomingTrustUkprn { get; set; }
    public DateOnly AdvisoryBoardDate { get; set; }
    public string? AdvisoryBoardConditions { get; set; }
    public DateOnly ProvisionalConversionDate { get; set; }
    public bool DirectiveAcademyOrder { get; set; }
    public string? CreatedByEmail { get; set; }
    public string? CreatedByFirstName { get; set; }
    public string? CreatedByLastName { get; set; }
    public int PrepareId { get; set; }
    public string? GroupId { get; set; }
}

// Response model for the new conversion endpoint
public class ConversionProjectResponse
{
    public int ConversionProjectId { get; set; }
}

// Extension methods for IProjectsClient (these would normally be in the generated client)
public static class ProjectsClientExtensions
{
    public static async Task<ConversionProjectResponse> CreateConversionAsync(this IProjectsClient client, ConversionProjectRequest request)
    {
        // This would be implemented in the actual client
        throw new NotImplementedException("This method would be implemented in the generated API client");
    }
}
