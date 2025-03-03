namespace Fx.QueryContext
{
    using System;

    using Fx.Either;

    internal sealed class NullableErrorElement : IElement<string, int?>
    {
        private readonly IQueryResultNode<string, int?> next;

        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="value"></param>
        public NullableErrorElement(string value)
            : this(
                value,
                Either
                    .Left<IElement<string, int?>>()
                    .Right(
                        Either
                            .Left<IError<int?>>()
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
        public NullableErrorElement(string value, IQueryResultNode<string, int?> next)
        {
            ArgumentNullException.ThrowIfNull(next);

            this.Value = value;
            this.next = next;
        }

        /// <inheritdoc/>
        public string Value { get; }

        /// <inheritdoc/>
        public IQueryResultNode<string, int?> Next()
        {
            return this.next;
        }
    }
}
