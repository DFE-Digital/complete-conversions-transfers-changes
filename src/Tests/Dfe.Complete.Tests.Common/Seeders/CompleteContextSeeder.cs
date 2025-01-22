using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Tests.Common.Seeders;

public static class CompleteContextSeeder
{
    //TODO: implement when needed

    public static void Seed(CompleteContext context)
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

        context.ProjectGroups.Add(projectGroup);
        context.Users.Add(projectUser);
        context.SaveChanges();
    }
}