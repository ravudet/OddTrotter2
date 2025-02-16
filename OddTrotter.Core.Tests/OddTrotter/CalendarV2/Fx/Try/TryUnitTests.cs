/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Try
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class TryUnitTests
    {
        [TestMethod]
        public void ImplicitConversion()
        {
            Foo(int.TryParse);
        }

        private static void Foo(Try<string, int> @try)
        {
        }
    }
}
