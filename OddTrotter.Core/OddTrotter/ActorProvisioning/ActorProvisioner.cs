namespace OddTrotter.ActorProvisioning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.V2;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    using OddTrotter.GraphClient;
    using OddTrotter.TokenIssuance;
    using OddTrotter.UserExtension;

    /// <summary>
    /// A <see cref="IActorProvisioner"/> that provisions actors in Microsoft Graph
    /// </summary>
    public sealed class ActorProvisioner : IActorProvisioner
    {
        /// <summary>
        /// The <see cref="IGraphClient"/> to use when provisioning <see cref="Actor"/>s
        /// </summary>
        private readonly IGraphClient graphClient;

        /// <summary>
        /// The domain name of the tenant to provision the actors in
        /// </summary>
        private readonly string tenantDomain;

        /// <summary>
        /// The string that should be prepended to the display name of all of the actors provisioned
        /// </summary>
        private readonly string resourceDisplayNamePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorProvisioner"/> class.
        /// </summary>
        /// <param name="graphClient">The <see cref="IGraphClient"/> to use when provisioning <see cref="Actor"/>s. The token used by this client must have been issued for an application with the 'Application.ReadWrite.All' and 'AppRoleAssignment.ReadWrite.All' app role assignments. Both assignments need to be admin consented.</param>
        /// <param name="tenantDomain">The domain name of the tenant to provision the actors in</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphClient"/> or <paramref name="tenantDomain"/> is <see langword="null"/></exception>
        public ActorProvisioner(IGraphClient graphClient, string tenantDomain)
            : this(graphClient, tenantDomain, ActorProvisionerSettings.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorProvisioner"/> class.
        /// </summary>
        /// <param name="graphClient">The <see cref="IGraphClient"/> to use when provisioning <see cref="Actor"/>s. The token used by this client must have been issued for an application with the 'Application.ReadWrite.All' and 'AppRoleAssignment.ReadWrite.All' app role assignments. Both assignments need to be admin consented.</param>
        /// <param name="tenantDomain">The domain name of the tenant to provision the actors in</param>
        /// <param name="settings">The settings to use to configure the provisioner</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphClient"/> or <paramref name="tenantDomain"/> or <paramref name="settings"/> is <see langword="null"/></exception>
        public ActorProvisioner(IGraphClient graphClient, string tenantDomain, ActorProvisionerSettings settings)
        {
            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            if (tenantDomain == null)
            {
                throw new ArgumentNullException(nameof(tenantDomain));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.graphClient = graphClient;
            this.tenantDomain = tenantDomain;

            this.resourceDisplayNamePrefix = settings.ResourceDisplayNamePrefix;
        }

        /// <inheritdoc/>
        public async Task<IProvisionedActor> ProvisionApplication(string applicationDisplayName, IEnumerable<AppRole> appRoles)
        {
            if (applicationDisplayName == null)
            {
                throw new ArgumentNullException(nameof(applicationDisplayName));
            }

            if (appRoles == null)
            {
                throw new ArgumentNullException(nameof(appRoles));
            }

            var createdApplication = await CreateApplication(this.resourceDisplayNamePrefix + applicationDisplayName).ConfigureAwait(false);
            var servicePrincipalId = await CreateServicePrincipal(createdApplication.ApplicationId).ConfigureAwait(false);
            var microsoftGraphServicePrincipalId = await GetMicrosoftGraphServicePrincipalId().ConfigureAwait(false);
            await AssignPermissions(servicePrincipalId, microsoftGraphServicePrincipalId, appRoles).ConfigureAwait(false);
            var secret = await CreateSecret(createdApplication.ObjectId).ConfigureAwait(false);
            var application = new Actor(createdApplication.ApplicationId, secret.secretText, this.tenantDomain);

            return new ProvisionedActor(application, this.graphClient, createdApplication.ObjectId);
        }

        /// <summary>
        /// Creates a new "secret" (the application analog to a "password") on the applicaiton with object ID <paramref name="applicationObjectId"/>
        /// </summary>
        /// <param name="applicationObjectId">The object ID of the application to add a secret to</param>
        /// <returns>The secret that was added to the application</returns>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by <see cref="graphClient"/> is invalid or does not have 'Application.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorProvisioningException">Thrown if an error occurred while creating the secret or deserializing the response</exception> //// TODO should you separate these two types? don't you want to know that the application was created so that it can be cleaned up?
        private async Task<(string secretText, string secretId)> CreateSecret(string applicationObjectId)
        {
            using (var requestContent = new StringContent(string.Empty, new MediaTypeHeaderValue("application/json")))
            {
                using (var graphResponse = await this.graphClient.PostAsync(new Uri($"/applications/{applicationObjectId}/addPassword", UriKind.Relative).ToRelativeUri(), requestContent).ConfigureAwait(false))
                {
                    var graphResponseContent = await graphResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        graphResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        throw new ActorProvisioningException($"An error occured while assigning the appRole to the servicePrincipal. The graph response body was '{graphResponseContent}'.", e);
                    }

                    AddPasswordResponseBody? createdSecret;
                    try
                    {
                        createdSecret = JsonSerializer.Deserialize<AddPasswordResponseBody>(graphResponseContent);
                    }
                    catch (JsonException e)
                    {
                        throw new ActorProvisioningException($"An error occured while deserializing the password response body. The graph response body was '{graphResponseContent}'.", e);
                    }

                    if (createdSecret == null)
                    {
                        throw new ActorProvisioningException($"Microsoft Graph added a password with a null JSON response.");
                    }

                    var missingProperties = new List<string>();
                    if (createdSecret.SecretText == null)
                    {
                        missingProperties.Add("secretText");
                    }

                    if (createdSecret.KeyId == null)
                    {
                        missingProperties.Add("keyId");
                    }

                    if (missingProperties.Any())
                    {
                        throw new ActorProvisioningException($"Microsoft Graph added a password with an incomplete JSON response. The following JSON properties were missing: {string.Join(", ", missingProperties)}. The graph response body was '{graphResponseContent}'.");
                    }

                    return 
                        (
                            createdSecret.SecretText!, // the null check adds it to the list of missing properties, which causes an exception if there are any elements, so this is safe
                            createdSecret.KeyId! // the null check adds it to the list of missing properties, which causes an exception if there are any elements, so this is safe
                        );
                }
            }
        }

        /// <summary>
        /// The expected structure of the response body for a password object on an application
        /// </summary>
        private sealed class AddPasswordResponseBody
        {
            /// <summary>
            /// The ID that can be used to refer to the password
            /// </summary>
            [JsonPropertyName("keyId")]
            public string? KeyId { get; set; }

            /// <summary>
            /// The value of the password (usually <see langword="null"/> unless the request was to create the password)
            /// </summary>
            [JsonPropertyName("secretText")]
            public string? SecretText { get; set; }
        }

        /// <summary>
        /// Grants the permissions in <paramref name="appRoles"/> for the servicePrincipal with ID <paramref name="servicePrincipalId"/> to access the application with ID <paramref name="resourceApplicationId"/>
        /// </summary>
        /// <param name="servicePrincipalId">The object ID of the servicePrincipal that will be granted permissions, assumed to not be <see langword="null"/></param>
        /// <param name="resourceApplicationId">The appId of the application that the servicePrincipal will access, assumed to not be <see langword="null"/></param>
        /// <param name="appRoles">The <see cref="AppRole"/>s that should be granted to the servicePrincipal, assumed to not be <see langword="null"/></param>
        /// <returns>A promise of the permissions being assigned</returns>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by <see cref="graphClient"/> is invalid or does not have 'AppRoleAssignment.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorProvisioningException">Thrown if an error occurred while assigning the appRole</exception>
        private async Task AssignPermissions(string servicePrincipalId, string resourceApplicationId, IEnumerable<AppRole> appRoles)
        {
            foreach (var appRole in appRoles)
            {
                await AssignPermission(servicePrincipalId, resourceApplicationId, appRole.Identifier).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Grants the permission of the appRole with ID <paramref name="appRoleId"/> to the servicePrincipal with ID <paramref name="servicePrincipalId"/> to access the application with ID <paramref name="resourceApplicationId"/>
        /// </summary>
        /// <param name="servicePrincipalId">The object ID of the servicePrincipal that will be granted the permission, assumed to not be <see langword="null"/></param>
        /// <param name="resourceApplicationId">The appId of the application that the servicePrincipal will access, assumed to not be <see langword="null"/></param>
        /// <param name="appRoleId">The ID of the <see cref="AppRole"/> that should be granted to the servicePrincipal, assumed to not be <see langword="null"/></param>
        /// <returns>A promise of the permission being assigned</returns>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by <see cref="graphClient"/> is invalid or does not have 'AppRoleAssignment.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorProvisioningException">Thrown if an error occurred while assigning the appRole</exception>
        private async Task AssignPermission(string servicePrincipalId, string resourceApplicationId, string appRoleId)
        {
            var appRoleAssignmentCreateRequestBodyFormat =
"""
{{
    "principalId": "{0}",
    "resourceId": "{1}",
    "appRoleId": "{2}"
}}
""";
            var appRoleAssignmentCreationRequestBody = string.Format(
                appRoleAssignmentCreateRequestBodyFormat,
                servicePrincipalId,
                resourceApplicationId,
                appRoleId
                );
            using (var requestContent = new StringContent(appRoleAssignmentCreationRequestBody, new MediaTypeHeaderValue("application/json")))
            {
                using (var graphResponse = await this.graphClient.PostAsync(new Uri($"/servicePrincipals/{servicePrincipalId}/appRoleAssignments", UriKind.Relative).ToRelativeUri(), requestContent).ConfigureAwait(false))
                {
                    var graphResponseContent = await graphResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        graphResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        throw new ActorProvisioningException($"An error occured while assigning the appRole to the servicePrincipal. The request body was '{appRoleAssignmentCreationRequestBody}'. The graph response body was '{graphResponseContent}'.", e);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the object ID of the servicePrincipal in the tenant that is associated with the 'Microsoft Graph' application
        /// </summary>
        /// <returns>The object ID of the 'Microsoft Graph' servicePrincipal</returns>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by <see cref="graphClient"/> is invalid or does not have 'Application.Read.All' permissions</exception>
        /// <exception cref="ActorProvisioningException">Thrown if an error occurred while retrieving the servicePrincipal or deserializing the response</exception> //// TODO should you separate these two types? don't you want to know that the application was created so that it can be cleaned up?
        private async Task<string> GetMicrosoftGraphServicePrincipalId()
        {
            const string microsoftGraphAppId = "00000003-0000-0000-c000-000000000000";

            using (var graphResponse = await this.graphClient.GetAsync(new Uri($"/servicePrincipals(appId='{microsoftGraphAppId}')", UriKind.Relative).ToRelativeUri()).ConfigureAwait(false))
            {
                var graphResponseContent = await graphResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    graphResponse.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException e)
                {
                    throw new ActorProvisioningException($"An error occured while retrieving the servicePrincipal. The graph response body was '{graphResponseContent}'.", e);
                }

                ServicePrincipalResponseBody? graphServicePrincipal;
                try
                {
                    graphServicePrincipal = JsonSerializer.Deserialize<ServicePrincipalResponseBody>(graphResponseContent);
                }
                catch (JsonException e)
                {
                    throw new ActorProvisioningException($"An error occured while deserializing the servicePrincipal response body. The graph response body was '{graphResponseContent}'.", e);
                }

                if (graphServicePrincipal == null)
                {
                    throw new ActorProvisioningException($"Microsoft Graph retrieved a servicePrincipal with a null JSON response.");
                }

                if (graphServicePrincipal.ObjectId == null)
                {
                    throw new ActorProvisioningException($"Microsoft Graph retrieved a servicePrincipal with an incomplete JSON response. The following JSON properties were missing: objectId. The graph response body was '{graphResponseContent}'.");
                }

                return graphServicePrincipal.ObjectId;
            }
        }

        /// <summary>
        /// Creates a Microsoft Graph servicePrincipal in the tenant
        /// </summary>
        /// <param name="applicationId">The appId of the application object that the created servicePrincipal should be associated with, assumed to not be <see langword="null"/></param>
        /// <returns>The `id` property of the servicePrincipal object</returns>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by <see cref="graphClient"/> is invalid or does not have 'Application.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorProvisioningException">Thrown if an error occurred while creating the servicePrincipal or deserializing the response</exception> //// TODO should you separate these two types? don't you want to know that the application was created so that it can be cleaned up?
        private async Task<string> CreateServicePrincipal(string applicationId)
        {
            var servicePrincipalCreationRequestBodyFormat =
"""
{{
    "appId": "{0}"
}}
""";
            var servicePrincipalCreationRequestBody = string.Format(
                servicePrincipalCreationRequestBodyFormat,
                applicationId);
            using (var requestContent = new StringContent(servicePrincipalCreationRequestBody, new MediaTypeHeaderValue("application/json")))
            {
                using (var graphResponse = await this.graphClient.PostAsync(new Uri("/servicePrincipals", UriKind.Relative).ToRelativeUri(), requestContent).ConfigureAwait(false))
                {
                    var graphResponseContent = await graphResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        graphResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        throw new ActorProvisioningException($"An error occured while creating the servicePrincipal. The request body was '{servicePrincipalCreationRequestBody}'. The graph response body was '{graphResponseContent}'.", e);
                    }
                    
                    ServicePrincipalResponseBody? createdServicePrincipal;
                    try
                    {
                        createdServicePrincipal = JsonSerializer.Deserialize<ServicePrincipalResponseBody>(graphResponseContent);
                    }
                    catch (JsonException e)
                    {
                        throw new ActorProvisioningException($"An error occured while deserializing the servicePrincipal response body. The request body was '{servicePrincipalCreationRequestBody}'. The graph response body was '{graphResponseContent}'.", e);
                    }

                    if (createdServicePrincipal == null)
                    {
                        throw new ActorProvisioningException($"Microsoft Graph created a servicePrincipal with a null JSON response. The request body was '{servicePrincipalCreationRequestBody}'.");
                    }

                    if (createdServicePrincipal.ObjectId == null)
                    {
                        throw new ActorProvisioningException($"Microsoft Graph created a servicePrincipal with an incomplete JSON response. The following JSON properties were missing: objectId. The request body was '{servicePrincipalCreationRequestBody}'. The graph response body was '{graphResponseContent}'.");
                    }

                    return createdServicePrincipal.ObjectId;
                }
            }
        }

        /// <summary>
        /// The expected structure of the response body for a servicePrincipal object
        /// </summary>
        private sealed class ServicePrincipalResponseBody
        {
            /// <summary>
            /// The 'id' property of the created servicePrincipal object
            /// </summary>
            [JsonPropertyName("id")]
            public string? ObjectId { get; set; }
        }

        /// <summary>
        /// Creates a new Microsoft Graph application in the tenant
        /// </summary>
        /// <param name="applicationDisplayName">The display name of the application to create, assumed to not be <see langword="null"/></param>
        /// <returns>
        /// The newly created application:
        /// ApplicationId: the `appId` property of the application object:
        /// ObjectId: the `id` property of the application object
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidAccessTokenException">Thrown if the token used by <see cref="graphClient"/> is invalid or does not have 'Application.ReadWrite.All' permissions</exception>
        /// <exception cref="ActorProvisioningException">Thrown if an error occurred while creating the application or deserializing the response</exception> //// TODO should you separate these two types? don't you want to know that the application was created so that it can be cleaned up?
        private async Task<(string ApplicationId, string ObjectId)> CreateApplication(string applicationDisplayName)
        {
            var applicationCreationRequestBodyFormat =
"""
{{
    "displayName": "{0}"
}}
""";
            var applicationCreationRequestBody = string.Format(
                applicationCreationRequestBodyFormat,
                applicationDisplayName);
            using (var requestContent = new StringContent(applicationCreationRequestBody, new MediaTypeHeaderValue("application/json")))
            {
                using (var graphResponse = await this.graphClient.PostAsync(new Uri("/applications", UriKind.Relative).ToRelativeUri(), requestContent).ConfigureAwait(false))
                {
                    var graphResponseContent = await graphResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        graphResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        throw new ActorProvisioningException($"An error occured while creating the application. The request body was '{applicationCreationRequestBody}'. The graph response body was '{graphResponseContent}'.", e);
                    }

                    ApplicationResponseBody? createdApplication;
                    try
                    {
                        createdApplication = JsonSerializer.Deserialize<ApplicationResponseBody>(graphResponseContent);
                    }
                    catch (JsonException e)
                    {
                        throw new ActorProvisioningException($"An error occured while deserializing the application response body. The request body was '{applicationCreationRequestBody}'. The graph response body was '{graphResponseContent}'.", e);
                    }

                    if (createdApplication == null)
                    {
                        throw new ActorProvisioningException($"Microsoft Graph created an application with a null JSON response. The request body was '{applicationCreationRequestBody}'.");
                    }

                    var missingProperties = new List<string>();
                    if (createdApplication.ObjectId == null)
                    {
                        missingProperties.Add("id");
                    }

                    if (createdApplication.ApplicationId == null)
                    {
                        missingProperties.Add("appId");
                    }

                    if (missingProperties.Any())
                    {
                        throw new ActorProvisioningException($"Microsoft Graph created an application with an incomplete JSON response. The following JSON properties were missing: {string.Join(", ", missingProperties)}. The request body was '{applicationCreationRequestBody}'. The graph response body was '{graphResponseContent}'.");
                    }

                    return 
                        (
                            createdApplication.ApplicationId!, // the null check adds it to the list of missing properties, which causes an exception if there are any elements, so this is safe
                            createdApplication.ObjectId! // the null check adds it to the list of missing properties, which causes an exception if there are any elements, so this is safe
                        );
                }
            }
        }

        /// <summary>
        /// The expected structure of the response body for an application creation request
        /// </summary>
        private sealed class ApplicationResponseBody
        {
            /// <summary>
            /// The 'id' property of the created application object
            /// </summary>
            [JsonPropertyName("id")]
            public string? ObjectId { get; set; }

            /// <summary>
            /// The 'appId' property of the created application object
            /// </summary>
            [JsonPropertyName("appId")]
            public string? ApplicationId { get; set; }
        }
    }
}
