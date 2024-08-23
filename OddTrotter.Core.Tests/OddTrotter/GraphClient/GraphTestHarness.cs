namespace OddTrotter.GraphClient
{
    using System;
    using OddTrotter.ActorProvisioning;
    using OddTrotter.TokenIssuance;

    /// <summary>
    /// The resources that can be used to issue real tokens that can be used to authorize clients against Microsoft Graph
    /// </summary>
    public sealed class GraphTestHarness
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphTestHarness"/> class
        /// </summary>
        /// <param name="tokenIssuer">The <see cref="TokenIssuer"/> that the test can use in order to get tokens issued for different actors</param>
        /// <param name="actorProvisioner">The <see cref="ActorProvisioner"/> that the test can use to provision actors within a tenant</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tokenIssuer"/> or <paramref name="actorProvisioner"/> is <see langword="null"/></exception>
        public GraphTestHarness(ITokenIssuer tokenIssuer, IActorProvisioner actorProvisioner)
        {
            if (tokenIssuer == null)
            {
                throw new ArgumentNullException(nameof(tokenIssuer));
            }

            if (actorProvisioner == null)
            {
                throw new ArgumentNullException(nameof(actorProvisioner));
            }

            this.TokenIssuer = tokenIssuer;
            this.ActorProvisioner = actorProvisioner;
        }

        /// <summary>
        /// The <see cref="TokenIssuer"/> that the test can use in order to get tokens issued for different actors
        /// </summary>
        public ITokenIssuer TokenIssuer { get; }

        /// <summary>
        /// The <see cref="ActorProvisioner"/> that the test can use to provision actors within a tenant
        /// </summary>
        public IActorProvisioner ActorProvisioner { get; }
    }
}
