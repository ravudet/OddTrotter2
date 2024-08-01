namespace OddTrotter.ActorProvisioning
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using OddTrotter.GraphClient;

    /// <summary>
    /// Provisions actor entities in a tenant
    /// </summary>
    public interface IActorProvisioner
    {
        /// <summary>
        /// Creates a new application that has the specified app roles assigned to it
        /// </summary>
        /// <param name="applicationDisplayName">The display name that the newly created application should have</param>
        /// <param name="appRoles">The IDs of the app roles that should be assigned to the application</param>
        /// <returns>The application that has been created, having enough context to clean up after itself when disposed</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applicationDisplayName"/> or <paramref name="appRoles"/> is <see langword="null"/></exception>
        /// <exception cref="HttpRequestException">Thrown if the any of the requests failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by the provisioner is invalid or does not have 'Application.ReadWrite.All' and the 'AppRoleAssignment.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorProvisioningException">Thrown if an error occurred while creating the application, its associated resources, or assigning it permissions</exception>
        Task<IProvisionedActor> ProvisionApplication(string applicationDisplayName, IEnumerable<AppRole> appRoles);
    }
}
