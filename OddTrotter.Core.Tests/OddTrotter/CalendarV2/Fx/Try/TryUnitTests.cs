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

        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="try"></param>
        private static void Foo(Try<string, int> @try)
        {
        }
    }
}
