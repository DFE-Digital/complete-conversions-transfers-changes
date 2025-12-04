using AutoFixture.AutoMoq;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using FluentValidation.Results;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using NSubstitute;

namespace Dfe.Complete.Tests.Pages.Projects.ExternalContacts
{
    public class TestExternalContactPageModel(ISender sender, ITrustCache trustCache) : ExternalContactBasePageModel(sender, trustCache)
    {
        // Exposing protected methods
        public Task<string?> Invoke_GetIncomingTrustNameAsync() => GetIncomingTrustNameAsync();
        public void Invoke_AddValidationErrorsToModelState(ValidationResult result)
            => AddValidationErrorsToModelState(result);

        // Exposing settable Project
        public void SetProject(ProjectDto project) => Project = project;
    }

    public class ExternalContactBasePageModelTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));
        private readonly ISender _sender = Substitute.For<ISender>();
        private readonly ITrustCache _trustCache = Substitute.For<ITrustCache>();

        private TestExternalContactPageModel CreateModel()
        {
            return new TestExternalContactPageModel(_sender, _trustCache);
        }

        [Fact]
        public async Task GetIncomingTrustNameAsync_WhenProjectIsNull_ReturnsNull()
        {
            var model = CreateModel();

            var result = await model.Invoke_GetIncomingTrustNameAsync();

            Assert.Null(result);
        }

        [Fact]
        public async Task GetIncomingTrustNameAsync_WhenUkprnIsNull_ReturnsNewTrustNameTitleCase()
        {
            var newTrustName = _fixture.Create<string>();
            Domain.ValueObjects.Ukprn? incomingTrustUkprn = null;

            var project = _fixture.Build<ProjectDto>()
                .With(x => x.IncomingTrustUkprn, incomingTrustUkprn)
                .With(x => x.NewTrustName, "academy trust")
                .Create();

            var model = CreateModel();
            model.SetProject(project);

            var result = await model.Invoke_GetIncomingTrustNameAsync();

            Assert.Equal("Academy Trust", result);
        }

        [Fact]
        public async Task GetIncomingTrustNameAsync_WhenUkprnExists_ReturnsTrustNameTitleCase()
        {
            var ukprn = "12345678";
            
            var project = _fixture.Build<ProjectDto>()
                .With(x => x.IncomingTrustUkprn, ukprn)
                .Create();

            var trust = _fixture.Build<TrustDto>()
                .With(t => t.Name, "academy trust")
                .Create();

            _trustCache.GetTrustAsync(ukprn).Returns(trust);

            var model = CreateModel();
            model.SetProject(project);

            var result = await model.Invoke_GetIncomingTrustNameAsync();

            Assert.Equal("Academy Trust", result);
        }

        [Fact]
        public async Task GetIncomingTrustNameAsync_WhenTrustNameNull_ReturnsNull()
        {
            var ukprn = "12345678";

            var project = _fixture.Build<ProjectDto>()
                .With(x => x.IncomingTrustUkprn, ukprn)
                .Create();

            var trust = _fixture.Build<TrustDto>()
                .With(t => t.Name, (string?)null)
                .Create();

            _trustCache.GetTrustAsync(ukprn).Returns(trust);

            var model = CreateModel();
            model.SetProject(project);

            var result = await model.Invoke_GetIncomingTrustNameAsync();

            Assert.Null(result);
        }        

        [Fact]
        public void AddValidationErrorsToModelState_AddsErrorsCorrectly()
        {
            var model = CreateModel();

            var errors = new[]
            {
            new ValidationFailure("FieldA", "Error message A"),
            new ValidationFailure("FieldB", "Error message B")
        };

            var validationResult = new ValidationResult(errors);

            model.Invoke_AddValidationErrorsToModelState(validationResult);

            Assert.Equal(2, model.ModelState.ErrorCount);
            Assert.Contains(model.ModelState, kv => kv.Key == "FieldA" && kv.Value?.Errors[0].ErrorMessage == "Error message A");
            Assert.Contains(model.ModelState, kv => kv.Key == "FieldB" && kv.Value?.Errors[0].ErrorMessage == "Error message B");
        }
    }
}
