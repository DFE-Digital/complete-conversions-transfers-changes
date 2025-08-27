namespace Dfe.Complete.Tests.Pages.Projects.ExternalContacts.New
{
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using Dfe.Complete.Application.Common.Models;
    using Dfe.Complete.Application.Projects.Models;
    using Dfe.Complete.Application.Projects.Queries.GetProject;
    using Dfe.Complete.Domain.Enums;
    using Dfe.Complete.Domain.ValueObjects;    
    using Dfe.Complete.Pages.Projects.ExternalContacts.New;
    using Dfe.Complete.Tests.Common.Customizations.Behaviours;
    using Dfe.Complete.Tests.Common.Customizations.Models;
    using Dfe.Complete.Tests.MockData;
    using Dfe.Complete.Utils;
    using DfE.CoreLibs.Testing.AutoFixture.Customizations;
    using MediatR;        
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class ExternalContactBasePageModelTests
    {   
        private readonly Mock<ISender> mockSender;
        private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));

        public ExternalContactBasePageModelTests()
        {
            mockSender = fixture.Freeze<Mock<ISender>>();
        }

        [Theory]        
        [InlineData(ProjectType.Conversion)]
        [InlineData(ProjectType.Transfer)]        
        public async Task OnGetAsync_Loads_Successfully(ProjectType projectType)
        {
            //Arrange                        
            ProjectId projectId = fixture.Create<ProjectId>();

            var testClass = fixture.Build<NewExternalContact>()
                .With(t => t.PageContext, PageDataHelper.GetPageContext())
                .With(t => t.ProjectId, projectId.Value.ToString())
                .Create();

            var projectDto = fixture.Build<ProjectDto>()
              .With(t => t.Id, projectId)
              .With(t => t.Type, projectType)
              .Create();           

            mockSender.Setup(s => s.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectDto?>.Success(projectDto));

            // Act
            var result = await testClass.OnGetAsync();

            // Assert
            Assert.Multiple(
               () => Assert.NotNull(testClass.Project),
               () => Assert.Equal(projectDto.Id, testClass.Project.Id)
            );            
           
        }

        [Theory]
        [InlineData("")]
        [InlineData("0000-0000-0000-0000")]        
        public async Task OnGetAsync_WhenProjectId_IsNotValid_ThrowsError(string projectIdValue)
        {
            // Arrange            
            Guid projectIdGuid = Guid.NewGuid();
            ProjectId projectId = new(projectIdGuid);            

            var testClass = fixture.Build<NewExternalContact>()
                .With(t => t.PageContext, PageDataHelper.GetPageContext())
                .With(t => t.ProjectId, projectIdValue)
                .Create();

            var projectDto = fixture.Build<ProjectDto>()
              .With(t => t.Id, projectId).Create();

            // Act
            var result = await Assert.ThrowsAsync<NotFoundException>(() => testClass.OnGetAsync());

            Assert.Equal($"{projectIdValue} is not a valid Guid.", result.Message);
        }

        [Fact]        
        public async Task OnGetAsyncThrowsExceptionReturnsPageNotFound()
        {
            // Arrange            
            ProjectId projectId = fixture.Create<ProjectId>();           
            
            var testClass = fixture.Build<NewExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .Create();

            var projectDto = fixture.Build<ProjectDto>()
               .With(t => t.Id, projectId).Create();

            mockSender.Setup(s => s.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(Result<ProjectDto?>.Failure("Database error"));

            // Act
            var result = await Assert.ThrowsAsync<NotFoundException>(() => testClass.OnGetAsync());

            Assert.Equal($"Project {projectId.Value} does not exist.", result.Message);
        }
    }
}