/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LeftMapExceptionUnitTests
    {
        [TestMethod]
        public void MessageAndException()
        {
            var message = "this is a different message";
            var innerException = new InvalidOperationException();
            var exception = new LeftMapException(message, innerException);

            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }
    }
}
