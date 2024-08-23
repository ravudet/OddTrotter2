namespace OddTrotter.GraphClient
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using OddTrotter.TokenIssuance;

    /// <summary>
    /// A <see cref="IGraphClient"/> that automatically renews the ESTS tokens that it uses and delegates the use of those tokens to another <see cref="IGraphClient"/>
    /// </summary>
    public sealed class AutomaticTokenRenewalGraphClient : IGraphClient
    {
        /// <summary>
        /// The <see cref="ITokenIssuer"/> to use to issue and renew tokens
        /// </summary>
        private readonly ITokenIssuer tokenIssuer;

        /// <summary>
        /// The <see cref="Actor"/> that should be used to authenticate with <see cref="tokenIssuer"/> and authorize with the Microsoft Graph service
        /// </summary>
        private readonly Actor actor;

        /// <summary>
        /// A factory that generates a <see cref="IGraphClient"/> from a token issued for <see cref="actor"/>
        /// </summary>
        /// <remarks>The <see cref="IssuedToken"/> is used instead of just a string for the input parameter because the factory may make use of the remainder of the token. For example, the factory may use the token in different ways depending on the token type.</remarks>
        private readonly Func<IssuedToken, IGraphClient> delegatedClientFactory;

        /// <summary>
        /// A <see cref="Stopwatch"/> that was started just before the most recent token issuance
        /// </summary>
        /// <remarks>
        /// Another option would be to use a <see cref="DateTime"/> to track when the token was last issued. However, <see cref="Stopwatch"/> will maintain the elapsed time even if the system time has changed (see the <see href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=net-8.0&redirectedfrom=MSDN#remarks">remarks</see> of the <see cref="Stopwatch"/> documentation).
        /// </remarks>
        private Stopwatch stopwatch;

        /// <summary>
        /// The lifetime of the token issued
        /// </summary>
        private TimeSpan tokenLifetime;

        /// <summary>
        /// The <see cref="IGraphClient"/> that actual Microsoft Graph calls are being delegated to
        /// </summary>
        private IGraphClient delegateGraphClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticTokenRenewalGraphClient"/> class.
        /// </summary>
        /// <param name="tokenIssuer">The <see cref="ITokenIssuer"/> to use to issue and renew tokens</param>
        /// <param name="actor">The <see cref="Actor"/> that should be used to authenticate with <see cref="tokenIssuer"/> and authorize with the Microsoft Graph service</param>
        /// <param name="delegatedClientFactory">A factory that generates a <see cref="IGraphClient"/> from a token issued for <see cref="actor"/></param>
        /// <param name="stopwatch">A <see cref="Stopwatch"/> that was started just before the most recent token issuance</param>
        /// <param name="tokenLifetime">The lifetime of the token issued</param>
        /// <param name="delegateGraphClient">The <see cref="IGraphClient"/> that actual Microsoft Graph calls are being delegated to</param>
        private AutomaticTokenRenewalGraphClient(ITokenIssuer tokenIssuer, Actor actor, Func<IssuedToken, IGraphClient> delegatedClientFactory, Stopwatch stopwatch, TimeSpan tokenLifetime, IGraphClient delegateGraphClient)
        {
            this.tokenIssuer = tokenIssuer;
            this.actor = actor;
            this.delegatedClientFactory = delegatedClientFactory;
            this.stopwatch = stopwatch;
            this.tokenLifetime = tokenLifetime;
            this.delegateGraphClient = delegateGraphClient;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AutomaticTokenRenewalGraphClient"/> class
        /// </summary>
        /// <param name="tokenIssuer">The <see cref="ITokenIssuer"/> that the client will use to issue and renew tokens</param>
        /// <param name="actor">The <see cref="Actor"/> that should be used to authenticate with <paramref name="tokenIssuer"/> and authorize with the Microsoft Graph service</param>
        /// <param name="delegatedClientFactory">A factory that generates a <see cref="IGraphClient"/> from a token issued for <paramref name="actor"/></param>
        /// <returns>A new instance of the <see cref="AutomaticTokenRenewalGraphClient"/> class</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tokenIssuer"/> or <paramref name="actor"/> or <paramref name="delegatedClientFactory"/> is <see langword="null"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if token issuance failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="TokenIssuanceException">Thrown if <paramref name="tokenIssuer"/> encountered an error authenticating <paramref name="actor"/> or issuing a token</exception>
        public static async Task<AutomaticTokenRenewalGraphClient> Create(ITokenIssuer tokenIssuer, Actor actor, Func<IssuedToken, IGraphClient> delegatedClientFactory)
        {
            if (tokenIssuer == null)
            {
                throw new ArgumentNullException(nameof(tokenIssuer));
            }

            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (delegatedClientFactory == null)
            {
                throw new ArgumentNullException(nameof(delegatedClientFactory));
            }

            var renewedToken = await IssueToken(tokenIssuer, actor, delegatedClientFactory).ConfigureAwait(false);
            var client = new AutomaticTokenRenewalGraphClient(tokenIssuer, actor, delegatedClientFactory, renewedToken.Item1, renewedToken.Item2, renewedToken.Item3);

            return client;
        }

        /// <summary>
        /// Issues a token for <paramref name="actor"/> and generates a client from that token
        /// </summary>
        /// <param name="tokenIssuer">The <see cref="ITokenIssuer"/> to use to issue a token for <paramref name="actor"/></param>
        /// <param name="actor">The <see cref="Actor"/> that will be used to authenticate with <paramref name="tokenIssuer"/> and authorize with the <see cref="IGraphClient"/> generated by <paramref name="delegatedClientFactory"/></param>
        /// <param name="delegatedClientFactory">The factory that will be used to genearte a <see cref="IGraphClient"/> from a token issued for <paramref name="actor"/></param>
        /// <returns>
        /// The state data involved in the token issuance:
        /// Item1: a <see cref="Stopwatch"/> that was started just before the token issuance
        /// Item2: the lifetime of the token issued
        /// Item3: the <see cref="IGraphClient"/> that was generated using the newly issued token
        /// </returns>
        /// <exception cref="HttpRequestException">
        /// Thrown if the token issuance failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="TokenIssuanceException">Thrown if <paramref name="tokenIssuer"/> encountered an error authenticating <paramref name="actor"/> or issuing a token</exception>
        private static async Task<(Stopwatch, TimeSpan, IGraphClient)> IssueToken(ITokenIssuer tokenIssuer, Actor actor, Func<IssuedToken, IGraphClient> delegatedClientFactory)
        {
            var stopwatch = Stopwatch.StartNew();
            var token = await tokenIssuer.IssueToken(actor).ConfigureAwait(false);
            var graphClient = delegatedClientFactory(token);
            var tokenLifetime = token.ExpiresIn;

            return (stopwatch, tokenLifetime, graphClient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task RenewToken()
        {
            var renewedToken = await IssueToken(this.tokenIssuer, this.actor, this.delegatedClientFactory).ConfigureAwait(false);
            this.stopwatch = renewedToken.Item1;
            this.tokenLifetime = renewedToken.Item2;
            this.delegateGraphClient = renewedToken.Item3;

            //// TODO this is not thread-safe; you can use a stored field token
        }

        private async Task EnsureToken()
        {
            if (this.stopwatch.Elapsed > this.tokenLifetime)
            {
                await this.RenewToken().ConfigureAwait(false);
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(RelativeUri relativeUri)
        {
            //// TODO document and check ecah of the interface method implementations
            //// TODO have a tiemespan buffer
            await EnsureToken().ConfigureAwait(false);

            return await this.delegateGraphClient.DeleteAsync(relativeUri).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri)
        {
            await EnsureToken().ConfigureAwait(false);

            return await this.delegateGraphClient.GetAsync(relativeUri).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri)
        {
            await EnsureToken().ConfigureAwait(false);

            return await this.delegateGraphClient.GetAsync(absoluteUri).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> PatchAsync(RelativeUri relativeUri, HttpContent httpContent)
        {
            await EnsureToken().ConfigureAwait(false);

            return await this.delegateGraphClient.PatchAsync(relativeUri, httpContent).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
        {
            await EnsureToken().ConfigureAwait(false);

            return await this.delegateGraphClient.PostAsync(relativeUri, httpContent).ConfigureAwait(false);
        }
    }
}
