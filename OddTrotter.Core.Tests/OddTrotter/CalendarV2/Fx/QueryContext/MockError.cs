/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using System;

    internal sealed class MockError : IError<Exception>
    {
        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="value"></param>
        public MockError(Exception value)
        {
            this.Value = value;
        }

        /// <inheritdoc/>
        public Exception Value { get; }
    }
}
