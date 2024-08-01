namespace OddTrotter.TokenIssuance
{
    using OddTrotter.ActorProvisioning;
    using System.Threading.Tasks;
    using System;

    /// <summary>
    /// Extension methods for <see cref="ITokenIssuer"/>
    /// </summary>
    public static class TokenIssuerExtensions
    {
        /// <summary>
        /// Issues a token for <paramref name="actor"/> using <paramref name="tokenIssuer"/>, retrying whenever replication delays are encountered
        /// </summary>
        /// <param name="tokenIssuer">The <see cref="ITokenIssuer"/> to use to issue the token</param>
        /// <param name="actor">The <see cref="Actor"/> to issue a token for</param>
        /// <returns>The token issued for <paramref name="actor"/> by <paramref name="tokenIssuer"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tokenIssuer"/> or <paramref name="actor"/> is <see langword="null"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="TokenIssuanceException">Thrown if the token issuer encountered an error authenticating <paramref name="actor"/> or issuing a token</exception>
        public static async Task<IssuedToken> IssueToken(this ITokenIssuer tokenIssuer, IProvisionedActor actor)
        {
            if (tokenIssuer == null)
            {
                throw new ArgumentNullException(nameof(tokenIssuer));
            }

            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            while (true)
            {
                try
                {
                    return await tokenIssuer.IssueToken(actor.Actor).ConfigureAwait(false);
                }
                catch (TokenIssuanceException e)
                {
                    if (!e.ToString().Contains("7000215", StringComparison.OrdinalIgnoreCase))
                    {
                        throw;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
