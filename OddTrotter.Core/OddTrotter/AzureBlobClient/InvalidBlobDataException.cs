namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class InvalidBlobDataException : Exception
    {
        public InvalidBlobDataException(string blobName, Exception exception)
            : base(blobName, exception)
        {
        }
    }
}
