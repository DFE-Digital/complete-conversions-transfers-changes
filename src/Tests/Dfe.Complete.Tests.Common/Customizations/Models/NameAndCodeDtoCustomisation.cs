using AutoFixture;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models;

public class NameAndCodeDtoCustomisation : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var region = fixture.Create<Region>();
        fixture
            .Customize(new CompositeCustomization(
                new DateOnlyCustomization()))
            .Customize<NameAndCodeDto>(composer => 
                composer.With(x => x.Code, region.GetCharValue())
                    .With(x => x.Name, region.ToDescription())
            );
    }
}