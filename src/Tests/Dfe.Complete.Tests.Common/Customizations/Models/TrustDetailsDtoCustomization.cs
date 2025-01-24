using AutoFixture;
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
    public class TrustDetailsDtoCustomization : ICustomization
    {
        public Ukprn? Ukprn { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<GroupContactAddressDto>(composer => composer
             .With(x => x.Street, fixture.Create<string>())
             .With(x => x.Locality, fixture.Create<string>())
             .With(x => x.AdditionalLine, fixture.Create<string>())
             .With(x => x.Town, fixture.Create<string>())
             .With(x => x.County, fixture.Create<string>())
             .With(x => x.Postcode, fixture.Create<string>())
            );

            fixture.Customize<TrustDetailsDto>(composer => composer
             .With(x => x.Ukprn, Ukprn ?? fixture.Create<Ukprn>()));

        }
    }
}
