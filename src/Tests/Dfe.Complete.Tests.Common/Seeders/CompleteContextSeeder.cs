using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Tests.Common.Seeders;

public static class CompleteContextSeeder
{
    //TODO: implement when needed

    public static void Seed(CompleteContext context, IFixture fixture)
    {
        var projectUser = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Team = ProjectTeam.WestMidlands.ToDescription()
        };

        var projectGroup = new ProjectGroup
        {
            Id = new ProjectGroupId(Guid.NewGuid()),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // var localAuthority = new LocalAuthority
        // {
        //     Id = new LocalAuthorityId(Guid.NewGuid()),
        //     Name = "Local Authority 1",
        //     Code = "Code 1", 
        //     CreatedAt = DateTime.Now,
        //     UpdatedAt = DateTime.Now,
        //     Address1 = "Random address 1", 
        //     AddressPostcode = "random postcode"
        // };

        // context.LocalAuthorities.Add(localAuthority);
        
        var localAuthorities = fixture.CreateMany<LocalAuthority>(10);
        context.LocalAuthorities.AddRange(localAuthorities);
        
        context.ProjectGroups.Add(projectGroup);
        context.Users.Add(projectUser);
        context.SaveChanges();
    }
}