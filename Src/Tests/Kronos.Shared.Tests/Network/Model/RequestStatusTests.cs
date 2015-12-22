using Kronos.Shared.Network.Codes;
using Xunit;

namespace Kronos.Shared.Tests.Network.Model
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
