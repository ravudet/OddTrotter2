namespace OddTrotter.TokenIssuance
{
    using System;

    /// <summary>
    /// A security principle that can perform operations in a tenant
    /// </summary>
    public sealed class Actor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        /// <param name="appId">The appId of the application that will be authenticated</param>
        /// <param name="secret">The value (not ID) of the secret that is being used to to authenticate the application</param>
        /// <param name="tenantDomain">The domain name of the tenant that the application will be acting in</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="appId"/> or <paramref name="secret"/> or <paramref name="tenantDomain"/> is <see langword="null"/></exception>
        public Actor(string appId, string secret, string tenantDomain)
        {
            if (appId == null)
            {
                throw new ArgumentNullException(nameof(appId));
            }

            if (secret == null)
            {
                throw new ArgumentNullException(nameof(secret));
            }

            if (tenantDomain == null)
            {
                throw new ArgumentNullException(nameof(tenantDomain));
            }

            this.ApplicationId = appId;
            this.Secret = secret;
            this.TenantDomain = tenantDomain;
        }

        /// <summary>
        /// The appId of the application that will be authenticated
        /// </summary>
        public string ApplicationId { get; }

        /// <summary>
        /// The value (not ID) of the secret that is being used to to authenticate the application
        /// </summary>
        public string Secret { get; }

        /// <summary>
        /// The domain name of the tenant that the application will be acting in
        /// </summary>
        public string TenantDomain { get; }
    }
}
