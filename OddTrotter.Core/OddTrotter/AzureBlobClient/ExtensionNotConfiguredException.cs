namespace OddTrotter.AzureBlobClient
{
    using System;

    public sealed class ExtensionNotConfiguredException : Exception
    {
        public ExtensionNotConfiguredException(string extensionPath)
            : base()
        {
            ExtensionPath = extensionPath;
        }

        public string ExtensionPath { get; }
    }
}
