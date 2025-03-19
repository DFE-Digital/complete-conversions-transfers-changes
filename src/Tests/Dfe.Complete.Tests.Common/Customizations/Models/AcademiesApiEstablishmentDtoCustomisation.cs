using AutoFixture;
using Dfe.AcademiesApi.Client.Contracts;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models;

public class AcademiesApiEstablishmentDtoCustomisation : ICustomization
{
    public string? Urn { get; set; }

    public NameAndCodeDto? Gor { get; set; }
    public void Customize(IFixture fixture)
    {
        fixture
            .Customize(new CompositeCustomization(
                new DateOnlyCustomization(), new NameAndCodeDtoCustomisation()))
            .Customize<EstablishmentDto>(composer => 
                composer.With(x => x.Urn, new Random().Next(100000, 199999).ToString())
                    .With(x => x.Gor, Gor ?? fixture.Customize(new NameAndCodeDtoCustomisation()).Create<NameAndCodeDto>())
                );
    }
}