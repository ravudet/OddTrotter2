namespace OddTrotter.GraphClient
{
    using System;
    using System.Threading;

    public sealed class GraphClientSettings
    {
        private GraphClientSettings(Uri graphRootUri, TimeSpan httpClientTimeout)
        {
            this.GraphRootUri = graphRootUri;
            this.HttpClientTimeout = httpClientTimeout;
        }

        public static GraphClientSettings Default { get; } = new GraphClientSettings(new Uri("https://graph.microsoft.com/v1.0/"), Timeout.InfiniteTimeSpan);

        public Uri GraphRootUri { get; }

        public TimeSpan HttpClientTimeout { get; }

        public sealed class Builder
        {
            public Uri GraphRootUri { get; set; } = GraphClientSettings.Default.GraphRootUri;

            public TimeSpan HttpClientTimeout { get; set; } = GraphClientSettings.Default.HttpClientTimeout;

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="GraphRootUri"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentException">Thrown if <see cref="GraphRootUri"/> is not an absolute URI</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="HttpClientTimeout"/> is not a positive value</exception>
            public GraphClientSettings Build()
            {
                if (this.GraphRootUri == null)
                {
                    throw new ArgumentNullException(nameof(this.GraphRootUri));
                }

                if (!this.GraphRootUri.IsAbsoluteUri)
                {
                    throw new ArgumentException($"'{nameof(this.GraphRootUri)}' must be an absolute URI");
                }

                if (this.HttpClientTimeout.Ticks < 0 && this.HttpClientTimeout != Timeout.InfiniteTimeSpan)
                {
                    throw new ArgumentOutOfRangeException($"'{nameof(this.HttpClientTimeout)} must be positive or must be infinite");
                }

                return new GraphClientSettings(this.GraphRootUri, this.HttpClientTimeout);
            }
        }
    }
}
