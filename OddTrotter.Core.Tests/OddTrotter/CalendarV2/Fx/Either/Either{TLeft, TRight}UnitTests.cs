namespace Fx.Either
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public sealed class EitherUnitTests
    {
        [TestMethod]
        public void CreateLeft()
        {
            var value = "some value";
            var left = new Either<string, int>.Left(value);

            Assert.AreEqual(value, left.Value);
        }

        [TestMethod]
        public void CreateRight()
        {
            var value = 42;
            var right = new Either<string, int>.Right(value);

            Assert.AreEqual(value, right.Value);
        }

        [TestMethod]
        public void NullLeft()
        {
            new Either<string?, string?>.Left(null);
        }

        [TestMethod]
        public void NullRight()
        {
            new Either<string?, string?>.Right(null);
        }

        [TestMethod]
        public void VisitNullNode()
        {
            Assert.ThrowsException<ArgumentNullException>(() => MockVisitor.Instance.Visit(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                , default));
        }

        [TestMethod]
        public void LeftAccept()
        {
            Either<string, int> either = new Either<string, int>.Left("asdf");
            
            var result = MockVisitor.Instance.Visit(either, default);

            Assert.AreEqual('a', result);
        }

        [TestMethod]
        public void RightAccept()
        {
            Either<string, int> either = new Either<string, int>.Right(42);

            var result = MockVisitor.Instance.Visit(either, default);

            Assert.AreEqual('4', result);
        }

        private sealed class MockVisitor : Either<string, int>.Visitor<char, Nothing>
        {
            private MockVisitor()
            {
            }

            public static MockVisitor Instance { get; } = new MockVisitor();

            protected override char Accept(Either<string, int>.Left node, Nothing context)
            {
                return node.Value[0];
            }

            protected override char Accept(Either<string, int>.Right node, Nothing context)
            {
                return node.Value.ToString()[0];
            }
        }

        [TestMethod]
        public void ApplyLeft()
        {
            Either<string, int> either = new Either<string, int>.Left("asdf");

            var result = either.Apply((left, context) => left[0], (right, context) => right.ToString()[0], new Nothing());

            Assert.AreEqual('a', result);
        }

        [TestMethod]
        public void ApplyRight()
        {
            Either<string, int> either = new Either<string, int>.Right(42);

            var result = either.Apply((left, context) => left[0], (right, context) => right.ToString()[0], new Nothing());

            Assert.AreEqual('4', result);
        }

        [TestMethod]
        public void ApplyLeftException()
        {
            Either<string, int> either = new Either<string, int>.Left("asdf");

            var invalidOperationException = new InvalidOperationException();
            var invalidCastException = new InvalidCastException();

            var leftMapException = Assert.ThrowsException<LeftMapException>(() => either.Apply<char, Nothing>((left, context) => throw invalidOperationException, (right, context) => throw invalidCastException, default));

            Assert.AreEqual(invalidOperationException, leftMapException.InnerException);
        }

        [TestMethod]
        public void ApplyRightException()
        {
            Either<string, int> either = new Either<string, int>.Right(42);

            var invalidOperationException = new InvalidOperationException();
            var invalidCastException = new InvalidCastException();

            var leftMapException = Assert.ThrowsException<RightMapException>(() => either.Apply<char, Nothing>((left, context) => throw invalidOperationException, (right, context) => throw invalidCastException, default));

            Assert.AreEqual(invalidCastException, leftMapException.InnerException);
        }
    }
}
