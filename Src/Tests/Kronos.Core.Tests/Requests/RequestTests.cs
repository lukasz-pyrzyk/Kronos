using Kronos.Core.Requests;
using Xunit;

namespace Kronos.Core.Tests.Requests
{
    public class RequestTests
    {
        [Fact]
        public void Ctor_CanAssignRequestType()
        {
            RequestType type = RequestType.GetRequest;
            Request r = new Request { RequestType = type };

            Assert.Equal(type, r.RequestType);
        }
    }
}
