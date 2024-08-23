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
            /// Gets the singleton 'Application.ReadWrite.All' permission
            /// </summary>
            public static AppRole ApplicationReadWriteAll { get; } = new AppRole("1bfefb4e-e0b5-418b-a88f-73c46d2cc8e9");

            /// <summary>
            /// Gets the singleton 'User.Read.All' permission
            /// </summary>
            public static AppRole UserReadAll { get; } = new AppRole("df021288-bdef-4463-88db-98f22de89214");

            /// <summary>
            /// Gets the singleton 'User.ReadWrite.All' permission
            /// </summary>
            public static AppRole UserReadWriteAll { get; } = new AppRole("741f803b-c850-494e-b5df-cde7c675a1ca");
        }

        /// <summary>
        /// Singleton instances of Microsoft Graph delegated permissions, as found <see href="https://learn.microsoft.com/en-us/graph/permissions-reference">here</see>.
        /// </summary>
        public static class Delegated
        {
            /// <summary>
            /// Gets the singleton 'Application.ReadWrite.All' permission
            /// </summary>
            public static AppRole ApplicationReadWriteAll { get; } = new AppRole("bdfbf15f-ee85-4955-8675-146e8e5296b5");

            /// <summary>
            /// Gets the singleton 'User.Read.All' permission
            /// </summary>
            public static AppRole UserReadAll { get; } = new AppRole("a154be20-db9c-4678-8ab7-66f6cc099a59");

            /// <summary>
            /// Gets the singleton 'User.ReadWrite.All' permission
            /// </summary>
            public static AppRole UserReadWriteAll { get; } = new AppRole("204e0828-b5ca-4ad8-b9f3-f32a958e7cc4");
        }
    }
}
