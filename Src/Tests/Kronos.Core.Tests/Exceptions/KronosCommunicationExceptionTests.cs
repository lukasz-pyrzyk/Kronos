using System;
using System.Reflection;
using Kronos.Core.Model.Exceptions;
using Xunit;

namespace Kronos.Core.Tests.Exceptions
{
    public class KronosCommunicationExceptionTests
    {
        [Fact]
        public void Ctor_CanInitializeException()
        {
            Exception ex = new KronosCommunicationException();

            Assert.NotNull(ex);
        }

        [Fact]
        public void Ctor_CanAssignMessage()
        {
            const string messsage = "lorem ipsum";
            KronosCommunicationException ex = new KronosCommunicationException(messsage);

            Assert.Equal(ex.Message, messsage);
        }

        [Fact]
        public void Ctor_CanAssignMessageAndInnerException()
        {
            const string messsage = "lorem ipsum";
            Exception innerException = new Exception();
            KronosCommunicationException ex = new KronosCommunicationException(messsage, innerException);

            Assert.Equal(ex.Message, messsage);
            Assert.Equal(ex.InnerException, innerException);
        }
    }
}
