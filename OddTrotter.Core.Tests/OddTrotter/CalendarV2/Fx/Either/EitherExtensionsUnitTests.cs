namespace Fx.Either
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class EitherExtensionsUnitTests
    {
        [TestMethod]
        public void ApplyNoContextNullEither()
        {
            Either<string, int> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                    .Apply(left => left.ToString().Count(), right => right));
        }

        [TestMethod]
        public void ApplyNoContextNullLeftMap()
        {
            var either = Either.Left("sadf").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.Apply(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                right => right));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.Apply(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                right => right));
        }

        [TestMethod]
        public void ApplyNoContextNullRightMap()
        {
            var either = Either.Left("saf").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.Apply(left => left,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.Apply(left => left,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        [TestMethod]
        public void ApplyNoContextLeftMapException()
        {
        }

        [TestMethod]
        public void ApplyNoContextRightMapException()
        {
        }

        [TestMethod]
        public void ApplyNoContext()
        {
        }

        //// TODO have a test that uses the linq query syntax for a select to ensure you have the right method signature
        //// TODO add a comment to the select extension of what haskell operation it is analogous to

        public static string First(Either<Either<short, int>, object> either)
        {
            return either.ToString() ?? string.Empty;
        }

        public static string Second(Either<short, int> either)
        {
            return either.ToString() ?? string.Empty;
        }

        public static string Second(Either<object, System.Exception> either)
        {
            return either.ToString() ?? string.Empty;
        }

        /*public static void Play()
        {
            Either<Either<Either<object, string>, int>, System.Exception> either1 = default!;
            var result =
                from first in either1
                from second in first
                select First(first) + Second(second);

            either1.SelectMany<Either<Either<short, int>, object>, string, Either<short, int>, object>(left => left, (first, second) => First(first) + Second(second));


            Either<short, Either<int, Either<object, System.Exception>>> either2 = default!;
            var result2 =
                from first2 in either2
                from second2 in first2
                select new object();

            //// TODO use linq syntax in a test to assert conformance to the linq requirements
            either2
                .SelectMany
                    <
                        short,
                        Either<int, Either<object, System.Exception>>,
                        int,
                        Either<object, System.Exception>,
                        string
                    >
                    (
                        right => right,
                        (first, second) => First(first) + Second(second));

            Either<Either<short, uint>, Either<int, Either<object, System.Exception>>> either3 = default!;
            var result =
                from first in either3
                from second in either3
                select First(first) + Second(second);
        }
        */
    }
}
