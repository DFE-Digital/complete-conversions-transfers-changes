namespace Dfe.Complete.Tests.Pages.Projects.ExternalContacts.New
{
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using Dfe.Complete.Application.Common.Models;
    using Dfe.Complete.Application.Contacts.Commands;
    using Dfe.Complete.Application.Projects.Models;
    using Dfe.Complete.Application.Projects.Queries.GetProject;
    using Dfe.Complete.Application.Services.TrustCache;
    using Dfe.Complete.Domain.Enums;
    using Dfe.Complete.Domain.ValueObjects;
    using Dfe.Complete.Pages.Projects.ExternalContacts.New;
    using Dfe.Complete.Services.Interfaces;
    using Dfe.Complete.Tests.Common.Customizations.Behaviours;
    using Dfe.Complete.Tests.Common.Customizations.Models;
    using Dfe.Complete.Tests.MockData;
    using Dfe.Complete.Utils;
    using DfE.CoreLibs.Testing.AutoFixture.Attributes;
    using DfE.CoreLibs.Testing.AutoFixture.Customizations;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;    
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

    public class CreateExternalContactTests
    {
        private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));
        private readonly Mock<ISender> mockSender;
        private readonly Mock<ILogger<CreateExternalContact>> mockLogger;
        private readonly Mock<IErrorService> mockErrorService;
        private readonly Mock<ITrustCache> mockTrustCacheService;

        public CreateExternalContactTests()
        {
            mockSender = fixture.Freeze<Mock<ISender>>();
            mockLogger = fixture.Freeze<Mock<ILogger<CreateExternalContact>>>();
            mockErrorService = fixture.Freeze<Mock<IErrorService>>();
            mockTrustCacheService = fixture.Freeze<Mock<ITrustCache>>();
        }

        [Theory]        
        [InlineData(ProjectType.Conversion)]
        [InlineData(ProjectType.Transfer)]
        public async Task OnGetAsync_Loads_Successfully(ProjectType projectType)
        {
            // Arrange            
            ProjectId projectId = fixture.Create<ProjectId>();

            var testClass = fixture.Build<NewExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.SelectedExternalContactType, string.Empty)
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
            Assert.NotNull(testClass.Project);
            Assert.Equal(projectDto.Id, testClass.Project.Id);
            Assert.Equal(ExternalContactType.Other.ToDescription(), testClass.SelectedExternalContactType);
        }

        [Theory]
        [InlineData(ProjectType.Transfer, "headteacher")]       
        [InlineData(ProjectType.Conversion, "headteacher")]      
        public async Task OnPost_ValidModel_ReturnsRedirectResult(ProjectType projectType, string contactType)
        {
            // Arrange             
            ProjectId projectId = fixture.Create<ProjectId>();

            var testClass = fixture.Build<CreateExternalContact>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.SelectedExternalContactType, contactType)
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
               .ReturnsAsync(contactId);

            // Act
            var result = await testClass.OnPostAsync();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Contains($"/projects/{projectId.Value}/external-contacts", redirectResult.Url);
        }

        //[Theory]
        //[InlineData("headteacher", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact?externalcontacttype=headteacher")]
        //[InlineData("incomingtrustceo", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact?externalcontacttype=incomingtrustceo")]
        //[InlineData("outgoingtrustceo", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact?externalcontacttype=outgoingtrustceo")]
        //[InlineData("chairofgovernors", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact?externalcontacttype=chairofgovernors")]
        //[InlineData("other", "ed11d27b-c35f-4c61-b794-b9317a28a30b", "/projects/ed11d27b-c35f-4c61-b794-b9317a28a30b/external-contacts/new/create-contact-type-other")]
        //public void OnPost_Valid_ReturnsRedirectResult(string contactType, string guidValue, string expectedRedirectUrl)
        //{
        //    // Arrange
        //    Guid projectIdGuid = Guid.Parse(guidValue);
        //    ProjectId projectId = new ProjectId(projectIdGuid);
        //    var now = DateTime.UtcNow;

        //    var testClass = new NewExternalContact(mockSender.Object, mockLogger.Object)
        //    {
        //        PageContext = PageDataHelper.GetPageContext(),
        //        ProjectId = guidValue,
        //        SelectedExternalContactType = contactType
        //    };

        //    // Act
        //    var result = testClass.OnPost();

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectResult>(result);
        //    Assert.Equal(expectedRedirectUrl, redirectResult.Url);
        //}


        //[Fact]
        //public void OnPost_WhenNotFoundExceptionThrown_LogsErrorAndReturnsPageResult()
        //{
        //    // Arrange
        //    string invalidContactType = "headteacher_invalid";
        //    Guid projectIdGuid = Guid.NewGuid();
        //    ProjectId projectId = new ProjectId(projectIdGuid);
        //    var now = DateTime.UtcNow;

        //    var testClass = new NewExternalContact(mockSender.Object, mockLogger.Object)
        //    {
        //        PageContext = PageDataHelper.GetPageContext(),
        //        ProjectId = projectIdGuid.ToString(),
        //        SelectedExternalContactType = invalidContactType
        //    };

        //    // Act and Assert            
        //    var result = Assert.Throws<NotFoundException>(() => testClass.OnPost());
        //    string messagePart = $"The selected contact type '{invalidContactType}' is invalid.";

        //    Assert.Equal(messagePart, result.Message);

        //    mockLogger.Verify(x => x.Log(
        //        It.Is<LogLevel>(l => l == LogLevel.Error),
        //        It.IsAny<EventId>(),
        //        It.Is<It.IsAnyType>((v, t) =>
        //            v.ToString().Contains(messagePart)),
        //        It.Is<Exception>(ex => ex.Message == messagePart),
        //        It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
        //    Times.Once);
        //}
    }
}