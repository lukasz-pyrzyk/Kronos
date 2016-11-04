using System;
using Kronos.Core.Exceptions;
using Xunit;

namespace Kronos.Core.Tests.Exceptions
{
    public class KronosExceptionTests
    {
        [Fact]
        public void Ctor_CanInitializeException()
        {
            Exception ex = new KronosException();

            Assert.NotNull(ex);
        }

        [Fact]
        public void Ctor_CanAssignMessage()
        {
            const string messsage = "lorem ipsum";
            KronosException ex = new KronosException(messsage);

            Assert.Equal(ex.Message, messsage);
        }

        [Fact]
        public void Ctor_CanAssignMessageAndInnerException()
        {
            const string messsage = "lorem ipsum";
            Exception innerException = new Exception();
            KronosException ex = new KronosException(messsage, innerException);

            Assert.Equal(ex.Message, messsage);
            Assert.Equal(ex.InnerException, innerException);
        }
    }
}
