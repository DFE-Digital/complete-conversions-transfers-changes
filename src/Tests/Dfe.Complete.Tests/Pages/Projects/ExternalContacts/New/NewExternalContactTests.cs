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
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;    
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class NewExternalContactTests
    {
        private readonly Mock<ISender> mockSender;
        private readonly Mock<ILogger<NewExternalContact>> mockLogger;
        private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));
      
        public NewExternalContactTests()
        {
            mockSender = fixture.Freeze<Mock<ISender>>();
            mockLogger = fixture.Freeze<Mock<ILogger<NewExternalContact>>>();
        }

        [Theory]        
        [InlineData(ProjectType.Conversion, ExternalContactType.ChairOfGovernors)]
        [InlineData(ProjectType.Transfer, ExternalContactType.OutgoingTrustCEO)]
        public async Task OnGetAsync_Loads_Successfully(ProjectType projectType, ExternalContactType expectedContactType)
        {
            //Arrange            

            ProjectId projectId = fixture.Create<ProjectId>();

            var testClass = fixture.Build<NewExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               //.With(t => t.SelectedExternalContactType, string.Empty)
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
                () => Assert.Equal(projectDto.Id, testClass.Project.Id),
                () => Assert.Equal(4, testClass.ContactTypeRadioOptions.Count()),
                () => Assert.Contains(ExternalContactType.HeadTeacher, testClass.ContactTypeRadioOptions),
                () => Assert.Contains(ExternalContactType.IncomingTrustCEO, testClass.ContactTypeRadioOptions),
                () => Assert.Contains(ExternalContactType.Other, testClass.ContactTypeRadioOptions),
                () => Assert.Contains(expectedContactType, testClass.ContactTypeRadioOptions)
            );
            
        }

        [Theory]
        [InlineData("headteacher", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact/headteacher")]
        [InlineData("incomingtrustceo", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact/incomingtrustceo")]
        [InlineData("outgoingtrustceo", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact/outgoingtrustceo")]
        [InlineData("chairofgovernors", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact/chairofgovernors")]
        [InlineData("other", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact-type-other")]
        public void OnPost_Valid_ReturnsRedirectResult(string contactType, string guidValue, string expectedRedirectUrl)
        {
            // Arrange
            Guid projectIdGuid = Guid.Parse(guidValue);
            ProjectId projectId = new ProjectId(projectIdGuid);
            
            var testClass = fixture.Build<NewExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectIdGuid.ToString())
               .With(t => t.SelectedExternalContactType, contactType)
               .Create();
           
            // Act
            var result = testClass.OnPost();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(expectedRedirectUrl, redirectResult.Url);
        }

        
        [Fact]
        public void OnPost_WhenNotFoundExceptionThrown_LogsErrorAndReturnsPageResult()
        {
            // Arrange
            string invalidContactType = "headteacher_invalid";            

            var testClass = fixture.Build<NewExternalContact>()
              .With(t => t.PageContext, PageDataHelper.GetPageContext())              
              .With(t => t.SelectedExternalContactType, invalidContactType)
              .Create();
            
            // Act and Assert            
            var result = Assert.Throws<NotFoundException>(() => testClass.OnPost());
            string messagePart = $"The selected contact type '{invalidContactType}' is invalid.";

            Assert.Multiple(                
                () => Assert.Equal(messagePart, result.Message),
                () => mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains(messagePart)),
                It.Is<Exception>(ex => ex.Message == messagePart),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once));            
        }
    }
}