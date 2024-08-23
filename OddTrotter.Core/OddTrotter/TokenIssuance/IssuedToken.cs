namespace OddTrotter.TokenIssuance
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// An authenticated token that can be used to authorize with a REST service
    /// </summary>
    public sealed class IssuedToken
    {
        private IssuedToken(string tokenType, TimeSpan expiresIn, TimeSpan extExpiresIn, string accessToken)
        {
            this.TokenType = tokenType;
            this.ExpiresIn = expiresIn;
            this.ExtExpiresIn = extExpiresIn;
            this.AccessToken = accessToken;
        }

        /// <summary>
        /// The type of the issued token
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// The lifetime of the issued token
        /// </summary>
        public TimeSpan ExpiresIn { get; set; }

        /// <summary>
        /// The extended lifetime of the issued token in case the token service experiences downtime
        /// </summary>
        public TimeSpan ExtExpiresIn { get; set; }

        /// <summary>
        /// The token that can be used to authorize
        /// </summary>
        public string AccessToken { get; set; }

        public sealed class Builer
        {
            /// <summary>
            /// The type of the issued token
            /// </summary>
            [JsonPropertyName("token_type")]
            public string? TokenType { get; set; }

            /// <summary>
            /// The lifetime of the issued token, in seconds
            /// </summary>
            [JsonPropertyName("expires_in")]
            public long? ExpiresIn { get; set; }

            /// <summary>
            /// The extended lifetime of the issued token, in seconds, in case the token service experiences downtime
            /// </summary>
            [JsonPropertyName("ext_expires_in")]
            public long? ExtExpiresIn { get; set; }

            /// <summary>
            /// The token that can be used to authorize
            /// </summary>
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="TokenType"/> or <see cref="ExpiresIn"/> or <see cref="ExtExpiresIn"/> or <see cref="AccessToken"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="ExpiresIn"/> or <see cref="ExtExpiresIn"/> is not a positive value</exception>
            public IssuedToken Build()
            {
                if (this.TokenType == null)
                {
                    throw new ArgumentNullException(nameof(this.TokenType));
                }

                if (this.ExpiresIn == null)
                {
                    throw new ArgumentNullException(nameof(this.ExpiresIn));
                }

                if (this.ExpiresIn <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.ExpiresIn), $"The token expiration must be a postive number. The provided value was '{this.ExpiresIn}'.");
                }

                if (this.ExtExpiresIn == null)
                {
                    throw new ArgumentNullException(nameof(this.ExtExpiresIn));
                }

                if (this.ExtExpiresIn <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.ExtExpiresIn), $"The token expiration must be a postive number. The provided value was '{this.ExtExpiresIn}'.");
                }

                if (this.AccessToken == null)
                {
                    throw new ArgumentNullException(nameof(this.AccessToken));
                }

                return new IssuedToken(
                    this.TokenType, 
                    TimeSpan.FromSeconds(this.ExpiresIn.Value), 
                    TimeSpan.FromSeconds(this.ExtExpiresIn.Value),
                    this.AccessToken);
            }
        }
    }
}
