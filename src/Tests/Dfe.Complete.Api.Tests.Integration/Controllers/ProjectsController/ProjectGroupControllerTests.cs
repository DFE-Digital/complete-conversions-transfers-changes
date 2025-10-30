using AutoFixture;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WireMock;
using System.Net;
using System.Security.Claims;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController
{
    public class ProjectGroupControllerTests
    {
        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task CreateProjectGroupAsync_ShouldCreateProjectGroup(
            CustomWebApplicationDbContextFactory<Program> factory,
            IProjectGroupClient projectGroupsClient,
            IFixture fixture)
        {
            factory.TestClaims = [.. new[] { ApiRoles.ReadRole, ApiRoles.WriteRole }.Select(x => new Claim(ClaimTypes.Role, x))];
            Assert.NotNull(factory.WireMockServer);
            var trustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = "00000000" }).Create<TrustDto>();

            factory.WireMockServer.AddGetWithJsonResponse(string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, 12121212), trustDto);

            var command = new CreateProjectGroupCommand() { GroupReferenceNumber = "GRP_12121212", Ukprn = new Ukprn() { Value = 12121212 } };
            var projectGroupId = await projectGroupsClient.CreateProjectGroupAsync(command, CancellationToken.None);

            Assert.IsType<Guid>(projectGroupId);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task CreateProjectGroupAsyncShould_ProjectGroupAlreadyExistsForIdentifier_ShouldNotCreateProjectGroup(
            CustomWebApplicationDbContextFactory<Program> factory,
            IProjectGroupClient projectGroupsClient)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();

            var groupReferenceNumber = "GRP_12345678";
            var existingProjectGroup = new ProjectGroup()
            {
                Id = new Domain.ValueObjects.ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = groupReferenceNumber,
                TrustUkprn = new Domain.ValueObjects.Ukprn(11111111),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.ProjectGroups.Add(existingProjectGroup);
            await dbContext.SaveChangesAsync();

            var command = new CreateProjectGroupCommand() { GroupReferenceNumber = existingProjectGroup.GroupIdentifier!, Ukprn = new Ukprn() { Value = 12222222 } };

            var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
                await projectGroupsClient.CreateProjectGroupAsync(command, CancellationToken.None));

            Assert.Contains($"Project group with identifier '{existingProjectGroup.GroupIdentifier}' already exists", exception.Message);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(UkprnCustomization)
            )]
        public async Task CreateProjectGroupAsyncShould_ProjectGroupAlreadyExistsForUkprn_ShouldNotCreateProjectGroup(
            CustomWebApplicationDbContextFactory<Program> factory,
            IProjectGroupClient projectGroupsClient,
            IFixture _fixture)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();

            var existingProjectGroup = new ProjectGroup()
            {
                Id = new Domain.ValueObjects.ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = "GRP_87654321",
                TrustUkprn = _fixture.Create<Domain.ValueObjects.Ukprn>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.ProjectGroups.Add(existingProjectGroup);
            await dbContext.SaveChangesAsync();

            var command = new CreateProjectGroupCommand() { GroupReferenceNumber = "GRP_87654322", Ukprn = new Ukprn() { Value = existingProjectGroup.TrustUkprn!.Value } };

            var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
                await projectGroupsClient.CreateProjectGroupAsync(command, CancellationToken.None));

            Assert.Contains($"Project group with UKPRN '{existingProjectGroup.TrustUkprn!.Value}' already exists", exception.Message);
        }

        [Theory]
        [CustomAutoData(
            typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(UkprnCustomization)
            )]
        public async Task CreateProjectGroupAsyncShould_TrustDoesNotExistForUkprn_ShouldNotCreateProjectGroup(
            CustomWebApplicationDbContextFactory<Program> factory,
            IProjectGroupClient projectGroupsClient)
        {
            factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole }
                .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var dbContext = factory.GetDbContext<CompleteContext>();

            var existingProjectGroup = new ProjectGroup()
            {
                Id = new Domain.ValueObjects.ProjectGroupId(Guid.NewGuid()),
                GroupIdentifier = "GRP_87654321",
                TrustUkprn = new Domain.ValueObjects.Ukprn(12123456),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.ProjectGroups.Add(existingProjectGroup);
            await dbContext.SaveChangesAsync();

            var command = new CreateProjectGroupCommand() { GroupReferenceNumber = "GRP_87654322", Ukprn = new Ukprn() { Value = 12123457 } };

            var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
                await projectGroupsClient.CreateProjectGroupAsync(command, CancellationToken.None));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, (HttpStatusCode)exception.StatusCode);

            var validationErrors = exception.Response;
            Assert.NotNull(validationErrors);

            Assert.Contains($"no trust with UKPRN 12123457. Check the number you entered is correct", validationErrors);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetProjectGroupByIdAsync_ShouldReturn_A_ProjectGroup(
            CustomWebApplicationDbContextFactory<Program> factory,
            IProjectGroupClient projectGroupClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var projectGroup = fixture.Build<ProjectGroup>()
               .With(pg => pg.Id, new Domain.ValueObjects.ProjectGroupId(Guid.NewGuid()))
               .With(pg => pg.GroupIdentifier, "test-group-identifier")
               .With(pg => pg.TrustUkprn, new Domain.ValueObjects.Ukprn(123456))
               .Create();

            await dbContext.ProjectGroups.AddAsync(projectGroup);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await projectGroupClient.GetProjectGroupByIdAsync(projectGroup.Id.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectGroup.Id.Value, result.Id!.Value);
        }

        [Theory]
        [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
        public async Task GetProjectGroupByDetailsByIdAsync_ShouldReturn_A_ProjectGroupsDetails(
            CustomWebApplicationDbContextFactory<Program> factory,
            IProjectGroupClient projectGroupClient,
            IFixture fixture)
        {
            // Arrange
            factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

            var dbContext = factory.GetDbContext<CompleteContext>();

            var projectGroup = fixture.Build<ProjectGroup>()
                .With(pg => pg.Id, new Domain.ValueObjects.ProjectGroupId(Guid.NewGuid()))
                .With(pg => pg.GroupIdentifier, "test-group-identifier")
                .With(pg => pg.TrustUkprn, new Domain.ValueObjects.Ukprn(123456))
                .Create();

            await dbContext.ProjectGroups.AddAsync(projectGroup);
            await dbContext.SaveChangesAsync();

            // Stub external trust lookup
            var trustDto = fixture
                .Customize(new TrustDtoCustomization { Ukprn = projectGroup.TrustUkprn!.Value.ToString() })
                .Create<TrustDto>();
            factory.WireMockServer!.AddGetWithJsonResponse($"/v4/trust/{trustDto.Ukprn}", trustDto);

            // Stub establishments bulk lookup (no projects -> empty response)
            factory.WireMockServer!.AddGetWithJsonResponse("/v4/establishments/bulk", Array.Empty<EstablishmentDto>());

            // Act
            var result = await projectGroupClient.GetProjectGroupDetailsAsync(projectGroup.Id.Value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectGroup.Id.Value.ToString(), result.GroupId);
        }


    }
}

