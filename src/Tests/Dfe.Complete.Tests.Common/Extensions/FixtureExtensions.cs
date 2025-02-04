using AutoFixture;

namespace Dfe.Complete.Tests.Common.Extensions
{
    public static class FixtureExtensions
    {
        public static int CreateInt(this IFixture fixture, int min, int max)
        {
            return fixture.Create<int>() % (max - min + 1) + min;
        }
    }
}
