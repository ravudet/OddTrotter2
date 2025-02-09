/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class EitherFactoryUnitTests
    {
        [TestMethod]
        public void Test()
        {
            new Either.Full<string>();
        }

        [TestMethod]
        public void Test2()
        {
            Either.Full<string> either = default;
            var result = either.Right<int>();
        }
    }
}
