/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Try
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class TryCovariantUnitTests
    {
        [TestMethod]
        public void CovarianceConversion()
        {
            var data = new TryCovariant<Shape, Dog>[0];
            Foo(data);
        }

        private class Shape
        {
        }

        private class Rectangle : Shape
        {
        }

        private class Animal
        {
        }

        private class Dog : Animal
        {
        }

        private static void Foo(IEnumerable<TryCovariant<Rectangle, Animal>> data)
        {
        }
    }
}
