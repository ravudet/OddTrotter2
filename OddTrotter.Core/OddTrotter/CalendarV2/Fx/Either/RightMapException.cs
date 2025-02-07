/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.Either
{
    using System;

    public sealed class RightMapException : Exception
    {
        public RightMapException()
        {
        }

        public RightMapException(string message)
            : base(message)
        {
        }

        public RightMapException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
