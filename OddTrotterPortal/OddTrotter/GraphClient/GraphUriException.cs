namespace OddTrotter.GraphClient
{
    using System;

    public sealed class GraphUriException : Exception
    {
        public GraphUriException(string providedUri)
            : base()
        {
            ProvidedUri = providedUri;
        }

        public string ProvidedUri { get; }
    }
}
