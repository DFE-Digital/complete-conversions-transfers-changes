using System.Net;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Customizations;
using Dfe.Complete.Tests.Common.Customizations.Commands;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Tests.Common.Customizations.Queries;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;

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

    [Theory]
    [CustomAutoData(typeof(CountAllProjectsQueryCustomization), typeof(ProjectCustomization), typeof(DateOnlyCustomization))]
    public async Task CountAllProjects_Async_ShouldReturnCorrectNumber(
        CustomWebApplicationDbContextFactory<Program> factory,
        CountAllProjectsQuery countAllProjectsQuery,
        ICountAllProjectsClient countAllProjectsClient)
    {
        //todo: when auth is done, add this back in

        var dbContext = factory.GetDbContext<CompleteContext>();
        
        dbContext.Projects.AddRangeAsync()

        // dbContext.Users.Update(testUser);
        // await dbContext.SaveChangesAsync();

        var result = await countAllProjectsClient.Projects_CountAllProjects_Async(
            (ProjectState?)countAllProjectsQuery.ProjectStatus, (ProjectType?)countAllProjectsQuery.Type);

        // Assert.NotNull(result);
        Assert.IsType<int>(result);
    }

    [Theory]
    [CustomAutoData(typeof(ListAllProjectsQueryCustomization))]
    public async Task ListAllProjects_Async_ShouldReturnList(
        ListAllProjectsQuery listAllProjectsQuery,
        IListAllProjectsClient listAllProjectsClient)
    {
        //todo: when auth is done, add this back in
        // factory.TestClaims = [new Claim(ClaimTypes.Role, "API.Write")];

        // var testUserAdId = createConversionProjectCommand.UserAdId;

        // var dbContext = factory.GetDbContext<CompleteContext>();

        // var testUser = await dbContext.Users.FirstOrDefaultAsync();
        // testUser.ActiveDirectoryUserId = testUserAdId;

        // dbContext.Users.Update(testUser);
        // await dbContext.SaveChangesAsync();

        var result = await listAllProjectsClient.Projects_ListAllProjects_Async(
            (ProjectState?)listAllProjectsQuery.ProjectStatus, (ProjectType?)listAllProjectsQuery.Type,
            listAllProjectsQuery.Page, listAllProjectsQuery.Count);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<ListAllProjectsResultModel>>(result);
        foreach (var item in result)
        {
            //Do something to match all the results
        }
    }
}