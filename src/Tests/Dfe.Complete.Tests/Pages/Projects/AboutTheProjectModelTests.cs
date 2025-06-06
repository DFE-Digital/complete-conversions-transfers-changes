using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Application.Services.AcademiesApi;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects.AboutTheProject;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MediatR;
using Moq;

namespace Dfe.Complete.Tests.Pages.Projects
{
    public class AboutTheProjectModelTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task OnGet_When_AcademyUrn_IsSupplied_But_Invalid_ThrowsException([Frozen] Mock<ISender> mockSender)
        {
            var projectIdGuid = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var model = new AboutTheProjectModel(mockSender.Object);
            model.ProjectId = projectIdGuid.ToString();

            var project = new ProjectDto
            {
                Id = new ProjectId(projectIdGuid),
                Urn = new Urn(133274),
                AcademyUrn = new Urn(123456),
                CreatedAt = now,
                UpdatedAt = now
            };

            var getProjectByIdQuery = new GetProjectByIdQuery(project.Id);

            mockSender.Setup(s => s.Send(getProjectByIdQuery, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectDto?>.Success(project));

            var establishment = new EstablishmentDto
            {
                Ukprn = "10060668",
                Urn = project.Urn.Value.ToString(),
                Name = "Park View Primary School",
            };

            mockSender.Setup(s => s.Send(new GetEstablishmentByUrnRequest(project.Urn.Value.ToString()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EstablishmentDto>.Success(establishment));

            mockSender.Setup(s => s.Send(new GetEstablishmentByUrnRequest(project.AcademyUrn.Value.ToString()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EstablishmentDto>.Failure("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => model.OnGet());

            Assert.Equal($"Academy {project.AcademyUrn.Value} does not exist.", ex.Message);
        }
    }
}
