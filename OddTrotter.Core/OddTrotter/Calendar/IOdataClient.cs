////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using OddTrotter.GraphClient;

    public interface IOdataClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeUri"/> is <see langword="null"/></exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">Thrown if the access token used is invalid or provides insufficient privileges for the request</exception> //// TODO this exception is pretty graph specific; how do you want to really model this?
        Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri);
    }
}
