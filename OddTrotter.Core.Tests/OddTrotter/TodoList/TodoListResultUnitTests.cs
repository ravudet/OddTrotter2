namespace OddTrotter.TodoList
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Fx.Caching;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="TodoListResult"/>
    /// </summary>
    [TestClass]
    public sealed class TodoListResultUnitTests
    {
        private sealed class Foo
        {
            public int Bar { get; } = 0;
        }

        private static class Fizz
        {
            public static int Buzz { get; } = 0;
        }

        private static int Frob { get; } = 0;

        private struct FooStruct
        {
            public int Bar { get; }
        }

        public static void DoThing(BinaryExpression expor)
        {
            var invoked = expor.Conversion?.Compile().DynamicInvoke();
        }

        private sealed class TestCache : IMemoryCache
        {
            public TestCache(string value)
            {

            }

            public TestCache(RavudetFactory<string> valueFactory)
            {

            }

            public ICacheEntry CreateEntry(object key)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void Remove(object key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(object key, out object? value)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void DoWork()
        {
            var memoryCacheFactory = new RavudetFactory<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
            var nestedMemoryCacheFactory = new RavudetFactory<IMemoryCache>(() => new NestedMemoryCache(memoryCacheFactory));
            var ravudetCache = new RavudetCache(nestedMemoryCacheFactory);

            ////Expression<Action<int>> factorialFunc = (n) => n + 1;

            /*var add = Expression.Add(
                Expression.Parameter(typeof(int), "n"),
                Expression.Constant(1));

            DoThing(add);

            //// TODO are there legitimate field accesses?
            new Nonclosure<Func<int>>(() => 2);
            var value = 2;
            new Nonclosure<Func<int>>(() => value);
            new Nonclosure<Func<int>>(() => new Foo().Bar);
            var foo = new Foo();
            new Nonclosure<Func<int>>(() => foo.Bar);
            //// TODO these static accessors won't be caught by a field access check because they don't actually result in closures
            //// TODO you don't actually want to catch *all* property accesses anyway, only those that are resulting in the type being factoried
            new Nonclosure<Func<int>>(() => Fizz.Buzz);
            new Nonclosure<Func<int>>(() => Frob);

            //// we *can* determine the different between reference types and value types, so you may not need to worry about value types that are closed
            new Nonclosure<Func<int>>(() => new FooStruct().Bar);

            //// TODo what if there's already a factory method?
            new Nonclosure<Func<int>>(() => Create());
            new Nonclosure<Func<int>>(CreateExpression);

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var objectCache = new ObjectCache(new MemoryCacheFactory(() => memoryCache));*/

            var stringFactory = new RavudetFactory<string>(() => "asdf");
            new MemoryCacheFactory(() => new TestCache(stringFactory));

            var factory = new MemoryCacheFactory(() => new TestCache("asdf"));

            var objectCache2 = new ObjectCache(new MemoryCacheFactory(() => new MemoryCache(new MemoryCacheOptions())));

            /*var memoryCache3 = new MemoryCache(new MemoryCacheOptions());
            var factory3 = new MemoryCacheFactory(() => memoryCache);
            var objectCache3 = new ObjectCache(factory3);*/
        }

        private static int Create()
        {
            return 0;
        }

        private static int CreateUsingExpression()
        {
            return CreateExpression.Compile()();
        }

        private static Expression<Func<int>> CreateExpression = () => 0;

        /// <summary>
        /// Creates a <see cref="TodoListResult"/> with a <see langword="null"/> todo list
        /// </summary>
        [TestMethod]
        public void NullTodoList()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TodoListResult(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
                Enumerable.Empty<CalendarEvent>(),
                Enumerable.Empty<(CalendarEvent, Exception)>(),
                Enumerable.Empty<CalendarEvent>(),
                Enumerable.Empty<(CalendarEvent, Exception)>()));
        }

        /// <summary>
        /// Creates a <see cref="TodoListResult"/> with a <see langword="null"/> sequence of events without starts
        /// </summary>
        [TestMethod]
        public void NullWithoutStarts()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TodoListResult(
                "the list",
                null,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                Enumerable.Empty<(CalendarEvent, Exception)>(),
                Enumerable.Empty<CalendarEvent>(),
                Enumerable.Empty<(CalendarEvent, Exception)>()));
        }

        /// <summary>
        /// Creates a <see cref="TodoListResult"/> with a <see langword="null"/> sequence of events that failed to parse starts
        /// </summary>
        [TestMethod]
        public void NullStartParsing()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TodoListResult(
                "the list",
                null,
                Enumerable.Empty<CalendarEvent>(),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                Enumerable.Empty<CalendarEvent>(),
                Enumerable.Empty<(CalendarEvent, Exception)>()));
        }

        /// <summary>
        /// Creates a <see cref="TodoListResult"/> with a <see langword="null"/> sequence of events without bodies
        /// </summary>
        [TestMethod]
        public void NullWithoutBodies()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TodoListResult(
                "the list",
                null,
                Enumerable.Empty<CalendarEvent>(),
                Enumerable.Empty<(CalendarEvent, Exception)>(),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                Enumerable.Empty<(CalendarEvent, Exception)>()));
        }

        /// <summary>
        /// Creates a <see cref="TodoListResult"/> with a <see langword="null"/> sequence of events that failed to parse bodies
        /// </summary>
        [TestMethod]
        public void NullBodyParsing()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TodoListResult(
                "the list",
                null,
                Enumerable.Empty<CalendarEvent>(),
                Enumerable.Empty<(CalendarEvent, Exception)>(),
                Enumerable.Empty<CalendarEvent>(),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }
    }
}
