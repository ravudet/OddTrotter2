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
            var either = Either.Left("asdf").Right<IEnumerable<int>>();

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectLeft(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<string, Nothing, StringBuilder>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                , new Nothing()));
        }

        [TestMethod]
        public void SelectLeft()
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

        [TestMethod]
        public void SelectLeftLeftMapException()
        {
            var either = Either.Left("sfd").Right<IEnumerable<int>>();
            var invalidOperationException = new InvalidOperationException();

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => either.SelectLeft((Func<string, Nothing, string>)((left, _) => throw invalidOperationException), new Nothing()));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);

            either = Either.Left<string>().Right(new[] { 42 }.AsEnumerable());

            either.SelectLeft((Func<string, Nothing, string>)((left, _) => throw invalidOperationException), new Nothing());
        }

        [TestMethod]
        public void SelectRightNullEither()
        {
            Either<string, int> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                .SelectRight((right, context) => right, new Nothing()));
        }

        [TestMethod]
        public void SelectRightNullRightSelector()
        {
            var either = Either.Left("asdf").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectRight(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<int, Nothing, int>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                , new Nothing()));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectRight(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<int, Nothing, int>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                , new Nothing()));
        }

        [TestMethod]
        public void SelectRight()
        {
            var either = Either.Left("asdf").Right<IEnumerable<int>>();
            var tuple = new TupleBuilder<string, object>();

            IEither<string, object> result = either.SelectRight((right, context) => context.Item2 = right.First(), tuple);

            Assert.IsNull(tuple.Item1);
            Assert.IsNull(tuple.Item2);

            either = Either.Left<string>().Right(new[] { 42 }.AsEnumerable());
            tuple = new TupleBuilder<string, object>();

            result = either.SelectRight((right, context) => context.Item2 = right.First(), tuple);

            Assert.IsNull(tuple.Item1);
            Assert.IsNotNull(tuple.Item2);
        }

        [TestMethod]
        public void SelectRightRightMapException()
        {
            var either = Either.Left("asdf").Right<int>();
            var invalidOperationException = new InvalidOperationException();

            either.SelectRight((Func<int, Nothing, int>)((right, _) => throw invalidOperationException), new Nothing());

            either = Either.Left<string>().Right(42);

            var rightMapException = Assert.ThrowsException<RightMapException>(() => either.SelectRight((Func<int, Nothing, int>)((right, _) => throw invalidOperationException), new Nothing()));

            Assert.AreEqual(invalidOperationException, rightMapException.InnerException);
        }

        [TestMethod]
        public void SelectNoContextNullEither()
        {
            Either<string, int> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                .Select(left => left, right => right));
        }

        [TestMethod]
        public void SelectNoContextNullLeftSelector()
        {
            var either = Either.Left("adsf").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.Select(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<string, string>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                , right => right));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.Select(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<string, string>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                , right => right));
        }

        [TestMethod]
        public void SelectNoContextNullRightSelector()
        {
            var either = Either.Left("adsf").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.Select(
                left => left,
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<int, int>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.Select(
                left => left,
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<int, int>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));
        }

        [TestMethod]
        public void SelectNoContext()
        {
            var either = Either.Left("safd").Right<int>();
            var tuple = new TupleBuilder<StringBuilder, int[]>();

            IEither<StringBuilder, int[]> result = either.Select(left => tuple.Item1 = new StringBuilder(left), right => tuple.Item2 = new[] { right });

            Assert.IsNotNull(tuple.Item1);
            Assert.IsNull(tuple.Item2);

            either = Either.Left<string>().Right(42);
            tuple = new TupleBuilder<StringBuilder, int[]>();

            result = either.Select(left => tuple.Item1 = new StringBuilder(left), right => tuple.Item2 = new[] { right });

            Assert.IsNull(tuple.Item1);
            Assert.IsNotNull(tuple.Item2);
        }

        [TestMethod]
        public void SelectNoContextLeftMapException()
        {
            var either = Either.Left("ASdf").Right<int>();
            var invalidOperationException = new InvalidOperationException();

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => either.Select((Func<string, string>)(left => throw invalidOperationException), right => right));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);

            either = Either.Left<string>().Right(42);

            either.Select((Func<string, string>)(left => throw invalidOperationException), right => right);
        }

        [TestMethod]
        public void SelectNoContextRightMapException()
        {
            var either = Either.Left("ASdf").Right<int>();
            var invalidOperationException = new InvalidOperationException();

            either.Select(left => left, (Func<int, int>)(right => throw invalidOperationException));

            either = Either.Left<string>().Right(42);

            var rightMapException = Assert.ThrowsException<RightMapException>(() => either.Select(left => left, (Func<int, int>)(right => throw invalidOperationException)));

            Assert.AreEqual(invalidOperationException, rightMapException.InnerException);
        }

        [TestMethod]
        public void SelectLeftNoContextNullEither()
        {
            Either<string, int> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                .SelectLeft(left => left));
        }

        [TestMethod]
        public void SelectLeftNoContextNullSelector()
        {
            var either = Either.Left("dsaf").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectLeft(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<string, string>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectLeft(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<string, string>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));
        }

        [TestMethod]
        public void SelectLeftNoContext()
        {
            var either = Either.Left("safd").Right<int>();
            var tuple = new TupleBuilder<StringBuilder, int[]>();

            either.SelectLeft(left => tuple.Item1 = new StringBuilder(left));

            Assert.IsNotNull(tuple.Item1);
            Assert.IsNull(tuple.Item2);

            either = Either.Left<string>().Right(42);
            tuple = new TupleBuilder<StringBuilder, int[]>();

            either.SelectLeft(left => tuple.Item1 = new StringBuilder(left));

            Assert.IsNull(tuple.Item1);
            Assert.IsNull(tuple.Item2);
        }

        [TestMethod]
        public void SelectLeftNoContextLeftMapException()
        {
            var either = Either.Left("sadf").Right<int>();
            var invalidOperationException = new InvalidOperationException();

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => either.SelectLeft((Func<string, string>)(left => throw invalidOperationException)));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);

            either = Either.Left<string>().Right(42);

            either.SelectLeft((Func<string, string>)(left => throw invalidOperationException));
        }

        [TestMethod]
        public void SelectRightNoContextNullEither()
        {
            Either<string, int> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                either
#pragma warning restore CS8604 // Possible null reference argument.
                .SelectRight(right => right));
        }

        [TestMethod]
        public void SelectRightNoContextNullSelector()
        {
            var either = Either.Left("sadf").Right<int>();

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectRight(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<int, int>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));

            either = Either.Left<string>().Right(42);

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectRight(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<int, int>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));
        }

        [TestMethod]
        public void SelectRightNoContext()
        {
            var either = Either.Left("safd").Right<int>();
            var tuple = new TupleBuilder<StringBuilder, int[]>();

            IEither<string, int[]> result = either.SelectRight(right => tuple.Item2 = new[] { right });

            Assert.IsNull(tuple.Item1);
            Assert.IsNull(tuple.Item2);

            either = Either.Left<string>().Right(42);
            tuple = new TupleBuilder<StringBuilder, int[]>();

            result = either.SelectRight(right => tuple.Item2 = new[] { right });

            Assert.IsNull(tuple.Item1);
            Assert.IsNotNull(tuple.Item2);
        }

        [TestMethod]
        public void SelectRightNoContextRightMapException()
        {
            var invalidOperationException = new InvalidOperationException();
            var either = Either.Left("sadf").Right<int>();

            either.SelectRight((Func<int, int>)(right => throw invalidOperationException));

            either = Either.Left<string>().Right(42);

            var rightMapException = Assert.ThrowsException<RightMapException>(() => either.SelectRight((Func<int, int>)(right => throw invalidOperationException)));

            Assert.AreEqual(invalidOperationException, rightMapException.InnerException);
        }

        [TestMethod]
        public void SelectManyPinnedRightNullEither()
        {
            Either<(string, Either<int, Exception>), Exception> either =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.s
                ;

            Assert.ThrowsException<ArgumentNullException>(() => 
#pragma warning disable CS8604 // Possible null reference argument.
               either
#pragma warning restore CS8604 // Possible null reference argument.
               .SelectMany(left => left.Item2, (left, @int) => (left.Item1, @int)));
        }

        [TestMethod]
        public void SelectManyPinnedRightNullSelector()
        {
            var either = Either.Left(("safd", Either.Left(42).Right<Exception>())).Right<Exception>();

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectMany(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<(string, Either<int, Exception>), Either<int, Exception>>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                , (left, @int) => (left.Item1, @int)));

            either = Either.Left(("asfd", Either.Left<int>().Right(new Exception()))).Right<Exception>();

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectMany(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<(string, Either<int, Exception>), Either<int, Exception>>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                , (left, @int) => (left.Item1, @int)));

            either = Either.Left<(string, Either<int, Exception>)>().Right(new Exception());

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectMany(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<(string, Either<int, Exception>), Either<int, Exception>>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                , (left, @int) => (left.Item1, @int)));
        }

        [TestMethod]
        public void SelectManyPinnedRightNullResultSelector()
        {
            var either = Either.Left(("safd", Either.Left(42).Right<Exception>())).Right<Exception>();

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectMany(
                left => left.Item2,
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<(string, Either<int, Exception>), int, (string, int)>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));

            either = Either.Left(("asfd", Either.Left<int>().Right(new Exception()))).Right<Exception>();

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectMany(
                left => left.Item2,
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<(string, Either<int, Exception>), int, (string, int)>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));

            either = Either.Left<(string, Either<int, Exception>)>().Right(new Exception());

            Assert.ThrowsException<ArgumentNullException>(() => either.SelectMany(
                left => left.Item2,
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<(string, Either<int, Exception>), int, (string, int)>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));
        }

        [TestMethod]
        public void SelectManyPinnedRight()
        {
            var either = Either.Left(("safd", Either.Left(42).Right<Exception>())).Right<Exception>();

            IEither<(string, int), Exception> result = either.SelectMany(left => left.Item2, (left, @int) => (left.Item1, @int));

            Assert.IsTrue(result.TryGetLeft(out var leftValue));
            Assert.AreEqual("safd", leftValue.Item1);
            Assert.AreEqual(42, leftValue.Item2);

            var invalidOperationException = new InvalidOperationException();
            either = Either.Left(("asdf", Either.Left<int>().Right((Exception)invalidOperationException))).Right<Exception>();

            result = either.SelectMany(left => left.Item2, (left, @int) => (left.Item1, @int));

            Assert.IsTrue(result.TryGetRight(out var rightTupleValue));
            Assert.AreEqual(invalidOperationException, rightTupleValue);

            var invalidCastException = new InvalidCastException();
            either = Either.Left<(string, Either<int, Exception>)>().Right((Exception)invalidCastException);

            result = either.SelectMany(left => left.Item2, (left, @int) => (left.Item1, @int));

            Assert.IsTrue(result.TryGetRight(out var rightValue));
            Assert.AreEqual(invalidCastException, rightValue);
        }

        [TestMethod]
        public void SelectManyPinnedRightSelectorLeftMapException()
        {
            var invalidOperationException = new InvalidOperationException();
            var either = Either.Left(("safd", Either.Left(42).Right<Exception>())).Right<Exception>();

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => either.SelectMany((Func<(string, Either<int, Exception>), Either<int, Exception>>)(left => throw invalidOperationException), (left, @int) => (left.Item1, @int)));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);

            either = Either.Left(("sadf", Either.Left<int>().Right(new Exception()))).Right<Exception>();

            leftMapException = Assert.ThrowsException<LeftMapException>(() => either.SelectMany((Func<(string, Either<int, Exception>), Either<int, Exception>>)(left => throw invalidOperationException), (left, @int) => (left.Item1, @int)));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);

            either = Either.Left<(string, Either<int, Exception>)>().Right(new Exception());

            either.SelectMany((Func<(string, Either<int, Exception>), Either<int, Exception>>)(left => throw invalidOperationException), (left, @int) => (left.Item1, @int));
        }

        [TestMethod]
        public void SelectManyPinnedRightResultSelectorLeftMapException()
        {
            var invalidOperationException = new InvalidOperationException();
            var either = Either.Left(("safd", Either.Left(42).Right<Exception>())).Right<Exception>();

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => either.SelectMany(left => left.Item2, (Func<(string, Either<int, Exception>), int, (string, int)>)((left, @int) => throw invalidOperationException)));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);

            either = Either.Left(("sadf", Either.Left<int>().Right(new Exception()))).Right<Exception>();

            either.SelectMany(left => left.Item2, (Func<(string, Either<int, Exception>), int, (string, int)>)((left, @int) => throw invalidOperationException));

            either = Either.Left<(string, Either<int, Exception>)>().Right(new Exception());

            either.SelectMany(left => left.Item2, (Func<(string, Either<int, Exception>), int, (string, int)>)((left, @int) => throw invalidOperationException));
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

        private static Either<string, Exception> Adapt(object val)
        {
            return "ASdf";
        }

        public static string Second(Either<object, System.Exception> either)
        {
            Either<object, Exception> either1 = default!;

            either1.SelectManyLeft(left => Adapt(left));

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
