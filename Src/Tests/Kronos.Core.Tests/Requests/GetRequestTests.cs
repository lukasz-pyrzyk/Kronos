using Kronos.Core.Requests;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class GetRequestTests
    {
        [Fact]
        public void ContainsCorrectRequestType()
        {
            GetRequest request = new GetRequest();

            Assert.Equal(request.RequestType, RequestType.GetRequest);
        }

        [Fact]
        public void CanAssignCorrectValuesByConstructor()
        {
            string key = "lorem ipsum";
            GetRequest request = new GetRequest(key);

            Assert.Equal(request.Key, key);
        }
    }
}
