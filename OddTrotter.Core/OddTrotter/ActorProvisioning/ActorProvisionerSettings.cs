namespace OddTrotter.ActorProvisioning
{
    using System;

    /// <summary>
    /// The settings used to instantiate a <see cref="ActorProvisioner"/>
    /// </summary>
    public sealed class ActorProvisionerSettings
    {
        /// <summary>
        /// Prevents the initialization of the <see cref="ActorProvisionerSettings"/> class
        /// </summary>
        /// <param name="resourceDisplayNamePrefix">The string that should be prepended to the display name of all of the actors provisioned</param>
        private ActorProvisionerSettings(string resourceDisplayNamePrefix)
        {
            this.ResourceDisplayNamePrefix = resourceDisplayNamePrefix;
        }

        /// <summary>
        /// The default instance of <see cref="ActorProvisionerSettings"/>
        /// </summary>
        public static ActorProvisionerSettings Default { get; } = new ActorProvisionerSettings(string.Empty);

        /// <summary>
        /// The string that should be prepended to the display name of all of the actors provisioned
        /// </summary>
        public string ResourceDisplayNamePrefix { get; }

        /// <summary>
        /// A builder for <see cref="ActorProvisionerSettings"/>
        /// </summary>
        public sealed class Builder
        {
            /// <summary>
            /// The string that should be prepended to the display name of all of the actors provisioned
            /// </summary>
            public string ResourceDisplayNamePrefix { get; set; } = ActorProvisionerSettings.Default.ResourceDisplayNamePrefix;

            /// <summary>
            /// Creates a new instance of <see cref="ActorProvisionerSettings"/> from the properties configured on this <see cref="Builder"/>
            /// </summary>
            /// <returns>The new instance of <see cref="ActorProvisionerSettings"/></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="ResourceDisplayNamePrefix"/> is <see langword="null"/></exception>
            public ActorProvisionerSettings Build()
            {
                if (this.ResourceDisplayNamePrefix == null)
                {
                    throw new ArgumentNullException(nameof(this.ResourceDisplayNamePrefix));
                }

                return new ActorProvisionerSettings(this.ResourceDisplayNamePrefix);
            }
        }
    }
}
