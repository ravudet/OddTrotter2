/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace System.Linq
{
    using Fx.Either;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class FirstOrDefaultFactoryUnitTests
    {
        [TestMethod]
        public void CreateNullEither()
        {
            IEither<string, int> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() => FirstOrDefault.Create(
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                ));
        }

        [TestMethod]
        public void Create()
        {
            var value = "asdf";
            var @default = 42;
            var either = Either.Left(value).Right<int>();

            var firstOrDefault = FirstOrDefault.Create(either);

            Assert.IsTrue(firstOrDefault.TryGetLeft(out var left));
            Assert.AreEqual(value, left);
            Assert.IsFalse(firstOrDefault.TryGetRight(out var right));

            either = Either.Left<string>().Right(@default);

            firstOrDefault = FirstOrDefault.Create(either);

            Assert.IsFalse(firstOrDefault.TryGetLeft(out left));
            Assert.IsTrue(firstOrDefault.TryGetRight(out right));
            Assert.AreEqual(@default, right);
        }
    }
}
