namespace OddTrotter.AzureBlobClient
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IAzureBlobClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="blobName"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="blobName"/> is empty</exception>
        /// <exception cref="InvalidBlobNameException">Thrown if <paramref name="blobName"/> results in an invalid URL or points to a blob that cannot be read</exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
        /// </exception>
        /// <exception cref="InvalidSasTokenException">Thrown if the configured SAS token is not a valid SAS token</exception>
        /// <exception cref="SasTokenNoReadPrivilegesException">
        /// Thrown if the configured SAS token does not have read permissions for the blob with name <paramref name="blobName"/>
        /// </exception>
        Task<HttpResponseMessage> GetAsync(string blobName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="blobName"/> or <paramref name="httpContent"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="blobName"/> is empty</exception>
        /// <exception cref="InvalidBlobNameException">Thrown if <paramref name="blobName"/> results in an invalid URL</exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
        /// </exception>
        /// <exception cref="InvalidSasTokenException">Thrown if the configured SAS token is not a valid SAS token</exception>
        /// <exception cref="SasTokenNoWritePrivilegesException">
        /// Thrown if the configured SAS token does not have write or create permissions for the blob with name <paramref name="blobName"/>
        /// </exception>
        /// <exception cref="InvalidBlobDataException">Thrown if that data in <paramref name="httpContent"/> cannot be written to the blob</exception>
        Task<HttpResponseMessage> PutAsync(string blobName, HttpContent httpContent);
    }
}
