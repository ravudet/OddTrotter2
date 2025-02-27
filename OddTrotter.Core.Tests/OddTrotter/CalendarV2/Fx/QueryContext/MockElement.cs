/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using System;

    using Fx.Either;

    internal sealed class MockElement : IElement<string, Exception>
    {
        private readonly IQueryResultNode<string, Exception> next;

        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="value"></param>
        public MockElement(string value)
            : this(
                value,
                Either
                    .Left<IElement<string, Exception>>()
                    .Right(
                        Either
                            .Left<IError<Exception>>()
                            .Right(
                                MockEmpty.Instance))
                    .ToQueryResultNode())
        {
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="value"></param>
        /// <param name="next"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is <see langword="null"/></exception>
        public MockElement(string value, IQueryResultNode<string, Exception> next)
        {
            ArgumentNullException.ThrowIfNull(next);

            this.Value = value;
            this.next = next;
        }

        /// <inheritdoc/>
        public string Value { get; }

        /// <inheritdoc/>
        public IQueryResultNode<string, Exception> Next()
        {
            return this.next;
        }
    }
}
