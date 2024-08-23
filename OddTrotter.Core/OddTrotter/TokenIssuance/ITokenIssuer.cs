namespace OddTrotter.TokenIssuance
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Issues authenticated tokens for <see cref="Actor"/> that can be used to authorize against REST services
    /// </summary>
    public interface ITokenIssuer
    {
        /// <summary>
        /// Authenticates <paramref name="actor"/> and issues a token that can be used to authorize against REST services
        /// </summary>
        /// <param name="actor">The <see cref="Actor"/> to issue a token for</param>
        /// <returns>The authenticated token for <paramref name="actor"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actor"/> is <see langword="null"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="TokenIssuanceException">Thrown if the token issuer encountered an error authenticating <paramref name="actor"/> or issuing a token</exception>
        Task<IssuedToken> IssueToken(Actor actor);
    }
}
