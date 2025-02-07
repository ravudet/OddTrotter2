/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using System;

    public sealed class LeftMapException : Exception
    {
        public LeftMapException()
        {
        }

        public LeftMapException(string message)
            : base(message)
        {
        }

        public LeftMapException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
