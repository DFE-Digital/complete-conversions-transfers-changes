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
            fixture.Customize<TrustDetailsDto>(composer => composer
             .With(x => x.Ukprn, Ukprn ?? fixture.Create<Ukprn>()));
        }
    }
}
