using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class UserCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize(new IgnoreVirtualMembersCustomisation())
                .Create<User>();
        }
    }
}
