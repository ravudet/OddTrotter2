/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using System;
    using System.Security.Cryptography;
    using Fx.Either;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OddTrotter.Calendar;

    [TestClass]
    public sealed class QueryResultExtensionsUnitTests
    {
        [TestMethod]
        public void WhereNullSource()
        {
            IQueryResult<string, Exception> queryResult =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                queryResult
#pragma warning restore CS8604 // Possible null reference argument.
                .Where(_ => true));
        }

        private sealed class MockQueryResult : IQueryResult<string, Exception>
        {
            /// <summary>
            /// placeholder
            /// </summary>
            /// <param name="nodes"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="nodes"/> is <see langword="null"/></exception>
            public MockQueryResult(IQueryResultNode<string, Exception> nodes)
            {
                ArgumentNullException.ThrowIfNull(nodes);

                this.Nodes = nodes;
            }

            /// <inheritdoc/>
            public IQueryResultNode<string, Exception> Nodes { get; }
        }

        [TestMethod]
        public void WhereNullPredicate()
        {
            var value = "asdf";
            var queryResult = 
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(value))
                        .Right<IEither<IError<Exception>, IEmpty>>()
                        .ToQueryResultNode());

            Assert.ThrowsException<ArgumentNullException>(() => queryResult.Where(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }
        [TestMethod]
        public void WhereNoElements()
        {
            var queryResult = 
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(
                                    MockEmpty.Instance))
                        .ToQueryResultNode());

            var whered = queryResult.Where(_ => true);

            Assert.IsFalse(whered.Nodes.TryGetLeft(out var element));
            Assert.IsTrue(whered.Nodes.TryGetRight(out var terminal));
            Assert.IsFalse(terminal.TryGetLeft(out var error));
            Assert.IsTrue(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void WhereNoElementsError()
        {
            var invalidOperationException = new InvalidOperationException();
            var queryResult = 
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left(
                                    new MockError(invalidOperationException))
                                .Right<MockEmpty>())
                        .ToQueryResultNode());

            var whered = queryResult.Where(_ => true);

            Assert.IsFalse(whered.Nodes.TryGetLeft(out var element));
            Assert.IsTrue(whered.Nodes.TryGetRight(out var terminal));
            Assert.IsTrue(terminal.TryGetLeft(out var error));
            Assert.AreEqual(invalidOperationException, error.Value);
            Assert.IsFalse(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void WhereNoError()
        {
            var value = "asdf";
            var queryResult =
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(value))
                        .Right<IEither<MockError, MockEmpty>>()
                        .ToQueryResultNode());

            var whered = queryResult.Where(_ => true);

            Assert.IsTrue(whered.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(value, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var secondElement));
            Assert.IsTrue(next.TryGetRight(out var secondTerminal));
            Assert.IsFalse(secondTerminal.TryGetLeft(out var secondError));
            Assert.IsTrue(secondTerminal.TryGetRight(out var secondEmpty));
            Assert.IsFalse(whered.Nodes.TryGetRight(out var terminal));

            whered = queryResult.Where(_ => false);

            Assert.IsFalse(whered.Nodes.TryGetLeft(out element));
            Assert.IsTrue(whered.Nodes.TryGetRight(out var terminal2));
            Assert.IsFalse(terminal2.TryGetLeft(out var error));
            Assert.IsTrue(terminal2.TryGetRight(out var empty));
        }

        [TestMethod]
        public void WhereElementFollowedByError()
        {
            var value = "asdf";
            var invalidOperationException = new InvalidOperationException();
            var queryResult =
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(
                                value,
                                Either
                                    .Left<MockElement>()
                                    .Right(
                                        Either
                                            .Left(
                                                new MockError(invalidOperationException))
                                            .Right<MockEmpty>())
                                    .ToQueryResultNode()))
                        .Right<IEither<MockError, MockEmpty>>()
                        .ToQueryResultNode());

            var whered = queryResult.Where(_ => true);

            Assert.IsTrue(whered.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(value, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var secondElement));
            Assert.IsTrue(next.TryGetRight(out var secondTerminal));
            Assert.IsTrue(secondTerminal.TryGetLeft(out var secondError));
            Assert.AreEqual(invalidOperationException, secondError.Value);
            Assert.IsFalse(secondTerminal.TryGetRight(out var secondEmpty));
            Assert.IsFalse(whered.Nodes.TryGetRight(out var terminal));

            whered = queryResult.Where(_ => false);

            Assert.IsFalse(whered.Nodes.TryGetLeft(out element));
            Assert.IsTrue(whered.Nodes.TryGetRight(out var terminal2));
            Assert.IsTrue(terminal2.TryGetLeft(out var error));
            Assert.AreEqual(invalidOperationException, error.Value);
            Assert.IsFalse(terminal2.TryGetRight(out var empty));
        }

        [TestMethod]
        public void SelectNullSource()
        {
            IQueryResult<string, Exception> queryResult =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                queryResult
#pragma warning restore CS8604 // Possible null reference argument.
                .Select(val => val.Length));
        }

        [TestMethod]
        public void SelectNullSelector()
        {
            var value = "asdf";
            var queryResult = 
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(value))
                        .Right<IEither<IError<Exception>, IEmpty>>()
                        .ToQueryResultNode());

            Assert.ThrowsException<ArgumentNullException>(() => queryResult.Select(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                (Func<string, int>)null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ));
        }

        [TestMethod]
        public void SelectNoElements()
        {
            var queryResult = 
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(
                                    MockEmpty.Instance))
                        .ToQueryResultNode());

            var selected = queryResult.Select(val => val.Length);

            Assert.IsFalse(selected.Nodes.TryGetLeft(out var element));
            Assert.IsTrue(selected.Nodes.TryGetRight(out var terminal));
            Assert.IsFalse(terminal.TryGetLeft(out var error));
            Assert.IsTrue(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void SelectNoElementsError()
        {
            var invalidOperationException = new InvalidOperationException();
            var queryResult =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left(
                                    new MockError(invalidOperationException))
                                .Right<MockEmpty>())
                        .ToQueryResultNode());

            var selected = queryResult.Select(val => val.Length);

            Assert.IsFalse(selected.Nodes.TryGetLeft(out var element));
            Assert.IsTrue(selected.Nodes.TryGetRight(out var terminal));
            Assert.IsTrue(terminal.TryGetLeft(out var error));
            Assert.AreEqual(invalidOperationException, error.Value);
            Assert.IsFalse(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void SelectNoError()
        {
            var value = "asdf";
            var queryResult = 
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(value))
                        .Right<IEither<MockError, MockEmpty>>()
                        .ToQueryResultNode());

            var selected = queryResult.Select(val => val.Length);

            Assert.IsTrue(selected.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(4, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var secondElement));
            Assert.IsTrue(next.TryGetRight(out var secondTerminal));
            Assert.IsFalse(secondTerminal.TryGetLeft(out var secondError));
            Assert.IsTrue(secondTerminal.TryGetRight(out var secondEmpty));
            Assert.IsFalse(selected.Nodes.TryGetRight(out var terminal));
        }

        [TestMethod]
        public void SelectElementFollowedByError()
        {
            var value = "asdf";
            var invalidOperationException = new InvalidOperationException();
            var queryResult =
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(
                                value,
                                Either
                                    .Left<MockElement>()
                                    .Right(
                                        Either
                                            .Left(
                                                new MockError(invalidOperationException))
                                            .Right<MockEmpty>())
                                    .ToQueryResultNode()))
                        .Right<IEither<MockError, MockEmpty>>()
                        .ToQueryResultNode());

            var selected = queryResult.Select(val => val.Length);

            Assert.IsTrue(selected.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(4, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var secondElement));
            Assert.IsTrue(next.TryGetRight(out var secondTerminal));
            Assert.IsTrue(secondTerminal.TryGetLeft(out var secondError));
            Assert.AreEqual(invalidOperationException, secondError.Value);
            Assert.IsFalse(secondTerminal.TryGetRight(out var secondEmpty));
            Assert.IsFalse(selected.Nodes.TryGetRight(out var terminal));
        }

        [TestMethod]
        public void FirstOrDefaultNullSource()
        {
            IQueryResult<string, Exception> queryResult =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                queryResult
#pragma warning restore CS8604 // Possible null reference argument.
                .FirstOrDefault(42));
        }

        [TestMethod]
        public void FirstOrDefaultNoElements()
        {
            var defaultValue = 42;
            var queryResult =
                new MockQueryResult(
                    Either
                        .Left<IElement<string, Exception>>()
                        .Right(
                            Either
                                .Left<IError<Exception>>()
                                .Right(MockEmpty.Instance))
                        .ToQueryResultNode());

            var firstOrDefaultOrError = queryResult.FirstOrDefault(defaultValue);

            Assert.IsTrue(firstOrDefaultOrError.TryGetLeft(out var firstOrDefault));
            Assert.IsFalse(firstOrDefault.TryGetLeft(out var first));
            Assert.IsTrue(firstOrDefault.TryGetRight(out var @default));
            Assert.AreEqual(defaultValue, @default);
            Assert.IsFalse(firstOrDefaultOrError.TryGetRight(out var error));
        }

        [TestMethod]
        public void FirstOrDefaultNoElementsError()
        {
            var errorValue = new InvalidOperationException();
            var queryResult =
                new MockQueryResult(
                    Either
                        .Left<IElement<string, Exception>>()
                        .Right(
                            Either
                                .Left(
                                    new MockError(errorValue))
                                .Right<IEmpty>())
                        .ToQueryResultNode());

            var firstOrDefaultOrError = queryResult.FirstOrDefault(42);

            Assert.IsFalse(firstOrDefaultOrError.TryGetLeft(out var firstOrDefault));
            Assert.IsTrue(firstOrDefaultOrError.TryGetRight(out var error));
            Assert.AreEqual(errorValue, error);
        }

        [TestMethod]
        public void FirstOrDefault()
        {
            var element = "ASdf";
            var queryResult =
                new MockQueryResult(
                    Either
                        .Left(new MockElement(element))
                        .Right<IEither<IError<Exception>, IEmpty>>()
                        .ToQueryResultNode());

            var firstOrDefaultOrError = queryResult.FirstOrDefault(42);

            Assert.IsTrue(firstOrDefaultOrError.TryGetLeft(out var firstOrDefault));
            Assert.IsTrue(firstOrDefault.TryGetLeft(out var first));
            Assert.AreEqual(element, first);
            Assert.IsFalse(firstOrDefault.TryGetRight(out var @default));
            Assert.IsFalse(firstOrDefaultOrError.TryGetRight(out var error));
        }

        [TestMethod]
        public void ConcatNullFirst()
        {
            IQueryResult<string, InvalidOperationException> first =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            var second =
                new MockQueryResult(
                    Either
                        .Left(new MockElement("saf"))
                        .Right<IEither<IError<InvalidCastException>, IEmpty>>()
                        .ToQueryResultNode());

            Assert.ThrowsException<ArgumentNullException>(
                () =>
#pragma warning disable CS8604 // Possible null reference argument.
                    first
#pragma warning restore CS8604 // Possible null reference argument.
                        .Concat(
                            second, 
                            firstError => new AggregateException(firstError), 
                            secondError => new AggregateException(secondError), 
                            (firstError, secondError) => new AggregateException(firstError, secondError)));
        }

        [TestMethod]
        public void ConcatNullSecond()
        {
            var first =
                new MockQueryResult(
                    Either
                        .Left(new MockElement("saf"))
                        .Right<IEither<IError<InvalidCastException>, IEmpty>>()
                        .ToQueryResultNode());

            IQueryResult<string, InvalidOperationException> second =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;

            Assert.ThrowsException<ArgumentNullException>(
                () =>
                    first
                        .Concat(
#pragma warning disable CS8604 // Possible null reference argument.
                            second
#pragma warning restore CS8604 // Possible null reference argument.
                            ,
                            firstError => new AggregateException(firstError),
                            secondError => new AggregateException(secondError),
                            (firstError, secondError) => new AggregateException(firstError, secondError)));
        }

        [TestMethod]
        public void ConcatNullFirstErrorSelector()
        {
            var first =
                new MockQueryResult(
                    Either
                        .Left(new MockElement("saf"))
                        .Right<IEither<IError<InvalidCastException>, IEmpty>>()
                        .ToQueryResultNode());
            var second =
                new MockQueryResult(
                    Either
                        .Left(new MockElement("qwer"))
                        .Right<IEither<IError<InvalidCastException>, IEmpty>>()
                        .ToQueryResultNode());

            Assert.ThrowsException<ArgumentNullException>(
                () => 
                    first
                        .Concat(
                            second,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                            null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                            ,
                            secondError => new AggregateException(secondError), 
                            (firstError, secondError) => new AggregateException(firstError, secondError)));
        }

        [TestMethod]
        public void ConcatNullSecondErrorSelector()
        {
            var first =
                new MockQueryResult(
                    Either
                        .Left(new MockElement("saf"))
                        .Right<IEither<IError<InvalidCastException>, IEmpty>>()
                        .ToQueryResultNode());
            var second =
                new MockQueryResult(
                    Either
                        .Left(new MockElement("qwer"))
                        .Right<IEither<IError<InvalidCastException>, IEmpty>>()
                        .ToQueryResultNode());

            Assert.ThrowsException<ArgumentNullException>(
                () =>
                    first
                        .Concat(
                            second,
                            firstError => new AggregateException(firstError),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                            null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                            ,
                            (firstError, secondError) => new AggregateException(firstError, secondError)));
        }

        [TestMethod]
        public void ConcatNullErrorAggregator()
        {
            var first =
                new MockQueryResult(
                    Either
                        .Left(new MockElement("saf"))
                        .Right<IEither<IError<InvalidCastException>, IEmpty>>()
                        .ToQueryResultNode());
            var second =
                new MockQueryResult(
                    Either
                        .Left(new MockElement("qwer"))
                        .Right<IEither<IError<InvalidCastException>, IEmpty>>()
                        .ToQueryResultNode());

            Assert.ThrowsException<ArgumentNullException>(
                () =>
                    first
                        .Concat(
                            second,
                            firstError => new AggregateException(firstError),
                            secondError => new AggregateException(secondError),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                            null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                            ));
        }

        [TestMethod]
        public void ConcatFirstNoElementNoErrorSecondNoElementNoError()
        {
            var first =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(MockEmpty.Instance))
                        .ToQueryResultNode());
            var second =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(MockEmpty.Instance))
                        .ToQueryResultNode());

            var concated = first.Concat(second, firstError => new AggregateException(firstError), secondError => new AggregateException(secondError), (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsFalse(concated.Nodes.TryGetLeft(out var element));
            Assert.IsTrue(concated.Nodes.TryGetRight(out var terminal));
            Assert.IsFalse(terminal.TryGetLeft(out var error));
            Assert.IsTrue(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void ConcatFirstNoElementNoErrorSecondNoElementError()
        {
            var first =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(MockEmpty.Instance))
                        .ToQueryResultNode());
            var invalidCastException = new InvalidCastException();
            var second =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left(new MockError(invalidCastException))
                                .Right<IEmpty>())
                        .ToQueryResultNode());

            var concated = first.Concat(second, firstError => new AggregateException(firstError), secondError => new AggregateException(secondError), (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsFalse(concated.Nodes.TryGetLeft(out var element));
            Assert.IsTrue(concated.Nodes.TryGetRight(out var terminal));
            Assert.IsTrue(terminal.TryGetLeft(out var error));
            Assert.AreEqual(1, error.Value.InnerExceptions.Count);
            Assert.AreEqual(invalidCastException, error.Value.InnerExceptions[0]);
            Assert.IsFalse(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void ConcatFirstNoElementNotErrorSecondElementNoError()
        {
            var first =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(MockEmpty.Instance))
                        .ToQueryResultNode());
            var secondValue = "qwer";
            var second =
                new MockQueryResult(
                    Either
                        .Left(new MockElement(secondValue))
                        .Right<IEither<IError<Exception>, IEmpty>>()
                        .ToQueryResultNode());

            var concated = first.Concat(
                second, 
                firstError => new AggregateException(firstError), 
                secondError => new AggregateException(secondError), 
                (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsTrue(concated.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(secondValue, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var nextTerminal));
            Assert.IsTrue(next.TryGetRight(out var empty));
            Assert.IsFalse(concated.Nodes.TryGetRight(out var terminal));
        }

        [TestMethod]
        public void ConcatFirstNoElementNoErrorSecondElementError()
        {
            var first =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(MockEmpty.Instance))
                        .ToQueryResultNode());
            var secondValue = "qwer";
            var invalidCastException = new InvalidCastException();
            var second =
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(
                                secondValue,
                                Either
                                    .Left<IElement<string, Exception>>()
                                    .Right(
                                        Either
                                            .Left(new MockError(invalidCastException))
                                            .Right<IEmpty>())
                                    .ToQueryResultNode()))
                        .Right<IEither<IError<Exception>, IEmpty>>()
                        .ToQueryResultNode());

            var concated = first.Concat(
                second,
                firstError => new AggregateException(firstError),
                secondError => new AggregateException(secondError),
                (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsTrue(concated.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(secondValue, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var nextElement));
            Assert.IsTrue(next.TryGetRight(out var nextTerminal));
            Assert.IsTrue(nextTerminal.TryGetLeft(out var nextError));
            Assert.AreEqual(1, nextError.Value.InnerExceptions.Count);
            Assert.AreEqual(invalidCastException, nextError.Value.InnerExceptions[0]);
            Assert.IsFalse(nextTerminal.TryGetRight(out var nextEmpty));
            Assert.IsFalse(concated.Nodes.TryGetRight(out var terminal));
        }

        [TestMethod]
        public void ConcatFirstNoElementErrorSecondNoElementNoError()
        {
            var invalidOperationException = new InvalidOperationException();
            var first =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left(new MockError(invalidOperationException))
                                .Right<IEmpty>())
                        .ToQueryResultNode());
            var second =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(MockEmpty.Instance))
                        .ToQueryResultNode());

            var concated = first.Concat(
                second,
                firstError => new AggregateException(firstError),
                secondError => new AggregateException(secondError),
                (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsFalse(concated.Nodes.TryGetLeft(out var element));
            Assert.IsTrue(concated.Nodes.TryGetRight(out var terminal));
            Assert.IsTrue(terminal.TryGetLeft(out var error));
            Assert.AreEqual(1, error.Value.InnerExceptions.Count);
            Assert.AreEqual(invalidOperationException, error.Value.InnerExceptions[0]);
            Assert.IsFalse(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void ConcatFirstNoElementErrorSecondNoElementError()
        {
            var invalidOperationException = new InvalidOperationException();
            var first =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left(new MockError(invalidOperationException))
                                .Right<IEmpty>())
                        .ToQueryResultNode());
            var invalidCastException = new InvalidCastException();
            var second =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left(new MockError(invalidCastException))
                                .Right<IEmpty>())
                        .ToQueryResultNode());

            var concated = first.Concat(
                second,
                firstError => new AggregateException(firstError),
                secondError => new AggregateException(secondError),
                (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsFalse(concated.Nodes.TryGetLeft(out var element));
            Assert.IsTrue(concated.Nodes.TryGetRight(out var terminal));
            Assert.IsTrue(terminal.TryGetLeft(out var error));
            Assert.AreEqual(2, error.Value.InnerExceptions.Count);
            Assert.AreEqual(invalidOperationException, error.Value.InnerExceptions[0]);
            Assert.AreEqual(invalidCastException, error.Value.InnerExceptions[1]);
            Assert.IsFalse(terminal.TryGetRight(out var empty));
        }

        [TestMethod]
        public void ConcatFirstNoElementErrorSecondElementNoError()
        {
            var invalidOperationException = new InvalidOperationException();
            var first =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left(new MockError(invalidOperationException))
                                .Right<IEmpty>())
                        .ToQueryResultNode());
            var secondValue = "qwer";
            var second =
                new MockQueryResult(
                    Either
                        .Left(new MockElement(secondValue))
                        .Right<IEither<IError<Exception>, IEmpty>>()
                        .ToQueryResultNode());

            var concated = first.Concat(
                second,
                firstError => new AggregateException(firstError),
                secondError => new AggregateException(secondError),
                (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsTrue(concated.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(secondValue, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var nextElement));
            Assert.IsTrue(next.TryGetRight(out var nextTerminal));
            Assert.IsTrue(nextTerminal.TryGetLeft(out var nextError));
            Assert.AreEqual(1, nextError.Value.InnerExceptions.Count);
            Assert.AreEqual(invalidOperationException, nextError.Value.InnerExceptions[0]);
            Assert.IsFalse(nextTerminal.TryGetRight(out var nextEmpty));
            Assert.IsFalse(concated.Nodes.TryGetRight(out var terminal));
        }

        [TestMethod]
        public void ConcatFirstNoElementErrorSecondElementError()
        {
            var invalidOperationException = new InvalidOperationException();
            var first =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left(new MockError(invalidOperationException))
                                .Right<IEmpty>())
                        .ToQueryResultNode());
            var secondValue = "qwer";
            var invalidCastException = new InvalidCastException();
            var second =
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(
                                secondValue,
                                Either
                                    .Left<IElement<string, Exception>>()
                                    .Right(
                                        Either
                                            .Left(new MockError(invalidCastException))
                                            .Right<IEmpty>())
                                    .ToQueryResultNode()))
                        .Right<IEither<IError<Exception>, IEmpty>>()
                        .ToQueryResultNode());

            var concated = first.Concat(
                second,
                firstError => new AggregateException(firstError),
                secondError => new AggregateException(secondError),
                (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsTrue(concated.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(secondValue, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var nextElement));
            Assert.IsTrue(next.TryGetRight(out var nextTerminal));
            Assert.IsTrue(nextTerminal.TryGetLeft(out var nextError));
            Assert.AreEqual(2, nextError.Value.InnerExceptions.Count);
            Assert.AreEqual(invalidOperationException, nextError.Value.InnerExceptions[0]);
            Assert.AreEqual(invalidCastException, nextError.Value.InnerExceptions[1]);
            Assert.IsFalse(nextTerminal.TryGetRight(out var nextEmpty));
            Assert.IsFalse(concated.Nodes.TryGetRight(out var terminal));
        }

        [TestMethod]
        public void ConcatFirstElementNoErrorSecondNoElementNoError()
        {
            var firstValue = "Asdf";
            var first =
                new MockQueryResult(
                    Either
                        .Left(
                            new MockElement(
                                    firstValue))
                        .Right<IEither<IError<Exception>, IEmpty>>()
                        .ToQueryResultNode());
            var second =
                new MockQueryResult(
                    Either
                        .Left<MockElement>()
                        .Right(
                            Either
                                .Left<MockError>()
                                .Right(MockEmpty.Instance))
                        .ToQueryResultNode());

            var concated = first.Concat(
                second,
                firstError => new AggregateException(firstError),
                secondError => new AggregateException(secondError),
                (firstError, secondError) => new AggregateException(firstError, secondError));

            Assert.IsTrue(concated.Nodes.TryGetLeft(out var element));
            Assert.AreEqual(firstValue, element.Value);
            var next = element.Next();
            Assert.IsFalse(next.TryGetLeft(out var nextElement));
            Assert.IsTrue(next.TryGetRight(out var nextTerminal));
            Assert.IsFalse(nextTerminal.TryGetLeft(out var nextError));
            Assert.IsTrue(nextTerminal.TryGetRight(out var nextEmpty));
            Assert.IsFalse(concated.Nodes.TryGetRight(out var terminal));
        }
/*
first element   first error     second element      second error
1               0               0                   1
1               0               1                   0
1               0               1                   1
1               1               0                   0
1               1               0                   1
1               1               1                   0
1               1               1                   1
*/
    }
}
