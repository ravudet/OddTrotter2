namespace System
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class NothingUnitTests
    {
        [TestMethod]
        public void NothingEquality()
        {
            var first = new Nothing();
            var second = new Nothing();

            Assert.AreEqual(first, second);
        }
    }
}
