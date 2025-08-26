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
    using Dfe.Complete.Tests.MockData;
    using Dfe.Complete.Utils;
    using DfE.CoreLibs.Testing.AutoFixture.Attributes;
    using DfE.CoreLibs.Testing.AutoFixture.Customizations;
    using MediatR;
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
        private readonly IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());

        public NewExternalContactTests()
        {
            mockSender = fixture.Freeze<Mock<ISender>>();
            mockLogger = fixture.Freeze<Mock<ILogger<NewExternalContact>>>(); 
        }
        
        [Theory]        
        [InlineData(ProjectType.Conversion, ExternalContactType.ChairOfGovernors)]
        [InlineData(ProjectType.Transfer, ExternalContactType.OutgoingTrustCEO)]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task OnGetAsync_Loads_Successfully(ProjectType projectType, ExternalContactType expectedContactType)
        {
            //Arrange            

            Guid projectIdGuid = Guid.NewGuid();
            ProjectId projectId = new ProjectId(projectIdGuid);
            var now = DateTime.UtcNow;

            var testClass = new NewExternalContact(mockSender.Object, mockLogger.Object)
            {
                PageContext = PageDataHelper.GetPageContext(),
                ProjectId = projectIdGuid.ToString()
            };

            var project = new ProjectDto
            {
                Id = projectId,
                Urn = new Urn(133274),               
                AcademyUrn = new Urn(123456),
                Type = projectType,
                CreatedAt = now,
                UpdatedAt = now
            };

            var getProjectByIdQuery = new GetProjectByIdQuery(project.Id);

            mockSender.Setup(s => s.Send(getProjectByIdQuery, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectDto?>.Success(project));

            // Act
            var result = await testClass.OnGetAsync();

            // Assert
            Assert.NotNull(testClass.Project);
            Assert.Equal(project.Id, testClass.Project.Id);
            Assert.Equal(4, testClass.ContactTypeRadioOptions.Count());
            Assert.Equal(ExternalContactType.Other.ToDescription(), testClass.SelectedExternalContactType);
            Assert.Contains(ExternalContactType.HeadTeacher, testClass.ContactTypeRadioOptions);
            Assert.Contains(ExternalContactType.IncomingTrustCEO, testClass.ContactTypeRadioOptions);
            Assert.Contains(ExternalContactType.Other, testClass.ContactTypeRadioOptions);
            Assert.Contains(expectedContactType, testClass.ContactTypeRadioOptions);
        }

        [Theory]
        [InlineData("")]
        [InlineData("0000-0000-0000-0000")]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task OnGetAsync_WhenProjectId_IsNotValid_ThrowsError(string projectIdSupplied)
        {
            // Arrange            
            Guid projectIdGuid = Guid.NewGuid();
            ProjectId projectId = new ProjectId(projectIdGuid);
            var now = DateTime.UtcNow;

            var testClass = new NewExternalContact(mockSender.Object, mockLogger.Object)
            {
                PageContext = PageDataHelper.GetPageContext(),
                ProjectId = projectIdSupplied
            };

            var project = new ProjectDto
            {
                Id = projectId,
                Urn = new Urn(133274),
                AcademyUrn = new Urn(123456),
                CreatedAt = now,
                UpdatedAt = now
            };

            var getProjectByIdQuery = new GetProjectByIdQuery(project.Id);

            // Act
            var result = await Assert.ThrowsAsync<NotFoundException>(() => testClass.OnGetAsync());

            Assert.Equal($"{projectIdSupplied} is not a valid Guid.", result.Message);
        }

        [Fact]        
        public async Task OnGetAsyncThrowsExceptionReturnsPageNotFound()
        {
            // Arrange            
            Guid projectIdGuid = Guid.NewGuid();
            ProjectId projectId = new ProjectId(projectIdGuid);
            var now = DateTime.UtcNow;

            var testClass = new NewExternalContact(mockSender.Object, mockLogger.Object)
            {
                PageContext = PageDataHelper.GetPageContext(),
                ProjectId = projectIdGuid.ToString()
            };

            var project = new ProjectDto
            {
                Id = projectId,
                Urn = new Urn(133274),
                AcademyUrn = new Urn(123456),
                CreatedAt = now,
                UpdatedAt = now
            };

            var getProjectByIdQuery = new GetProjectByIdQuery(project.Id);

            mockSender.Setup(s => s.Send(getProjectByIdQuery, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(Result<ProjectDto?>.Failure("Database error"));

            // Act
            var result = await Assert.ThrowsAsync<NotFoundException>(() => testClass.OnGetAsync());

            Assert.Equal($"Project {projectIdGuid.ToString()} does not exist.", result.Message);
        }


        //[Theory]
        //[InlineData("headteacher", "/projects/{projectId}/external-contacts/new/create-contact?externalcontacttype=headteacher")]
        //[InlineData("incomingtrustceo", "/projects/{projectId}/external-contacts/new/create-contact?externalcontacttype=incomingtrustceo")]
        //[InlineData("outgoingtrustceo", "/projects/{projectId}/external-contacts/new/create-contact?externalcontacttype=outgoingtrustceo")]
        //[InlineData("chairofgovernors", "/projects/{projectId}/external-contacts/new/create-contact?externalcontacttype=chairofgovernors")]
        //[InlineData("other", "/projects/{projectId}/external-contacts/new/create-contact-type-other")]
        //public void OnPost_RedirectsToCorrectPage_BasedOnContactType(string contactType, string expectedRedirectPage)
        //{
        //    // Arrange
        //    Guid projectIdGuid = Guid.NewGuid();
        //    ProjectId projectId = new ProjectId(projectIdGuid);
        //    var now = DateTime.UtcNow;

        //    var testClass = new NewExternalContact(mockSender.Object, mockLogger.Object)
        //    {
        //        PageContext = PageDataHelper.GetPageContext(),
        //        ProjectId = projectIdGuid.ToString(),
        //        SelectedExternalContactType = contactType
        //    };

        //    // Act
        //    var result = testClass.OnPost() as RedirectToPageResult;
        //    expectedRedirectPage = expectedRedirectPage.Replace("{projectId}", projectIdGuid.ToString());

        //    // Assert
        //    if (string.IsNullOrEmpty(expectedRedirectPage))
        //    {
        //        Assert.Null(result);
        //    }
        //    else
        //    {
        //        Assert.Equal(expectedRedirectPage, result.PageName);
        //    }
        //}
    }
}