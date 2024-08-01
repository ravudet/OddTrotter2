namespace OddTrotter.TokenIssuance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Issues ESTS tokens for use with Microsoft Graph
    /// </summary>
    /// <remarks>You can find more details <see href="https://learn.microsoft.com/en-us/graph/auth-v2-service?tabs=http">here</see> about the token issuance process.</remarks>
    public sealed class TokenIssuer : ITokenIssuer
    {
        /// <summary>
        /// Prevents initialization of a default <see cref="TokenIssuer"/>
        /// </summary>
        private TokenIssuer()
        {
        }

        /// <summary>
        /// The default instance of the <see cref="GraphClientIntegrationTestHarnessFactory"/>
        /// </summary>
        public static TokenIssuer Default { get; } = new TokenIssuer();

        /// <inheritdoc/>
        public async Task<IssuedToken> IssueToken(Actor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            using (var httpClient = new HttpClient())
            {
                var scope = "https://graph.microsoft.com/.default";
                var tokenEndpoint = new Uri($"https://login.microsoftonline.com/{actor.TenantDomain}/oauth2/v2.0/token", UriKind.Absolute);

                var issueTokenRequest = new IssueTokenRequest(
                    actor.ApplicationId,
                    actor.Secret, 
                    scope, 
                    "client_credentials");
                var serializedTokenRequestContent = ToRequestBody(issueTokenRequest);
                using (var httpRequestContent = new StringContent(serializedTokenRequestContent, new MediaTypeHeaderValue("application/x-www-form-urlencoded")))
                {
                    using (var httpResponseMessage = await httpClient.PostAsync(tokenEndpoint, httpRequestContent).ConfigureAwait(false))
                    {
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        try
                        {
                            httpResponseMessage.EnsureSuccessStatusCode();
                        }
                        catch (HttpRequestException e)
                        {
                            throw new TokenIssuanceException($"An error occurred while issuing a token. The request was '{serializedTokenRequestContent}'. The ESTS response was '{responseContent}'", e);
                        }

                        IssuedToken.Builer? issuedToken;
                        try
                        {
                            issuedToken = JsonSerializer.Deserialize<IssuedToken.Builer>(responseContent);
                        }
                        catch (JsonException e)
                        {
                            throw new TokenIssuanceException($"ESTS issued a token that was not valid JSON. The request was '{serializedTokenRequestContent}'. The ESTS response was '{responseContent}'", e);
                        }

                        if (issuedToken == null)
                        {
                            throw new TokenIssuanceException($"ESTS issued a token that contained a null JSON response. The request was '{serializedTokenRequestContent}'.");
                        }

                        // all of the beloew properties are likely not required for most callers, but since [this](https://learn.microsoft.com/en-us/graph/auth-v2-service?tabs=http#token-response) documentation guarantees them, and the return type also has them as required, let's throw if they don't exist
                        var missingProperties = new List<string>();
                        if (issuedToken.TokenType == null)
                        {
                            missingProperties.Add("token_type");
                        }

                        if (issuedToken.ExpiresIn == null)
                        {
                            missingProperties.Add("expires_in");
                        }
                        
                        if (issuedToken.ExtExpiresIn == null)
                        {
                            missingProperties.Add("ext_expires_in");
                        }

                        if (issuedToken.AccessToken == null)
                        {
                            missingProperties.Add("access_token");
                        }

                        if (missingProperties.Any())
                        {
                            throw new TokenIssuanceException($"ESTS issued a token that did not contain a complete JSON response. The following JSON properties were missing: {string.Join(", ", missingProperties)}. The request was '{serializedTokenRequestContent}'.");
                        }

                        return issuedToken.Build();
                    }
                }
            }
        }

        /// <summary>
        /// Translates <paramref name="issueTokenRequest"/> into a 'application/x-www-form-urlencoded' request body string
        /// </summary>
        /// <param name="issueTokenRequest">The token issuance data to translate</param>
        /// <returns>The request body that should be used for a 'application/x-www-form-urlencoded' request</returns>
        private static string ToRequestBody(IssueTokenRequest issueTokenRequest)
        {
            return $"client_id={HttpUtility.UrlEncode(issueTokenRequest.ClientId)}&client_secret={HttpUtility.UrlEncode(issueTokenRequest.ClientSecret)}&scope={HttpUtility.UrlEncode(issueTokenRequest.Scope)}&grant_type={HttpUtility.UrlEncode(issueTokenRequest.GrantType)}";
        }

        /// <summary>
        /// The parameters to use for a request to issue a token from ESTS
        /// </summary>
        /// <remarks>
        /// More details can be found <see href="https://learn.microsoft.com/en-us/graph/auth-v2-service?tabs=http#token-request">here</see>.
        /// </remarks>
        private sealed class IssueTokenRequest
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="IssueTokenRequest"/> class.
            /// </summary>
            /// <param name="clientId">The appId of the client application that will be authenticated</param>
            /// <param name="clientSecret">The secret on the client application that will be used to authenticate it</param>
            /// <param name="scope">An identifier URI of the resource application that is being accessed, following by the '.default' suffix</param>
            /// <param name="grantType">Must be the value 'client_credentials'</param>
            public IssueTokenRequest(string clientId, string clientSecret, string scope, string grantType)
            {
                ClientId = clientId;
                ClientSecret = clientSecret;
                Scope = scope;
                GrantType = grantType;
            }

            /// <summary>
            /// The appId of the client application that will be authenticated
            /// </summary>
            public string ClientId { get; }

            /// <summary>
            /// The secret on the client application that will be used to authenticate it
            /// </summary>
            public string ClientSecret { get; }

            /// <summary>
            /// An identifier URI of the resource application that is being accessed, following by the '.default' suffix
            /// </summary>
            public string Scope { get; }

            /// <summary>
            /// Must be the value 'client_credentials'
            /// </summary>
            public string GrantType { get; }
        }
    }
}
