namespace Fx.Either
{

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class EitherExtensionsUnitTests
    {
        /*public static string First(Either<Either<short, int>, object> either)
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

        public static void Play()
        {
            Either<Either<Either<short, int>, object>, System.Exception> either1 = default!;
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
        }*/
    }
}
