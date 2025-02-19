/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace System.Linq
{
    using Fx.Either;
    using Fx.Try;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public sealed class EnumerableExtensionsUnitTests
    {
        [TestMethod]
        public void EitherFirstOrDefaultNullSource()
        {
            IEnumerable<string> source =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                source
#pragma warning restore CS8604 // Possible null reference argument.
                .EitherFirstOrDefault());
        }

        [TestMethod]
        public void EitherFirstOrDefaultEmptySource()
        {
            var source = Enumerable.Empty<string>();

            var firstOrDefault = source.EitherFirstOrDefault();

            Assert.IsFalse(firstOrDefault.TryGetLeft(out var first));
            Assert.IsTrue(firstOrDefault.TryGetRight(out var right));
            Assert.IsNull(right);
        }

        [TestMethod]
        public void EitherFirstOrDefaultSingleElement()
        {
            var element = "asfd";
            var source = new[] { element };

            var firstOrDefault = source.EitherFirstOrDefault();

            Assert.IsTrue(firstOrDefault.TryGetLeft(out var left));
            Assert.AreEqual(element, left);
            Assert.IsFalse(firstOrDefault.TryGetRight(out var right));
        }

        [TestMethod]
        public void EitherFirstOrDefaultWithDefaultNullSource()
        {
            IEnumerable<string> source =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                source
#pragma warning restore CS8604 // Possible null reference argument.
                .EitherFirstOrDefault(42));
        }

        [TestMethod]
        public void EitherFirstOrDefaultWithDefaultEmptySource()
        {
            var @default = 42;
            var source = Enumerable.Empty<string>();

            var firstOrDefault = source.EitherFirstOrDefault(@default);

            Assert.IsFalse(firstOrDefault.TryGetLeft(out var left));
            Assert.IsTrue(firstOrDefault.TryGetRight(out var right));
            Assert.AreEqual(@default, right);
        }

        [TestMethod]
        public void EitherFirstOrDefaultWithSingleElement()
        {
            var element = "Asfd";
            var source = new[] { element };

            var firstOrDefault = source.EitherFirstOrDefault(42);

            Assert.IsTrue(firstOrDefault.TryGetLeft(out var left));
            Assert.AreEqual(element, left);
            Assert.IsFalse(firstOrDefault.TryGetRight(out var right));
        }

        [TestMethod]
        public void TrySelectNullSource()
        {
            IEnumerable<string> source =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                source
#pragma warning restore CS8604 // Possible null reference argument.
                .TrySelect(IntTryParse));
        }

        [TestMethod]
        public void TrySelectNullTry()
        {
            var source = new[] { "42" };
            Try<string, int> @try =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() => source.TrySelect(@try));
        }

        private static Try<string, int> IntTryParse { get; } = int.TryParse;
    }
}
