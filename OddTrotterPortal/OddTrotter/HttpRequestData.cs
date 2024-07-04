namespace OddTrotter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public sealed class HttpRequestData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpRequest"/> is <see langword="null"/></exception>
        public HttpRequestData(HttpRequest httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            this.Form = httpRequest.Form.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// A unique value to indicate which POST request is being handled; this is used because blazor will "refresh" the page, and we need a way to know if blazor refreshed vs
        /// if the user refreshed
        /// </summary>
        public Guid Id { get; }

        public IReadOnlyDictionary<string, StringValues> Form { get; }
    }
}
