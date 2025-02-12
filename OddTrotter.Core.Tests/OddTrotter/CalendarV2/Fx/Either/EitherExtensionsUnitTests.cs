namespace Fx.Either
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
            var exception = new InvalidOperationException();
            var either = Either.Left("asdF").Right<int>();

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => either.Apply(left => throw exception, right => right));
            Assert.AreEqual(exception, leftMapException.InnerException);

            either = Either.Left<string>().Right(42);

            either.Apply(left => throw exception, right => right);
        }

        [TestMethod]
        public void ApplyNoContextRightMapException()
        {
            var exception = new InvalidOperationException();
            var either = Either.Left("asdf").Right<int>();

            either.Apply(left => left, right => throw exception);

            either = Either.Left<string>().Right(42);

            var rightMapException = Assert.ThrowsException<RightMapException>(() => either.Apply(left => left, right => throw exception));
            Assert.AreEqual(exception, rightMapException.InnerException);
        }

        [TestMethod]
        public void ApplyNoContext()
        {
            var either = Either.Left("sadf").Right<int>();

            Assert.AreEqual(4, either.Apply(left => left.Count(), right => right));

            either = Either.Left<string>().Right(42);

            Assert.AreEqual(42, either.Apply(left => left.Count(), right => right));
        }

        [TestMethod]
        public void SelectNullEither()
        {
            Either<string, int> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                .Select((left, context) => left, (right, context) => right, new Nothing()));
        }

        [TestMethod]
        public void SelectNullLeftSelector()
        {
            var either = Either.Left("asdF").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.Select(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                (Func<string, Nothing, string>)null,
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                (right, context) => right, new Nothing()));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.Select(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                (Func<string, Nothing, string>)null,
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                (right, context) => right, new Nothing()));
        }

        [TestMethod]
        public void SelectNullRightSelector()
        {
            var either = Either.Left("asdF").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.Select(
                (left, context) => left,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                (Func<int, Nothing, int>)null,
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                new Nothing()));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.Select(
                (left, context) => left,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                (Func<int, Nothing, int>)null,
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                new Nothing()));
        }

        [TestMethod]
        public void Select()
        {
            var either = Either.Left("asdf").Right<IEnumerable<int>>();
            var tuple = new TupleBuilder<StringBuilder, IEnumerable<int>>();
 
            IEither<StringBuilder, IEnumerable<int>> result = either.Select((left, context) => context.Item1 = new StringBuilder(left), (right, context) => context.Item2 = right.Select(val => val * 2), tuple);

            Assert.IsNotNull(tuple.Item1);
            Assert.IsNull(tuple.Item2);

            either = Either.Left<string>().Right(new[] { 42 }.AsEnumerable());
            tuple = new TupleBuilder<StringBuilder, IEnumerable<int>>();

            result = either.Select((left, context) => context.Item1 = new StringBuilder(left), (right, context) => context.Item2 = right.Select(val => val * 2), tuple);

            Assert.IsNull(tuple.Item1);
            Assert.IsNotNull(tuple.Item2);
        }

        [TestMethod]
        public void SelectLeftMapException()
        {
            var either = Either.Left("asdf").Right<IEnumerable<int>>();
            var tuple = new TupleBuilder<StringBuilder, IEnumerable<int>>();
            var invalidOperationException = new InvalidOperationException();

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => either.Select((Func<string, TupleBuilder<StringBuilder, IEnumerable<int>>, string>)((left, context) => throw invalidOperationException), (right, context) => context.Item2 = right, tuple));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);

            either = Either.Left<string>().Right(new[] { 42 }.AsEnumerable());

            either.Select((Func<string, TupleBuilder<StringBuilder, IEnumerable<int>>, string>)((left, context) => throw invalidOperationException), (right, context) => context.Item2 = right, tuple);
        }

        [TestMethod]
        public void SelectRightMapException()
        {
            var either = Either.Left("asdf").Right<IEnumerable<int>>();
            var tuple = new TupleBuilder<StringBuilder, IEnumerable<int>>();
            var invalidOperationException = new InvalidOperationException();

            either.Select((left, context) => context.Item1 = new StringBuilder(left), (Func<IEnumerable<int>, TupleBuilder<StringBuilder, IEnumerable<int>>, int>)((right, context) => throw invalidOperationException), tuple);

            either = Either.Left<string>().Right(new[] { 42 }.AsEnumerable());
            var rightMapException = Assert.ThrowsException<RightMapException>(() => either.Select((left, context) => context.Item1 = new StringBuilder(left), (Func<IEnumerable<int>, TupleBuilder<StringBuilder, IEnumerable<int>>, int>)((right, context) => throw invalidOperationException), tuple));

            Assert.AreEqual(invalidOperationException, rightMapException.InnerException);

        }

        private sealed class TupleBuilder<T1, T2>
        {
            /// <summary>
            /// placeholder
            /// </summary>
            public TupleBuilder()
            {
            }

            public T1? Item1 { get; set; }

            public T2? Item2 { get; set; }
        }

        [TestMethod]
        public void SelectLeftNullEither()
        {
            Either<string, IEnumerable<int>> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            var tuple = new TupleBuilder<string, IEnumerable<int>>();

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                .SelectLeft((left, context) => context.Item1 = left, tuple));
        }

        [TestMethod]
        public void SelectLeftNullLeftSelector()
        {
            var either = Either.Left("safd").Right<IEnumerable<int>>();
            var tuple = new TupleBuilder<StringBuilder, IEnumerable<int>>();

            IEither<StringBuilder, IEnumerable<int>> result = either.SelectLeft((left, context) => context.Item1 = new StringBuilder(left), tuple);

            Assert.IsNotNull(tuple.Item1);
            Assert.IsNull(tuple.Item2);

            either = Either.Left<string>().Right(new[] { 42 }.AsEnumerable());
            tuple = new TupleBuilder<StringBuilder, IEnumerable<int>>();

            result = either.SelectLeft((left, context) => context.Item1 = new StringBuilder(left), tuple);

            Assert.IsNull(tuple.Item1);
            Assert.IsNull(tuple.Item2);
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
