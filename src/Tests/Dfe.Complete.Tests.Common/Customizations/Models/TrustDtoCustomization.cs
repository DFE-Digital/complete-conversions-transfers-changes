using AutoFixture;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class TrustDtoCustomization : ICustomization
    {
        public string? Ukprn { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<AddressDto>(composer => composer
             .With(x => x.Street, fixture.Create<string>())
             .With(x => x.Locality, fixture.Create<string>())
             .With(x => x.Additional, fixture.Create<string>())
             .With(x => x.Town, fixture.Create<string>())
             .With(x => x.County, fixture.Create<string>())
             .With(x => x.Postcode, fixture.Create<string>())
            );

            fixture.Customize<TrustDto>(composer => composer
             .With(x => x.Ukprn, Ukprn ?? fixture.Create<int>().ToString()));

        }
    }
}
