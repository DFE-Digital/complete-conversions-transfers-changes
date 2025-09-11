namespace Dfe.Complete.Tests.Pages.Projects.ExternalContacts.New
{
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using Dfe.Complete.Application.Common.Models;
    using Dfe.Complete.Application.Contacts.Commands;
    using Dfe.Complete.Application.Projects.Models;
    using Dfe.Complete.Application.Projects.Queries.GetProject;    
    using Dfe.Complete.Domain.Enums;
    using Dfe.Complete.Domain.ValueObjects;
    using Dfe.Complete.Models.ExternalContact;
    using Dfe.Complete.Pages.Projects.ExternalContacts.New;    
    using Dfe.Complete.Tests.Common.Customizations.Behaviours;
    using Dfe.Complete.Tests.Common.Customizations.Models;
    using Dfe.Complete.Tests.MockData;    
    using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;    
    using MediatR;    
    using Microsoft.AspNetCore.Mvc;    
    using Microsoft.Extensions.Logging;
    using Moq;    
    using System.Threading.Tasks;
    using Xunit;

    public class CreateOtherExternalContactTests
    {
        private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));
        private readonly Mock<ISender> mockSender;
        private readonly Mock<ILogger<CreateOtherExternalContact>> mockLogger;        

        public CreateOtherExternalContactTests()
        {
            mockSender = fixture.Freeze<Mock<ISender>>();
            mockLogger = fixture.Freeze<Mock<ILogger<CreateOtherExternalContact>>>();            
        }

        [Theory]        
        [InlineData(ProjectType.Conversion)]
        [InlineData(ProjectType.Transfer)]
        public async Task OnGetAsync_Loads_Successfully(ProjectType projectType)
        {
            // Arrange            
            ProjectId projectId = fixture.Create<ProjectId>();

            var testClass = fixture.Build<CreateOtherExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())               
               .With(t => t.ExternalContactInput, fixture.Build<OtherExternalContactInputModel>()
                   .With(e => e.SelectedExternalContactType, string.Empty)     
                   .Without(e => e.ContactTypeRadioOptions)
                   .Create())
               .Create();

            var projectDto = fixture.Build<ProjectDto>()
               .With(t => t.Id, projectId)
               .With(t => t.Type, projectType)
               .With(t => t.EstablishmentName, "Test School")
               .Create();            

            var getProjectByIdQuery = new GetProjectByIdQuery(projectDto.Id);

            mockSender.Setup(s => s.Send(getProjectByIdQuery, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectDto?>.Success(projectDto));

            // Act
            var result = await testClass.OnGetAsync();

            // Assert
            Assert.Multiple(
                () => Assert.NotNull(testClass.Project),
                () => Assert.Equal(projectDto.Id, testClass.Project?.Id),                
                () => Assert.Contains(ExternalContactType.SchoolOrAcademy, testClass.ExternalContactInput.ContactTypeRadioOptions!),
                () => Assert.Contains(ExternalContactType.IncomingTrust, testClass.ExternalContactInput.ContactTypeRadioOptions!),                
                () => Assert.Contains(ExternalContactType.LocalAuthority, testClass.ExternalContactInput.ContactTypeRadioOptions!),
                () => Assert.Contains(ExternalContactType.Solicitor, testClass.ExternalContactInput.ContactTypeRadioOptions!),
                () => Assert.Contains(ExternalContactType.Diocese, testClass.ExternalContactInput.ContactTypeRadioOptions!),
                () => Assert.Contains(ExternalContactType.Other, testClass.ExternalContactInput.ContactTypeRadioOptions!)
            );

            if (projectType == ProjectType.Transfer)
            {
                Assert.Contains(ExternalContactType.OutgoingTrust, testClass.ExternalContactInput.ContactTypeRadioOptions!);
                Assert.Equal(7, testClass.ExternalContactInput.ContactTypeRadioOptions?.Length);
            }
            else
            {
                Assert.DoesNotContain(ExternalContactType.OutgoingTrust, testClass.ExternalContactInput.ContactTypeRadioOptions!);
                Assert.Equal(6, testClass.ExternalContactInput.ContactTypeRadioOptions?.Length);
            }
        }

        [Theory]
        [InlineData(ProjectType.Transfer, "schoolacademy")]       
        [InlineData(ProjectType.Conversion, "schoolacademy")]      
        public async Task OnPost_ValidModel_ReturnsRedirectResult(ProjectType projectType, string contactType)
        {
            // Arrange             
            ProjectId projectId = fixture.Create<ProjectId>();
            

            var testClass = fixture.Build<CreateOtherExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.ExternalContactInput, fixture.Build<OtherExternalContactInputModel>()
                   .With(e => e.SelectedExternalContactType, contactType)
                   .With(e => e.IsPrimaryProjectContact, false) 
                   .Create())
               .Create();

            var projectDto = fixture.Build<ProjectDto>()
               .With(t => t.Id, projectId)
               .With(t => t.Type, projectType)
               .With(t => t.EstablishmentName, "Test School")
               .Create();

            var contactId = fixture.Create<ContactId>();

            mockSender.
                Setup(s => s.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectDto?>.Success(projectDto));

            mockSender.
                Setup(s => s.Send(It.IsAny<CreateExternalContactCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<ContactId>.Success(contactId));

            // Act
            var result = await testClass.OnPostAsync();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Contains($"/projects/{projectId.Value}/external-contacts", redirectResult.Url);
        }

        [Fact]       
        public async Task OnPost_WhenExceptionThrown_LogsErrorAndReturnsPageResult()
        {
            // Arrange             
            ProjectId projectId = fixture.Create<ProjectId>();

            var testClass = fixture.Build<CreateOtherExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.ExternalContactInput, fixture.Build<OtherExternalContactInputModel>()
                   .With(e => e.SelectedExternalContactType, "solicitor")
                   .With(e => e.IsPrimaryProjectContact, false) 
                   .Create())
               .Create();

            var projectDto = fixture.Build<ProjectDto>()
               .With(t => t.Id, projectId)
               .With(t => t.Type, ProjectType.Transfer)
               .With(t => t.EstablishmentName, "Test School")
               .Create();

            var contactId = fixture.Create<ContactId>();

            mockSender.
                Setup(s => s.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectDto?>.Success(projectDto));

            var exceptionMessage = "Error message";
            var exception = new Exception(exceptionMessage);
            mockSender
                .Setup(s => s.Send(It.IsAny<CreateExternalContactCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // Act
            await testClass.OnPostAsync();

            // Assert
            Assert.Multiple(
                () => Assert.True(testClass.ModelState.ContainsKey("UnexpectedError")),
                () => Assert.Equal("An unexpected error occurred. Please try again later.", testClass.ModelState["UnexpectedError"]?.Errors[0].ErrorMessage),               
                () => mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once));
        }
    }
}