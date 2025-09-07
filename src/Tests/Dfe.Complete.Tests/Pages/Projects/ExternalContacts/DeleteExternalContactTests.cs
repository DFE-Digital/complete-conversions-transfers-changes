namespace Dfe.Complete.Tests.Pages.Projects.ExternalContacts.New
{
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using Dfe.Complete.Application.Common.Models;
    using Dfe.Complete.Application.Contacts.Commands;
    using Dfe.Complete.Application.Contacts.Models;    
    using Dfe.Complete.Application.Projects.Queries.GetProject;        
    using Dfe.Complete.Domain.ValueObjects;    
    using Dfe.Complete.Pages.Projects.ExternalContacts;    
    using Dfe.Complete.Tests.Common.Customizations.Behaviours;
    using Dfe.Complete.Tests.Common.Customizations.Models;
    using Dfe.Complete.Tests.MockData;    
    using DfE.CoreLibs.Testing.AutoFixture.Customizations;    
    using MediatR;    
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using Moq;    
    using System.Threading.Tasks;
    
    using Xunit;

    public class DeleteExternalContactTests
    {
        private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));
        private readonly Mock<ISender> mockSender;
        private readonly Mock<ILogger<DeleteExternalContact>> mockLogger;        

        public DeleteExternalContactTests()
        {
            mockSender = fixture.Freeze<Mock<ISender>>();
            mockLogger = fixture.Freeze<Mock<ILogger<DeleteExternalContact>>>();            
        }

        [Fact]                
        public async Task OnGetAsync_Loads_Successfully()
        {
            // Arrange            
            ProjectId projectId = fixture.Create<ProjectId>();
            ContactId contactId = fixture.Create<ContactId>();

            var testClass = fixture.Build<DeleteExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.ContactId, contactId.Value.ToString())               
               .Create();

            var contactDto = fixture.Build<ContactDto>()
              .With(t => t.Id, contactId)
              .With(t => t.ProjectId, projectId)
              .Create();
            
            var getContactByIdQuery = new GetContactByIdQuery(contactDto.Id);

            mockSender.Setup(s => s.Send(getContactByIdQuery, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ContactDto?>.Success(contactDto));

            // Act
            var result = await testClass.OnGetAsync();

            // Assert
            Assert.Multiple(
                () => Assert.Equal(contactDto.Name, testClass.FullName),
                () => Assert.Equal(contactDto.Title, testClass.Role),
                () => Assert.IsType<PageResult>(result)
            );
        }

        [Fact]
        public async Task OnPost_Success_ReturnsRedirectResult()
        {
            // Arrange                              
            ProjectId projectId = fixture.Create<ProjectId>();
            ContactId contactId = fixture.Create<ContactId>();

            var testClass = fixture.Build<DeleteExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.ContactId, contactId.Value.ToString())
               .Create();            
           
            var contactDto = fixture.Build<ContactDto>()
              .With(t => t.Id, contactId)
              .With(t => t.ProjectId, projectId)
              .Create();            


            mockSender.
                Setup(s => s.Send(It.IsAny<DeleteExternalContactCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<bool>.Success(true));

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
            ContactId contactId = fixture.Create<ContactId>();

            var testClass = fixture.Build<DeleteExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.ContactId, contactId.Value.ToString())
               .Create();

            var contactDto = fixture.Build<ContactDto>()
              .With(t => t.Id, contactId)
              .With(t => t.ProjectId, projectId)
              .Create();

            var getContactByIdQuery = new GetContactByIdQuery(contactDto.Id);

            mockSender.Setup(s => s.Send(getContactByIdQuery, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ContactDto?>.Success(contactDto));

            var exceptionMessage = "Error message";
            var exception = new Exception(exceptionMessage);

            mockSender.
                Setup(s => s.Send(It.IsAny<DeleteExternalContactCommand>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(exception);

            // Act
            var result = await testClass.OnPostAsync();           

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
                Times.AtLeastOnce));
        }
    }
}