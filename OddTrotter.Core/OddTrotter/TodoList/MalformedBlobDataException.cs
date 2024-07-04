namespace OddTrotter.TodoList
{
    using System;

    public sealed class MalformedBlobDataException : Exception
    {
        public MalformedBlobDataException(string blobContent, Exception exception)
            : base(blobContent, exception)
        {
        }
    }
}
