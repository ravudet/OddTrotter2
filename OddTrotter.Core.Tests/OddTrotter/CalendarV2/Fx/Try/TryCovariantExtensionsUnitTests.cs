/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Try
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class TryCovariantExtensionsUnitTests
    {
        [TestMethod]
        public void TryNullTry()
        {
            TryCovariant<string, int> @try =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                @try
#pragma warning restore CS8604 // Possible null reference argument.
                .Try("sadf", out var input));
        }

        [TestMethod]
        public void Try()
        {
            Assert.IsTrue(GetTry().Try(4, out var output));
            Assert.AreEqual("4", output);

            Assert.IsFalse(GetTry().Try(5, out output));
            Assert.IsNull(output);
        }

        private static TryCovariant<int, string> GetTry()
        {
            return MakeString;
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="input"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        [return: MaybeNull]
        private static string MakeString(int input, out bool success)
        {
            if (input % 2 == 0)
            {
                success = true;
                return input.ToString();
            }
            else
            {
                success = false;
                return null;
            }
        }

        [TestMethod]
        public void ToTryNullTry()
        {
            TryCovariant<string, int> @try =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                @try
#pragma warning restore CS8604 // Possible null reference argument.
                .ToTry());
        }

        [TestMethod]
        public void ToTry()
        {
            var @try = GetTry().ToTry();

            Assert.IsTrue(@try(4, out var output));
            Assert.AreEqual("4", output);

            Assert.IsFalse(@try(5, out output));
            Assert.IsNull(output);
        }
    }
}
