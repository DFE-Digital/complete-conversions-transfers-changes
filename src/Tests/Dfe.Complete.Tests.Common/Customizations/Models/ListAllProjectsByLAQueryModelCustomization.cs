using AutoFixture;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Tests.Common.Customizations.Models;

public class ListAllProjectsByLAQueryModelCustomization : ICustomization
{
    public required Project Project { get; set; }
    
    public void Customize(IFixture fixture)
    {
        fixture.Customize<ListAllProjectsQueryModel>(composer => composer
            .FromFactory(() =>
            {
                var project = fixture.Create<Project>();
                var giasEstablishment = fixture.Create<GiasEstablishment>();
                return new ListAllProjectsQueryModel(project, giasEstablishment);
            }));
    }
}