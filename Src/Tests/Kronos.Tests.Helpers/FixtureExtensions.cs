using System;
using Ploeh.AutoFixture;

namespace Kronos.Tests.Helpers
{
    public static class FixtureExtensions
    {
        public static string CreateIpAddress(this Fixture fixture)
        {
            // X.X.X.X
            return $"{GetRandomNumber()}.{GetRandomNumber()}.{GetRandomNumber()}.{GetRandomNumber()}";
        }

        private static int GetRandomNumber()
        {
            Random r = new Random();
            return r.Next(0, 254);
        }
    }
}
