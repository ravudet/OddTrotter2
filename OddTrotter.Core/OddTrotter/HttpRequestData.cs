namespace OddTrotter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class HttpRequestData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is <see langword="null"/></exception>
        public HttpRequestData(IReadOnlyDictionary<string, IReadOnlyList<string>> form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            this.Form = form.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<string>)kvp.Value.ToList());

            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// A unique value to indicate which POST request is being handled; this is used because blazor will "refresh" the page, and we need a way to know if blazor refreshed vs
        /// if the user refreshed
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// adapted from the underlying <see cref="Microsoft.AspNetCore.Http.HttpRequest.Form"/>
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Form { get; }
    }
}
