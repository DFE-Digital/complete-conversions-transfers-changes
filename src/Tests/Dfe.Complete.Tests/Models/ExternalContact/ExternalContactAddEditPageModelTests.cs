namespace Dfe.Complete.Tests.Models.ExternalContact
{
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using Dfe.AcademiesApi.Client.Contracts;
    using Dfe.Complete.Application.Projects.Models;
    using Dfe.Complete.Application.Services.TrustCache;
    using Dfe.Complete.Domain.Enums;
    using Dfe.Complete.Domain.ValueObjects;
    using Dfe.Complete.Models.ExternalContact;
    using Dfe.Complete.Tests.Common.Customizations.Behaviours;
    using Dfe.Complete.Tests.Common.Customizations.Models;
    using Dfe.Complete.Tests.MockData;
    using Dfe.Complete.Utils;
    using DfE.CoreLibs.Testing.AutoFixture.Customizations;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Threading.Tasks;
    using Xunit;

    public class ExternalContactAddEditPageModelTests
    {
        private class TestExternalContactAddEditPageModel : ExternalContactAddEditPageModel
        {
            public TestExternalContactAddEditPageModel(ITrustCache trustCacheService, ISender sender, ILogger logger) : base(trustCacheService, sender, logger)
            {
            }

            public Task<string> PublicGetOrganisationName(ExternalContactType contactType)
            {
                return base.GetOrganisationName(contactType);
            }
        }
        
        private readonly Mock<ITrustCache> trustCacheService;        
        private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));

        public ExternalContactAddEditPageModelTests()
        {  
            trustCacheService = fixture.Freeze<Mock<ITrustCache>>();                
        }

        [Theory]
        [InlineData(ExternalContactType.IncomingTrust, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.OutgoingTrust, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.HeadTeacher, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.ChairOfGovernors, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.SchoolOrAcademy, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.LocalAuthority, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.Solicitor, "test organisation", "test organisation")]
        [InlineData(ExternalContactType.Diocese, "test organisation", "test organisation")]
        [InlineData(ExternalContactType.Other, "test organisation", "test organisation")]
        public async Task CanCallGetOrganisationName(ExternalContactType contactType, string input, string expectedResult)
        {
            // Arrange
            
            ProjectId projectId = fixture.Create<ProjectId>();
            ContactId contactId = fixture.Create<ContactId>();            
           
            var mockTrustDto = fixture.Build<TrustDto>().With(x => x.Name, input).Create();           

            TestExternalContactAddEditPageModel testClass = fixture.Build<TestExternalContactAddEditPageModel>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.Project, fixture.Build<ProjectDto>()
                  .With(t => t.Id, projectId)
                  .With(t => t.Type, ProjectType.Transfer)
                  .With(t => t.EstablishmentName, input)
                  .Create())
               .With(t => t.ExternalContactInput, fixture.Build<OtherExternalContactInputModel>()
                   .With(e => e.SelectedExternalContactType, contactType.ToDescription())
                   .With(e => e.OrganisationDiocese, input)
                   .With(e => e.OrganisationOther, input)
                   .With(e => e.OrganisationSolicitor, input)
                   .Without(e => e.ContactTypeRadioOptions)
                   .Create())
               .Create();

            trustCacheService.Setup(x => x.GetTrustAsync(It.IsAny<Ukprn>())).ReturnsAsync(mockTrustDto);           

            // Act
            var result = await testClass.PublicGetOrganisationName(contactType);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}