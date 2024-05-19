namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class InvalidBlobNameException : Exception
    {
        public InvalidBlobNameException(string blobName, Exception exception)
            : base(blobName, exception)
        {
        }
    }
}
