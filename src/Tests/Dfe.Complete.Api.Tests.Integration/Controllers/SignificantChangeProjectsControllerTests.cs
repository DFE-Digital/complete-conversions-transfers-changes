using AutoFixture;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WireMock;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using GiasEstablishment = Dfe.Complete.Domain.Entities.GiasEstablishment;

namespace Dfe.Complete.Api.Tests.Integration.Controllers;

public class SignificantChangeProjectsControllerTests
{
    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(LocalAuthorityCustomization))]
    public async Task CreateSignificantChangeProjectAsync_ShouldCreateProjectAndTaskData(
        CustomWebApplicationDbContextFactory<Program> factory,
        ISignificantChangeProjectsClient significantChangeProjectsClient,
        IFixture fixture)
    {
        factory.TestClaims =
        [
            new Claim(ClaimTypes.Role, ApiRoles.WriteRole),
            new Claim(ClaimTypes.Role, ApiRoles.ReadRole)
        ];

        var dbContext = factory.GetDbContext<CompleteContext>();
        var academyUrn = await GetUnusedUrnAsync(dbContext);

        var command = new CreateSignificantChangeProjectCommand
        {
            AcademyUrn = academyUrn,
            TrustUkprn = 10000001,
            PrepareId = 42,
            DecisionRecordedByEmail = "decision.recorder@education.gov.uk",
            DecisionRecordedByFirstName = "Decision",
            DecisionRecordedByLastName = "Recorder",
            DecisionConditions = "Decision conditions"
        };

        Assert.NotNull(factory.WireMockServer);
        var trustDto = fixture
            .Customize(new TrustDtoCustomization { Ukprn = command.TrustUkprn!.Value.ToString() })
            .Create<TrustDto>();

        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, command.TrustUkprn!.Value),
            trustDto);

        var localAuthority = await dbContext.LocalAuthorities.FirstOrDefaultAsync();
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization
            {
                LocalAuthority = localAuthority,
                Urn = new Dfe.Complete.Domain.ValueObjects.Urn(command.AcademyUrn!.Value)
            })
            .Create<GiasEstablishment>();

        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var significantChangeProjectCountBefore = await dbContext.SignificantChangeProjects.CountAsync();
        var significantTaskDataCountBefore = await dbContext.SignificantChangeProjectTasksData.CountAsync();

        var result = await significantChangeProjectsClient.CreateSignificantChangeProjectAsync(command, default);

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.Equal(significantChangeProjectCountBefore + 1, await dbContext.SignificantChangeProjects.CountAsync());
        Assert.Equal(significantTaskDataCountBefore + 1, await dbContext.SignificantChangeProjectTasksData.CountAsync());

        dbContext.ChangeTracker.Clear();

        var createdProjectId = new Dfe.Complete.Domain.ValueObjects.ProjectId(result.Value!.Value);
        var createdProject = await dbContext.SignificantChangeProjects
            .Include(p => p.SignificantTasksData)
            .SingleOrDefaultAsync(p => p.Id == createdProjectId);

        Assert.NotNull(createdProject);
        Assert.Equal(trustDto.Name, createdProject.TrustName);
        Assert.Equal(command.PrepareId, createdProject.PrepareId);
        Assert.Equal(command.DecisionConditions, createdProject.DecisionConditions);
        Assert.Equal(command.AcademyUrn!.Value, createdProject.AcademyUrn.Value);
        Assert.Equal(command.TrustUkprn!.Value, createdProject.TrustUkprn.Value);
        Assert.Equal(localAuthority.Id.Value, createdProject.LocalAuthorityId.Value);
        Assert.NotNull(createdProject.AssignedToUserId);
        Assert.NotNull(createdProject.AssignedAt);

        var assignedUser = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == createdProject.AssignedToUserId);
        Assert.NotNull(assignedUser);
        Assert.Equal(command.DecisionRecordedByEmail, assignedUser.Email);
        Assert.Equal(command.DecisionRecordedByFirstName, assignedUser.FirstName);
        Assert.Equal(command.DecisionRecordedByLastName, assignedUser.LastName);

        Assert.NotNull(createdProject.SignificantTasksData);
        Assert.Equal(createdProject.Id.Value, createdProject.SignificantTasksData.ProjectId.Value);

        var createdTaskData = await dbContext.SignificantChangeProjectTasksData
            .SingleOrDefaultAsync(x => x.ProjectId == createdProject.Id);

        Assert.NotNull(createdTaskData);
        Assert.Equal(createdProject.Id.Value, createdTaskData.ProjectId.Value);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateSignificantChangeProjectAsync_WithUnauthorizedUser_ShouldReturnForbidden(
        CustomWebApplicationDbContextFactory<Program> factory,
        ISignificantChangeProjectsClient significantChangeProjectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var command = new CreateSignificantChangeProjectCommand
        {
            AcademyUrn = 123456,
            TrustUkprn = 10000001,
            PrepareId = 42,
            DecisionRecordedByEmail = "decision.recorder@education.gov.uk",
            DecisionRecordedByFirstName = "Decision",
            DecisionRecordedByLastName = "Recorder",
            DecisionConditions = "Decision conditions"
        };

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await significantChangeProjectsClient.CreateSignificantChangeProjectAsync(command, default));

        Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)exception.StatusCode);
    }

    private static async Task<int> GetUnusedUrnAsync(CompleteContext dbContext)
    {
        var existingUrnValues = (await dbContext.GiasEstablishments
            .AsNoTracking()
            .ToListAsync())
            .Where(x => x.Urn is not null)
            .Select(x => x.Urn!.Value)
            .ToHashSet();

        var urn = 100000;

        while (existingUrnValues.Contains(urn))
        {
            urn++;
        }

        return urn;
    }
}