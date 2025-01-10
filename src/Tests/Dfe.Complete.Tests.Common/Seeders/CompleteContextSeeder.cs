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

        context.Users.Add(projectUser);
        context.SaveChanges();
    }
}