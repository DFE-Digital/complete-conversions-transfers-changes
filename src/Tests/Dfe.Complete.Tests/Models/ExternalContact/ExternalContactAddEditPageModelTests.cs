namespace Dfe.Complete.Tests.Models.ExternalContact
{
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using Dfe.AcademiesApi.Client.Contracts;
    using Dfe.Complete.Application.Common.Models;
    using Dfe.Complete.Application.LocalAuthorities.Models;
    using Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;
    using Dfe.Complete.Application.Projects.Models;
    using Dfe.Complete.Application.Services.AcademiesApi;
    using Dfe.Complete.Application.Services.TrustCache;
    using Dfe.Complete.Domain.Enums;
    using Dfe.Complete.Domain.ValueObjects;
    using Dfe.Complete.Models.ExternalContact;
    using Dfe.Complete.Tests.Common.Customizations.Behaviours;
    using Dfe.Complete.Tests.Common.Customizations.Models;
    using Dfe.Complete.Tests.MockData;
    using Dfe.Complete.Utils;
    using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
    using MediatR;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class ExternalContactAddEditPageModelTests
    {
        private class TestExternalContactAddEditPageModel(ITrustCache trustCacheService, ISender sender) : ExternalContactAddEditPageModel(trustCacheService, sender)
        {
            public async Task<string?> PublicGetOrganisationName(ExternalContactType contactType)
            {
                return await GetOrganisationNameAsync(contactType);
            }
        }

        private readonly Mock<ITrustCache> trustCacheService;
        private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));
        private readonly Mock<ISender> mockSender;

        public ExternalContactAddEditPageModelTests()
        {
            trustCacheService = fixture.Freeze<Mock<ITrustCache>>();
            mockSender = fixture.Freeze<Mock<ISender>>();
        }

        [Theory]
        [InlineData(ExternalContactType.IncomingTrust, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.OutgoingTrust, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.HeadTeacher, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.ChairOfGovernors, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.SchoolOrAcademy, "test organisation", "Test Organisation")]
        [InlineData(ExternalContactType.LocalAuthority, "test organisation", "test organisation")]
        [InlineData(ExternalContactType.Solicitor, "test organisation", "test organisation")]
        [InlineData(ExternalContactType.Diocese, "test organisation", "test organisation")]
        [InlineData(ExternalContactType.Other, "test organisation", "test organisation")]
        public async Task CanCallGetOrganisationName(ExternalContactType contactType, string input, string expectedResult)
        {
            // Arrange

            ProjectId projectId = fixture.Create<ProjectId>();
            string urn = "123";
            string incomingTrustUkprn = "101483";
            string localAuthorityCode = fixture.Create<string>();

            var mockTrustDto = fixture.Build<TrustDto>().With(x => x.Name, input).Create();
            var mockEstablishmentDto = fixture.Build<EstablishmentDto>()
                .With(x => x.Name, input)
                .With(x => x.Urn, urn)
                .With(x => x.LocalAuthorityCode, localAuthorityCode)
                .Create();

            var mockLocalAuthorityDto = fixture.Build<LocalAuthorityDto>().With(x => x.Name, input).Create();

            TestExternalContactAddEditPageModel testClass = fixture.Build<TestExternalContactAddEditPageModel>()
               .With(t => t.PageContext, PageDataHelper.GetPageContext())
               .With(t => t.ProjectId, projectId.Value.ToString())
               .With(t => t.Project, fixture.Build<ProjectDto>()
                  .With(t => t.Id, projectId)
                  .With(t => t.Type, ProjectType.Transfer)
                  .With(t => t.EstablishmentName, input)
                  .With(t => t.Urn, new Urn(Convert.ToInt32(urn)))                         
                  .With(t => t.IncomingTrustUkprn, new Ukprn(Convert.ToInt32(incomingTrustUkprn)))
                  .With(t => t.NewTrustReferenceNumber, string.Empty)
                  .With(t => t.NewTrustName, string.Empty)
                  .Create())

               .With(t => t.ExternalContactInput, fixture.Build<OtherExternalContactInputModel>()
                   .With(e => e.SelectedExternalContactType, contactType.ToDescription())
                   .With(e => e.OrganisationDiocese, input)
                   .With(e => e.OrganisationOther, input)
                   .With(e => e.OrganisationSolicitor, input)
                   .Without(e => e.ContactTypeRadioOptions)
                   .Create())
               .Create();

            var establishmentQuery = new GetEstablishmentByUrnRequest(urn);

            mockSender.Setup(s => s.Send(establishmentQuery, It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<EstablishmentDto>.Success(mockEstablishmentDto));

            var laQuery = new GetLocalAuthorityByCodeQuery(localAuthorityCode);

            mockSender.Setup(s => s.Send(laQuery, It.IsAny<CancellationToken>()))
             .ReturnsAsync(Result<LocalAuthorityDto?>.Success(mockLocalAuthorityDto));

            trustCacheService.Setup(x => x.GetTrustAsync(It.IsAny<Ukprn>())).ReturnsAsync(mockTrustDto);

            // Act
            var result = await testClass.PublicGetOrganisationName(contactType);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}