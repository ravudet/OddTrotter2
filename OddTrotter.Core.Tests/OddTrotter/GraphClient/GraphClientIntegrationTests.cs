namespace OddTrotter.GraphClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OddTrotter.ActorProvisioning;
    using OddTrotter.TokenIssuance;

    /*
The ideal goal for graph integration tests would be that each test has its own tenant and executes whatever it needs to in that tenant, and then the tenant is deleted. If the test fails, it might make sense for the tenant to persist and for there to be a background job that checks for undeleted tenants and generates work items to have those tenants removed. This would allow investigating test failures while still ensuring test tenants are not long-lived. It seems unlikely that we will be able to get one tenant per test, so we can try to mitigate this by allowing tests to share a tenant, but take exclusive or shared locks on the tenant depending on the requirements of that individual tenant. Regardless, we should strive, design, and architect for the world where one tenant per test is a reality.

I am planning 7 stages on that journey:
1. There is a single tenant with the following characteristics:
    a. It is persistent.
    b. It has a single owner user. //// TODO what permissions are needed here?
    c. It has a single application with the following characteristics:
        i. It has 1 client secret.
    d. It has a service principle of that application with the following characteristics:
        i. It has the following app role assignments: `User.ReadWrite.All`, `Application.ReadWrite.All`, and `AppRoleAssignment.ReadWrite.All`.
        ii. The above app role assignments have been consented by an admin
    e. I am unclear if this will happen automatically, but the a service principal for the `Microsoft Graph` application (app ID '00000003-0000-0000-c000-000000000000') must be present

    Each test will share this tenant. The tests will use some form of a reader/writer lock that allows them to obtain exclusive access to the tenant if necessary. The tests will also use a test harness that allows them to provisiong actor resources (applications and users) in the tenant with certain permissions as required by the individual tests.

    The test harness will use the persistent application in the tenant to to provision these actors and grant them permissions. The test harness will also delete this actors once the test has finished executing, resulting in a clean state for the tenant when the next test runs.

    In order to instantiate the test harness, at this stage, a configuration file will be used that contains the domain name of the tenant that the tests will run in, the application ID of the persistent application in that tenant, and the client secret on that application that is used for authentication. 

    NOTES: Though this is a pretty decent end-state for this stage, we don't want to stop here because this stage has the following security issues:
    1. The configuration file contains sensitive data and is persisted to the disk
    2. There is a persistent test tenant which could be compromised
    3. The tenant has a persist application with extremely high privileges that could be compromised
    4. The tenant has a persist service principal with extremely high privileges that could be compromised
    5. There is a persistent user with TODO
    6. There is no clean up of resouces in the event of catastrophic failures during the test run

2. Create a script that generates the test configuration file. This script will be run by the user who wants to run the integration tests prior to executing the integration tests. The script will use the graph powershell cmdlets to prompt the user to login to the pesistent tenant. Once logged in, the user will be prompted to select the application to use for the test run. This script will put the test configuration into a known location, which will be included in the `.gitignore` file.

    The script helps mitigate (but does not completely mitigate) 1.1 because the test configuration will now be on disk in locations where `git` will not publish them to the repository, and `git clean` will remove the test configurations. TODO can you remove the permissions from the persistent user if you do this?

    NOTES: This stage has basically all of the same security issues and introduces a new issue for developers
    1. Developers must be aware that they should run the test script prior to executing the tests, though the tests *should* inform the developer if the test configuration is not present. 

3. The application from 1.c and service principal from 1.d should be deleted. The script from 2 should be updated to create the application, add a client secret, create the service principal, assign the permissions, and grant admin consent to those permissions. It should do this instead of asking the user which application should be used for the test run. The tests should also be updated to delete the new resources at the end of the test run, as well as delete the test configuration file.

    This stage mitigates 1.3 and 1.4. These two resources will now be created for each test run and that test run will automatically delete them, so they are no longer persistent. This stage also helps mitigate 1.1 because the configuration file will now remain on disk for less time, meaning that mistakes where that test file ends up published somewhere have a smaller time window where they can happen.

    TODO NOTES: this will require the user to have more permissions

4. The test projects should all coordinate and use an assembly initialize in some way to run the script from 3 at the beginning of a test run. The script itself actually should not be run, but instead should be translated to c# code. Once this is done, the test harness should no longer be initialized using a test configuration file, but instead should be initialized using an in-memory test configuration.

    This stage mitigates 1.1 because the configuration is now entirely in memory. It also mitigates 2.1 because developers will now be automatically prompted for the test configuration to be generated.

    NOTES: This stage introduces:
    1. A developer user interaction component during test runs. This means that the test can only be run manually and not in an automated fashion.

5. The test configuration script and the test harness should be updated to not assign permissions to resources directly, but to instead assign them using permissions-on-demand features. 

    This stages helps mitigate 1.6 because actor resources that are created during the test run will not have permissions permanents, and they will eventually expire.

6. There should be a background script that runs, checking for resources that were created during a test run, and creating work items to request those resources be cleaned up. The check for resources should be relatively straightforward since the only resources that should be persistent in the tenant at this point are 1.b and 1.e.

    This stage mitigates the remainder of 1.6 that stage 5 didn't already mitigate. There will now be no actors with permanent permissions, and resources will be cleaned up on a manual basis whenever tests runs do not clean up after themsevles due to catastrophic failure.

7. The script introduced in 2 should no longer prompt the user to login to the test tenant. The test tenant should be deleted entirely. The script will now prompt the user to create a new temporary tenant, displaying the browser whenever appropriate. The script will then use that tenant to provision the test runner application and service principal, generate the test configuration, etc. The tenant details should be logged immediately after creation so that the background script from 6 can find those tenants if they haven't been cleaned up. The tests should be updated to delete the tenant after the tests have been completed.

    This stage mitigates 1.2 and 1.5. There are no persistent test resources.

    NOTES: The only remaining issue is not a security issue. It is 4.1. User interaction is required to run the integration tests. It is also somewhat extensive user interaction. If graph ever allows creating tenants in a more automated fashion, that should be leveraged. 
*/

    /// <summary>
    /// Integration tests for <see cref="GraphClient"/>
    /// </summary>
    /// <remarks>
    /// Note that these tests require:
    /// 1. A network connection with access to https://graph.microsoft.com/v1.0
    /// </remarks>
    [TestCategory(TestCategories.Integration)]
    [TestClass]
    public sealed class GraphClientIntegrationTests
    {
        /// <summary>
        /// A <see cref="IGraphTestHarnessFactory"/> for use in <see cref="GraphClientIntegrationTests"/>
        /// </summary>
        private sealed class GraphClientIntegrationTestHarnessFactory : IGraphTestHarnessFactory
        {
            /// <summary>
            /// The string that will prefix the display name of all resources provisioned by the created test harnesses
            /// </summary>
            private readonly string testRunPrefix;

            /// <summary>
            /// The domain name of the tenant to use during this test run
            /// </summary>
            private readonly string tenantDomainName;

            /// <summary>
            /// The <see cref="ITokenIssuer"/> to use during this test run
            /// </summary>
            private readonly ITokenIssuer tokenIssuer;

            /// <summary>
            /// The <see cref="IGraphClient"/> to use to provision new actors during this test run. To provision new actors, the token used by this client must have been issued for an application with the 'Application.ReadWrite.All' and 'AppRoleAssignment.ReadWrite.All' app role assignments. Both assignments need to be admin consented.
            /// </summary>
            private readonly IGraphClient testRunGraphClient;

            /// <summary>
            /// Prevents initialization of a default <see cref="GraphClientIntegrationTestHarnessFactory"/>
            /// </summary>
            /// <param name="testRunPrefix">The string that will prefix the display name of all resources provisioned by the created test harnesses</param>
            /// <param name="tenantDomainName">The domain name of the tenant to use during this test run</param>
            /// <param name="tokenIssuer">The <see cref="ITokenIssuer"/> to use during this test run</param>
            /// <param name="testRunGraphClient">The <see cref="IGraphClient"/> to use to provision new actors during this test run. To provision new actors, the token used by this client must have been issued for an application with the 'Application.ReadWrite.All' and 'AppRoleAssignment.ReadWrite.All' app role assignments. Both assignments need to be admin consented.</param>
            private GraphClientIntegrationTestHarnessFactory(string testRunPrefix, string tenantDomainName, ITokenIssuer tokenIssuer, IGraphClient testRunGraphClient)
            {
                this.testRunPrefix = testRunPrefix;
                this.tenantDomainName = tenantDomainName;
                this.tokenIssuer = tokenIssuer;
                this.testRunGraphClient = testRunGraphClient;
            }

            /// <summary>
            /// The singleton instance of the <see cref="GraphClientIntegrationTestHarnessFactory"/>
            /// </summary>
            /// <exception cref="FileNotFoundException">Thrown if the configuration file is not found at 'c:\secrets.json'</exception>
            /// <exception cref="IOException">Thrown if an error occurred reading the 'c:\secrets.json' file</exception>
            /// <exception cref="UnauthorizedAccessException">Thrown if the test runner process does not have permissions to access 'c:\secrets.json'</exception>
            /// <exception cref="JsonException">Thrown if the configuration file at 'c:\secrets.json' is not a valid JSON file</exception>
            /// <exception cref="InvalidSecretsException">Thrown if the configuration file at 'c:\secrets.json' did not contain all of the necessary properties</exception>
            /// <exception cref="HttpRequestException">
            /// Thrown if token issuance failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
            /// </exception>
            /// <exception cref="TokenIssuanceException">Thrown if the token issuer encountered an error authenticating the actor that was configured in 'c:\secrets.json' or encountered an error issuing a token for that actor</exception>
            public static IGraphTestHarnessFactory Instance { get; } = Create().ConfigureAwait(false).GetAwaiter().GetResult();

            /// <summary>
            /// Creates a <see cref="IGraphTestHarnessFactory"/> using hard-coded configuration file 'c:\secrets.json'. Those configuration files must specify an application with the 'Application.ReadWrite.All' and 'AppRoleAssignment.ReadWrite.All' app role assignments. Both assignments need to be admin consented.
            /// </summary>
            /// <returns>The <see cref="IGraphTestHarnessFactory"/> to use for the entire test run</returns>
            /// <exception cref="FileNotFoundException">Thrown if the configuration file is not found at 'c:\secrets.json'</exception>
            /// <exception cref="IOException">Thrown if an error occurred reading the 'c:\secrets.json' file</exception>
            /// <exception cref="UnauthorizedAccessException">Thrown if the test runner process does not have permissions to access 'c:\secrets.json'</exception>
            /// <exception cref="JsonException">Thrown if the configuration file at 'c:\secrets.json' is not a valid JSON file</exception>
            /// <exception cref="InvalidSecretsException">Thrown if the configuration file at 'c:\secrets.json' did not contain all of the necessary properties</exception>
            /// <exception cref="HttpRequestException">
            /// Thrown if token issuance failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
            /// </exception>
            /// <exception cref="TokenIssuanceException">Thrown if the token issuer encountered an error authenticating the actor that was configured in 'c:\secrets.json' or encountered an error issuing a token for that actor</exception>
            private static async Task<IGraphTestHarnessFactory> Create()
            {
                var testRunActor = await GetTestRunActor().ConfigureAwait(false);
                var tokenIssuer = TokenIssuer.Default;
                var issuedToken = await tokenIssuer.IssueToken(testRunActor).ConfigureAwait(false);

                var testRunGraphClient = new GraphClient(issuedToken.AccessToken, new GraphClientSettings.Builder().Build());

                return new GraphClientIntegrationTestHarnessFactory(
                    DateTime.UtcNow.ToString("O"), // we are using 'O' as defined [here](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) so that we can easily find resources provisioned by the test run using requests like `GET https://graph.microsoft.com/v1.0/applications?$filter=startswith(displayName, '2024')`. This also allows finding test runs by the date that they ran and things along those lines
                    testRunActor.TenantDomain,
                    tokenIssuer,
                    testRunGraphClient);
            }

            /// <summary>
            /// Reads the 'c:\secrets.json' configuration file and initializes the <see cref="Actor"/> that will be used for this test run using the data in the configuration. The actor configured in the JSON file must be an application with the 'Application.ReadWrite.All' and 'AppRoleAssignment.ReadWrite.All' app role assignments. Both assignments need to be admin consented.
            /// </summary>
            /// <returns>The <see cref="Actor"/> that will be used for this test run.</returns>
            /// <exception cref="FileNotFoundException">Thrown if the configuration file is not found at 'c:\secrets.json'</exception>
            /// <exception cref="IOException">Thrown if an error occurred reading the 'c:\secrets.json' file</exception>
            /// <exception cref="UnauthorizedAccessException">Thrown if the test runner process does not have permissions to access 'c:\secrets.json'</exception>
            /// <exception cref="JsonException">Thrown if the configuration file at 'c:\secrets.json' is not a valid JSON file</exception>
            /// <exception cref="InvalidSecretsException">Thrown if the configuration file at 'c:\secrets.json' did not contain all of the necessary properties</exception>
            private static async Task<Actor> GetTestRunActor()
            {
                const string configurationFilePath = @"c:\secrets.json";
                const string tenantNameDescription = "'tenantName' (the domain name of the tenant to use for testing)";
                const string appIdDescription = "'appId' (the appId of the application in that tenant that individual tests will use to provision further actor resources; this application must have the 'Application.ReadWrite.All' and 'AppRoleAssignment.ReadWrite.All' app role assignments; both assignments need to be admin consented)";
                const string secretDescription = "'secret' (the text of the secret that is on the application that will be used for authentication)";

                Stream? secretsFile = null;
                try
                {
                    try
                    {
                        secretsFile = File.OpenRead(configurationFilePath);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        // there is no directory in the path we are using, so this should never be thrown
                        throw;
                    }
                    catch (FileNotFoundException e)
                    {
                        throw new FileNotFoundException($"Could not find the configuration file at '{configurationFilePath}'. The configuration file is a JSON blob with {tenantNameDescription}, {appIdDescription}, and {secretDescription}.", e);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        throw new UnauthorizedAccessException($"The test runner could not access the configuration file at '{configurationFilePath}'.", e);
                    }
                    catch (IOException e)
                    {
                        throw new IOException($"An error occurred while reading the test configuration file at '{configurationFilePath}'.", e);
                    }

                    var secrets = await JsonSerializer.DeserializeAsync<Secrets>(secretsFile).ConfigureAwait(false);
                    if (secrets == null)
                    {
                        throw new InvalidSecretsException($"The configuration file at '{configurationFilePath}' is null");
                    }

                    var missingProperties = new List<string>();
                    if (secrets.ApplicationId == null)
                    {
                        missingProperties.Add(appIdDescription);
                    }

                    if (secrets.Secret == null)
                    {
                        missingProperties.Add(secretDescription);
                    }

                    if (secrets.TenantName == null)
                    {
                        missingProperties.Add(tenantNameDescription);
                    }

                    if (missingProperties.Any())
                    {
                        throw new InvalidSecretsException($"The configuration file at '{configurationFilePath}' did not contain the following JSON properties: {string.Join(", ", missingProperties)}");
                    }

                    return new Actor(
                        secrets.ApplicationId!, // the null check adds it to the list of missing properties, which causes an exception if there are any elements, so this is safe
                        secrets.Secret!, // the null check adds it to the list of missing properties, which causes an exception if there are any elements, so this is safe
                        secrets.TenantName! // the null check adds it to the list of missing properties, which causes an exception if there are any elements, so this is safe
                        );
                }
                finally
                {
                    if (secretsFile != null)
                    {
                        await secretsFile.DisposeAsync().ConfigureAwait(false);
                    }
                }
            }

            /// <inheritdoc/>
            public async Task<GraphTestHarness> CreateTestHarnessAsync()
            {
                var testGuid = Guid.NewGuid();
                var resourceDisplayNamePrefix = this.testRunPrefix + ";" + testGuid;
                Console.WriteLine($"The actor resources created by this test will have a display name that is prefixed with '{resourceDisplayNamePrefix}'");

                var actorProvisioner = new ActorProvisioner(
                    this.testRunGraphClient,
                    this.tenantDomainName,
                    new ActorProvisionerSettings.Builder()
                    {
                        ResourceDisplayNamePrefix = resourceDisplayNamePrefix,
                    }.Build());

                return await Task.FromResult(new GraphTestHarness(this.tokenIssuer, actorProvisioner)).ConfigureAwait(false);
            }

            /// <summary>
            /// A representation of the structure of the secrets.json blob that is used to configure the test run actor that provisions resources for individual tests
            /// </summary>
            private sealed class Secrets
            {
                /// <summary>
                /// The appId of the application in that tenant that individual tests will use to provision further actor resources
                /// </summary>
                [JsonPropertyName("appId")]
                public string? ApplicationId { get; set; }

                /// <summary>
                /// The text of the secret that is on the application that will be used for authentication
                /// </summary>
                [JsonPropertyName("secret")]
                public string? Secret { get; set; }

                /// <summary>
                /// The domain name of the tenant to use for testing
                /// </summary>
                [JsonPropertyName("tenantName")]
                public string? TenantName { get; set; }
            }

            /// <summary>
            /// Thrown if the contents of a secrets.json configuration file is not a valid <see cref="Secrets"/>
            /// </summary>
            private sealed class InvalidSecretsException : Exception
            {
                public InvalidSecretsException(string message)
                    : base(message)
                {
                }
            }
        }

        /// <summary>
        /// The <see cref="IGraphTestHarnessFactory"/> that can be used to issue real tokens that can be used to authorize clients against Microsoft Graph
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown if the configuration file is not found at 'c:\secrets.json'</exception>
        /// <exception cref="IOException">Thrown if an error occurred reading the 'c:\secrets.json' file</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the test runner process does not have permissions to access 'c:\secrets.json'</exception>
        /// <exception cref="JsonException">Thrown if the configuration file at 'c:\secrets.json' is not a valid JSON file</exception>
        /// <exception cref="InvalidSecretsException">Thrown if the configuration file at 'c:\secrets.json' did not contain all of the necessary properties</exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if token issuance failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        /// <exception cref="TokenIssuanceException">Thrown if the token issuer encountered an error authenticating the actor that was configured in 'c:\secrets.json' or encountered an error issuing a token for that actor</exception>
        private static IGraphTestHarnessFactory TestHarnessFactory { get; } = GraphClientIntegrationTestHarnessFactory.Instance;

        /// <summary>
        /// Retrieves a user that does not exist
        /// </summary>
        [TestMethod]
        public async Task GetNonexistentUserAbsoluteUri()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetNonexistentUserAbsoluteUri),
                new[]
                {
                    AppRole.Application.UserReadAll,
                }).ConfigureAwait(false))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                using (var httpResponse = await graphClient.GetAsync(new Uri("https://graph.microsoft.com/v1.0/users/00000000-0000-0000-000000000000", UriKind.Absolute).ToAbsoluteUri()).ConfigureAwait(false))
                {
                    Assert.AreEqual(HttpStatusCode.NotFound, httpResponse.StatusCode, await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }
        }

        /// <summary>
        /// Retrieves a user when there is no network
        /// </summary>
        [TestMethod]
        public async Task GetUserWithNoNetworkAbsoluteUri()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithNoNetworkAbsoluteUri),
                Enumerable.Empty<AppRole>()))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                await Assert.ThrowsExceptionAsync<HttpRequestException>(() => graphClient.GetAsync(new Uri("https://0.0.0.0/v1.0/users/00000000-0000-0000-000000000000", UriKind.Absolute).ToAbsoluteUri())).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves a user with an invalid access token
        /// </summary>
        [TestMethod]
        public async Task GetUserWithInvalidAccessTokenAbsoluteUri()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithInvalidAccessTokenAbsoluteUri),
                Enumerable.Empty<AppRole>()).ConfigureAwait(false))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                await Assert.ThrowsExceptionAsync<UnauthorizedAccessTokenException>(() => graphClient.GetAsync(new Uri("https://graph.microsoft.com/v1.0/users/00000000-0000-0000-000000000000", UriKind.Absolute).ToAbsoluteUri())).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves a user with an access token that doesn't have read permissions
        /// </summary>
        [TestMethod]
        public async Task GetUserWithNoPermissionsAbsoluteUri()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithInvalidAccessTokenAbsoluteUri),
                Enumerable.Empty<AppRole>()).ConfigureAwait(false))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                await Assert.ThrowsExceptionAsync<UnauthorizedAccessTokenException>(() => graphClient.GetAsync(new Uri("https://graph.microsoft.com/v1.0/servicePrincipals", UriKind.Absolute).ToAbsoluteUri())).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves a user that does not exist
        /// </summary>
        [TestMethod]
        public async Task GetNonexistentUserRelativeUri()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetNonexistentUserAbsoluteUri),
                new[]
                {
                    AppRole.Application.UserReadAll,
                }).ConfigureAwait(false))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                using (var httpResponse = await graphClient.GetAsync(new Uri("/users/00000000-0000-0000-000000000000", UriKind.Relative).ToRelativeUri()).ConfigureAwait(false))
                {
                    Assert.AreEqual(HttpStatusCode.NotFound, httpResponse.StatusCode, await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }
        }

        /// <summary>
        /// Retrieves a user when there is no network
        /// </summary>
        [TestMethod]
        public async Task GetUserWithNoNetworkRelativeUri()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithNoNetworkAbsoluteUri),
                Enumerable.Empty<AppRole>()))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClientSettings = new GraphClientSettings.Builder()
                {
                    GraphRootUri = new Uri("https://0.0.0.0/v1.0", UriKind.Absolute).ToAbsoluteUri(),
                }.Build();
                var graphClient = new GraphClient(testApplicationToken.AccessToken, graphClientSettings);
                await Assert.ThrowsExceptionAsync<HttpRequestException>(() => graphClient.GetAsync(new Uri("/users/00000000-0000-0000-000000000000", UriKind.Relative).ToRelativeUri())).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves a user with an invalid access token
        /// </summary>
        [TestMethod]
        public async Task GetUserWithInvalidAccessTokenRelativeUri()
        {
            var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build());
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessTokenException>(() => graphClient.GetAsync(new Uri("/users/00000000-0000-0000-000000000000", UriKind.Relative).ToRelativeUri())).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a user with an access token that doesn't have read permissions
        /// </summary>
        [TestMethod]
        public async Task GetUserWithNoPermissionsRelativeUri()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithNoNetworkAbsoluteUri),
                Enumerable.Empty<AppRole>()))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                await Assert.ThrowsExceptionAsync<UnauthorizedAccessTokenException>(() => graphClient.GetAsync(new Uri("/servicePrincipals", UriKind.Relative).ToRelativeUri())).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Updates a user that does not exist
        /// </summary>
        [TestMethod]
        public async Task PatchNonexistentUser()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithNoNetworkAbsoluteUri),
                new[]
                {
                    AppRole.Application.UserReadWriteAll,
                }))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                using (var content = new StringContent(
"""
{
  "displayName": "test"
}
""",
                    new MediaTypeHeaderValue("application/json")))
                {
                    using (var httpResponse = await graphClient.PatchAsync(new Uri("/users/00000000-0000-0000-000000000000", UriKind.Relative).ToRelativeUri(), content).ConfigureAwait(false))
                    {
                        Assert.AreEqual(HttpStatusCode.NotFound, httpResponse.StatusCode, await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));
                    }
                }
            }
        }

        /// <summary>
        /// Updates a user when there is no network
        /// </summary>
        [TestMethod]
        public async Task PatchUserWithNoNetwork()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithNoNetworkAbsoluteUri),
                Enumerable.Empty<AppRole>()))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClientSettings = new GraphClientSettings.Builder()
                {
                    GraphRootUri = new Uri("https://0.0.0.0/v1.0", UriKind.Absolute).ToAbsoluteUri(),
                }.Build();
                var graphClient = new GraphClient(testApplicationToken.AccessToken, graphClientSettings);
                using (var content = new StringContent(
"""
{
  "displayName": "test"
}
""",
                    new MediaTypeHeaderValue("application/json")))
                {
                    await Assert.ThrowsExceptionAsync<HttpRequestException>(() => graphClient.PatchAsync(new Uri("/users/00000000-0000-0000-000000000000", UriKind.Relative).ToRelativeUri(), content)).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Update a user with an invalid access token
        /// </summary>
        [TestMethod]
        public async Task PatchUserWithInvalidAccessToken()
        {
            var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build());
            using (var content = new StringContent(
"""
{
  "displayName": "test"
}
""",
                new MediaTypeHeaderValue("application/json")))
            {
                await Assert.ThrowsExceptionAsync<UnauthorizedAccessTokenException>(() => graphClient.PatchAsync(new Uri("/users/00000000-0000-0000-000000000000", UriKind.Relative).ToRelativeUri(), content)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Updates a service principal with an access token that doesn't have write permissions
        /// </summary>
        [TestMethod]
        public async Task PatchServicePrincipalWithNoPermissions()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithNoNetworkAbsoluteUri),
                Enumerable.Empty<AppRole>()))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                using (var content = new StringContent(
"""
{
  "displayName": "test"
}
""",
                    new MediaTypeHeaderValue("application/json")))
                {
                    await Assert.ThrowsExceptionAsync<UnauthorizedAccessTokenException>(() => graphClient.PatchAsync(new Uri("/servicePrincipals(appId='00000003-0000-0000-c000-000000000000')", UriKind.Relative).ToRelativeUri(), content)).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Creates a new application
        /// </summary>
        [TestMethod]
        public async Task PostApplication()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithNoNetworkAbsoluteUri),
                new[]
                {
                    AppRole.Application.ApplicationReadWriteAll,
                }))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                using (var content = new StringContent(
"""
{
  "displayName": "testing"
}
""",
                    new MediaTypeHeaderValue("application/json")))
                {
                    using (var httpResponse = await graphClient.PostAsync(new Uri("/applications", UriKind.Relative).ToRelativeUri(), content).ConfigureAwait(false))
                    {
                        Assert.AreEqual(HttpStatusCode.Created, httpResponse.StatusCode, await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));
                    }
                }
            }
        }

        /// <summary>
        /// Creates a user when there is no network
        /// </summary>
        [TestMethod]
        public async Task PostUserWithNoNetwork()
        {
            var graphClientSettings = new GraphClientSettings.Builder()
            {
                GraphRootUri = new Uri("https://0.0.0.0/v1.0", UriKind.Absolute).ToAbsoluteUri(),
            }.Build();
            var graphClient = new GraphClient("sometoken", graphClientSettings);
            using (var content = new StringContent(
"""
{
  "displayName": "test"
}
""",
                new MediaTypeHeaderValue("application/json")))
            {
                await Assert.ThrowsExceptionAsync<HttpRequestException>(() => graphClient.PostAsync(new Uri("/users", UriKind.Relative).ToRelativeUri(), content)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates a user with an invalid access token
        /// </summary>
        [TestMethod]
        public async Task PostUserWithInvalidAccessToken()
        {
            var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build());
            using (var content = new StringContent(
"""
{
  "displayName": "test"
}
""",
                new MediaTypeHeaderValue("application/json")))
            {
                await Assert.ThrowsExceptionAsync<UnauthorizedAccessTokenException>(() => graphClient.PostAsync(new Uri("/users", UriKind.Relative).ToRelativeUri(), content)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates an application with an access token that doesn't have write permissions
        /// </summary>
        [TestMethod]
        public async Task PostApplicationWithNoPermissions()
        {
            var testHarness = await TestHarnessFactory.CreateTestHarnessAsync().ConfigureAwait(false);

            await using (var testApplication = await testHarness.ActorProvisioner.ProvisionApplication(
                nameof(GetUserWithNoNetworkAbsoluteUri),
                Enumerable.Empty<AppRole>()))
            {
                var testApplicationToken = await testHarness.TokenIssuer.IssueToken(testApplication).ConfigureAwait(false);
                var graphClient = new GraphClient(testApplicationToken.AccessToken, new GraphClientSettings.Builder().Build());
                using (var content = new StringContent(
"""
{
  "displayName": "testing"
}
""",
                    new MediaTypeHeaderValue("application/json")))
                {
                    await Assert.ThrowsExceptionAsync<UnauthorizedAccessTokenException>(() => graphClient.PostAsync(new Uri("/applications", UriKind.Relative).ToRelativeUri(), content)).ConfigureAwait(false);
                }
            }
        }
    }
}
