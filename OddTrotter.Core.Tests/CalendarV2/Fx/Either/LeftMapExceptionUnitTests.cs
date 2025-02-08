﻿/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LeftMapExceptionUnitTests
    {
        [TestMethod]
        public void Default()
        {
            var exception = new LeftMapException();

            Assert.AreEqual(
                $"Exception of type '{typeof(LeftMapException).Namespace}.{typeof(LeftMapException).Name}' was thrown.",
                exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void Message()
        {
            var message = "this is the message";
            var exception = new LeftMapException(message);

            Assert.AreEqual(message, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

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
