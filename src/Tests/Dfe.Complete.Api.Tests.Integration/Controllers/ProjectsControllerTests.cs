using System.Net;
using AutoFixture.Xunit2;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Customizations;
using Dfe.Complete.Tests.Common.Customizations.Commands;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UserId = Dfe.Complete.Domain.ValueObjects.UserId;
using User = Dfe.Complete.Domain.Entities.User;

namespace Dfe.Complete.Api.Tests.Integration.Controllers;

public class ProjectsControllerTests
{
    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(CreateConversionProjectCommandCustomization))]
    public async Task CreateProject_Async_ShouldCreateConversionProject(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateConversionProjectCommand createConversionProjectCommand,
        ICreateProjectClient createProjectClient)
    {
        //todo: when auth is done, add this back in
        // factory.TestClaims = [new Claim(ClaimTypes.Role, "API.Write")];

        var testUserAdId = createConversionProjectCommand.UserAdId;
        
        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstOrDefaultAsync();
        testUser.ActiveDirectoryUserId = testUserAdId;

        dbContext.Users.Update(testUser);
        await dbContext.SaveChangesAsync();
        
        var result = await createProjectClient.Projects_CreateProject_Async(createConversionProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateProject_WithNullRequest_ThrowsException(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateConversionProjectCommand createConversionProjectCommand,
        ICreateProjectClient createProjectClient)
    {
        //todo: when auth is done, add this back in
        // factory.TestClaims = [new Claim(ClaimTypes.Role, "API.Write")];

        createConversionProjectCommand.Urn = null;

        //todo: change exception type? 
        var exception = await Assert.ThrowsAsync<PersonsApiException>(async () =>
            await createProjectClient.Projects_CreateProject_Async(createConversionProjectCommand));
        
        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
    }
}