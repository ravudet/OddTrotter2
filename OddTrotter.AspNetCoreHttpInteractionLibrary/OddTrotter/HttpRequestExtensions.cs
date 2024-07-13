namespace OddTrotter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Extension methods for <see cref="HttpRequest"/>
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpRequest"/> is <see langword="null"/></exception>
        public static HttpRequestData ToHttpRequestData(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            return new HttpRequestData(httpRequest.Form.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<string>)kvp.Value));
        }
    }
}
