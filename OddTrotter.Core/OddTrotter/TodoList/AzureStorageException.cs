namespace OddTrotter.TodoList
{
    using System;
    using System.Net.Http;

    public sealed class AzureStorageException : Exception
    {
        public AzureStorageException(string message, HttpRequestException exception)
            : base(message, exception)
        {
        }
    }
}
