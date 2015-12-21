using Kronos.Shared.Network;
using Xunit;

namespace Kronos.Shared.Tests.Network
{
    public class RequestStatusTests
    {
        [Theory]
        [InlineData(RequestStatusCode.Ok, 0)]
        [InlineData(RequestStatusCode.Failed, 1)]
        public void RequestStatusContainsGoodStatusCodes(RequestStatusCode status, int expectedValue)
        {   
            int statusCode = (int) status;

            Assert.Equal(statusCode, expectedValue);
        }
    }
}
