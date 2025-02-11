/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using System;

    public sealed class RightMapException : Exception
    {
        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <remarks>
        /// This is the only overload implemented because the purpose of this exception is to wrap and normalize other
        /// exceptions. As a result, we can know that <see cref="InnerException"/> is not <see langword="null"/>.
        /// </remarks>
        public RightMapException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.InnerException = innerException;
        }

        public new Exception InnerException { get; }
    }
}
