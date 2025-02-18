using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Tests.Common.Seeders;

public static class CompleteContextSeeder
{
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
        
        var giasEstablishment = fixture.Create<GiasEstablishment>();
        
        var localAuthorities = fixture.CreateMany<LocalAuthority>(10);
        var authorities = localAuthorities.ToList();
        authorities.FirstOrDefault()!.Code = giasEstablishment.LocalAuthorityCode!;
        
        context.GiasEstablishments.Add(giasEstablishment);
        context.LocalAuthorities.AddRange(authorities);
        context.ProjectGroups.Add(projectGroup);
        context.Users.Add(projectUser);
        context.SaveChanges();
    }
}