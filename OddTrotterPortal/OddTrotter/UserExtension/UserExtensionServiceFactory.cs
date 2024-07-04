namespace OddTrotter.UserExtension
{
    using System;

    using Microsoft.Extensions.Configuration;

    using global::OddTrotter.Encryptor;
    using global::OddTrotter.GraphClient;

    public static class UserExtensionServiceFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpRequestData"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpRequestData"/> or <paramref name="configuration"/> is <see langword="null"/></exception>
        /// <exception cref="GraphUriException">
        /// Thrown if the graph root URL specified in <paramref name="configuration"/> is not a valid URL or it is not an absolute URL
        /// </exception>
        /// <exception cref="MissingFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained missing fields</exception>
        /// <exception cref="InvalidFormDataException">Thrown if the form data in <paramref name="httpRequestData"/> contained fields with invalid data</exception>
        public static UserExtensionService Create(HttpRequestData httpRequestData, IConfiguration configuration)
        {
            if (httpRequestData == null)
            {
                throw new ArgumentNullException(nameof(httpRequestData));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var graphClient = GraphClientFactory.Create(httpRequestData, configuration);
            var encryptor = EncryptorFactory.Create(httpRequestData);

            return new UserExtensionService(graphClient, encryptor);
        }
    }
}
