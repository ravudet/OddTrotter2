namespace OddTrotter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="HttpRequestExtensions"/>
    /// </summary>
    [TestClass]
    public class HttpRequestExtensionsUnitTests
    {
        /// <summary>
        /// Converts a <see langword="null"/> <see cref="HttpRequest"/> into <see cref="HttpRequestData"/>
        /// </summary>
        [TestMethod]
        public void ToHttpRequestDataNullRequest()
        {
            HttpRequest httpRequest =
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                ;
            Assert.ThrowsException<ArgumentNullException>(() =>
#pragma warning disable CS8604 // Possible null reference argument.
                httpRequest
#pragma warning restore CS8604 // Possible null reference argument.
                    .ToHttpRequestData());
        }

        /// <summary>
        /// Converts a <see cref="HttpRequest"/> into <see cref="HttpRequestData"/>
        /// </summary>
        [TestMethod]
        public void ToHttpRequest()
        {
            var httpRequest = new MockHttpRequest()
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                {
                    { "first", new StringValues(new[] { "a value", "another value" }) },
                }),
            };

            var httpRequestData = httpRequest.ToHttpRequestData();

            Assert.IsTrue(httpRequestData.Form.TryGetValue("first", out var values));
            CollectionAssert.AreEqual(new[] { "a value", "another value" }, values.ToList());
        }

        private sealed class MockHttpRequest : HttpRequest
        {
            public override HttpContext HttpContext => throw new NotImplementedException();

            public override string Method { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string Scheme { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override bool IsHttps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override HostString Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override PathString PathBase { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override PathString Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override QueryString QueryString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override IQueryCollection Query { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string Protocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override IHeaderDictionary Headers => throw new NotImplementedException();

            public override IRequestCookieCollection Cookies { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override string? ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override Stream Body { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override bool HasFormContentType => throw new NotImplementedException();

            public override IFormCollection Form { get; set; } = new FormCollection(new Dictionary<string, StringValues>());

            public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}