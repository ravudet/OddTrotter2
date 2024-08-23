namespace OddTrotter.ActorProvisioning
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using OddTrotter.GraphClient;
    using OddTrotter.TokenIssuance;

    /// <summary>
    /// A <see cref="Actor"/> for Microsoft Graph that will be deleted when this instance is <see cref="IAsyncDisposable.DisposeAsync"/>d
    /// </summary>
    public sealed class ProvisionedActor : IProvisionedActor
    {
        /// <summary>
        /// The client to use to make the requests to clean up the application
        /// </summary>
        private readonly IGraphClient graphClient;

        /// <summary>
        /// The object ID of the application entity in graph that represents that <see cref="Actor"/>
        /// </summary>
        private readonly string objectId;

        /// <summary>
        /// Whether or not this object is disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvisionedActor"/> class.
        /// </summary>
        /// <param name="actor">The <see cref="Actor"/> that was created, that will be removed when this instance of <see cref="IProvisionedActor"/> is <see cref="IAsyncDisposable.DisposeAsync"/>d</param>
        /// <param name="graphClient">The client to use to make the requests to clean up the application</param>
        /// <param name="objectId">The object ID of the application entity in graph that represents that <see cref="Actor"/></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actor"/> or <paramref name="graphClient"/> or <paramref name="objectId"/> is <see langword="null"/></exception>
        public ProvisionedActor(Actor actor, IGraphClient graphClient, string objectId)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            this.Actor = actor;
            this.graphClient = graphClient;
            this.objectId = objectId;

            this.disposed = false;
        }

        /// <inheritdoc/>
        public Actor Actor { get; }

        /// <inheritdoc/>
        /// <exception cref="HttpRequestException">Thrown if the requests failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used to make the requests is invalid or does not have 'Application.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorCleanupException">Thrown if an error occurred while soft-deleting the application</exception>
        public async ValueTask DisposeAsync()
        {
            if (this.disposed)
            {
                return;
            }

            await SoftDeleteApplication().ConfigureAwait(false);
            await HardDeleteApplication().ConfigureAwait(false);

            this.disposed = true;
        }

        /// <summary>
        /// Soft-deletes the application with ID <see cref="objectId"/>
        /// </summary>
        /// <returns>A promise of deleting the application</returns>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by <see cref="graphClient"/> is invalid or does not have 'Application.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorCleanupException">Thrown if an error occurred while deleting the application</exception>
        private async Task SoftDeleteApplication()
        {
            using (var graphResponse = await this.graphClient.DeleteAsync(new Uri($"/applications/{this.objectId}", UriKind.Relative).ToRelativeUri()).ConfigureAwait(false))
            {
                var graphResponseContent = await graphResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    graphResponse.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException e)
                {
                    throw new ActorCleanupException($"An error occurred while soft-deleting the application. The graph response body was '{graphResponseContent}'.", e);
                }
            }
        }

        /// <summary>
        /// Hard-deletes the application with ID <see cref="objectId"/>
        /// </summary>
        /// <returns>A promise of deleting the application</returns>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by <see cref="graphClient"/> is invalid or does not have 'Application.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorCleanupException">Thrown if an error occurred while deleting the application</exception>
        private async Task HardDeleteApplication()
        {
            using (var graphResponse = await this.graphClient.DeleteAsync(new Uri($"/directory/deletedItems/{this.objectId}", UriKind.Relative).ToRelativeUri()).ConfigureAwait(false))
            {
                var graphResponseContent = await graphResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    graphResponse.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException e)
                {
                    throw new ActorCleanupException($"An error occurred while hard-deleting the application. The graph response body was '{graphResponseContent}'.", e);
                }
            }
        }
    }
}
