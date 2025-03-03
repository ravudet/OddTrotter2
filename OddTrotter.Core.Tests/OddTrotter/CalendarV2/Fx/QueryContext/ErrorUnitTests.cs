/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class ErrorUnitTests
    {
        [TestMethod]
        public void Initialize()
        {
            var errorValue = "asf";
            var error = new Error<string>(errorValue);

            Assert.AreEqual(errorValue, error.Value);
        }

        [TestMethod]
        public void InitializeWithNull()
        {
            string? errorValue = null;
            var error = new Error<string?>(errorValue);

            Assert.AreEqual(errorValue, error.Value);
        }
    }
}
