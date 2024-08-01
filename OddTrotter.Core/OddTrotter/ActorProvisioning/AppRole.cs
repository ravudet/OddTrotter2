namespace OddTrotter.ActorProvisioning
{
    using System;

    /// <summary>
    /// A representation of a permission that can be assigned to an application
    /// </summary>
    public sealed class AppRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppRole"/> class.
        /// </summary>
        /// <param name="identifier">The identifier of the permission</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="identifier"/> is <see langword="null"/></exception>
        public AppRole(string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            this.Identifier = identifier;
        }

        /// <summary>
        /// The identifier of the permission
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Singleton instances of Microsoft Graph app-only permissions, as found <see href="https://learn.microsoft.com/en-us/graph/permissions-reference">here</see>.
        /// </summary>
        public static class Application
        {
            /// <summary>
            /// Gets the singleton 'User.Read.All' permission
            /// </summary>
            public static AppRole UserReadAll { get; } = new AppRole("df021288-bdef-4463-88db-98f22de89214");
        }

        /// <summary>
        /// Singleton instances of Microsoft Graph delegated permissions, as found <see href="https://learn.microsoft.com/en-us/graph/permissions-reference">here</see>.
        /// </summary>
        public static class Delegated
        {
            /// <summary>
            /// Gets the singleton 'User.Read.All' permission
            /// </summary>
            public static AppRole UserReadAll { get; } = new AppRole("a154be20-db9c-4678-8ab7-66f6cc099a59");
        }
    }
}
