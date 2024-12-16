namespace OddTrotter
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="HttpRequestData"/>
    /// </summary>
    [TestCategory(TestCategories.Unit)]
    [TestClass]
    public sealed class HttpRequestDataUnitTests
    {
        /// <summary>
        /// Creates <see cref="HttpRequestData"/> with <see langword="null"/> form data
        /// </summary>
        [TestMethod]
        public void NullForm()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new HttpRequestData(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        /// <summary>
        /// Retrieves the form data after the original has been modified
        /// </summary>
        [TestMethod]
        public void ModifiedFormData()
        {
            var form = new Dictionary<string, IReadOnlyList<string>>()
            {
                { "key", new List<string>() { "a value" } },
            };
            var httpRequestData = new HttpRequestData(form);
            form.Add("key2", new List<string>());
            Assert.IsFalse(httpRequestData.Form.TryGetValue("key2", out _));
        }

        /// <summary>
        /// Compares the ID of two <see cref="HttpRequestData"/>s
        /// </summary>
        [TestMethod]
        public void UniqueId()
        {
            var first = new HttpRequestData(new Dictionary<string, IReadOnlyList<string>>());
            var second = new HttpRequestData(new Dictionary<string, IReadOnlyList<string>>());
            Assert.AreNotEqual(first.Id, second.Id);
        }

        private static bool Started { get; set; } = false;

        [TestMethod]
        public void HttpClientHeaderTest()
        {
            //// https://github.com/jeske/SimpleHttpServer/blob/master/SimpleHttpServer/HttpProcessor.cs
            var tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => Listen(tokenSource.Token));
            while (!Started)
            {
            }

            Task.Factory.StartNew(() => Send(tokenSource.Token));

            while (true)
            {
                Task.Delay(1000).ConfigureAwait(false).GetAwaiter().GetResult();
            }

#pragma warning disable CS0162 // Unreachable code detected
            tokenSource.Cancel();
#pragma warning restore CS0162 // Unreachable code detected
        }

        private static void Send(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using (var client = new HttpClient())
                {
                    using (var response = client.GetAsync("http://localhost:8080").ConfigureAwait(false).GetAwaiter().GetResult())
                    {
                        var responseContent = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                }
            }
        }

        private static void Listen(CancellationToken token)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var listener = new TcpListener(8080);
#pragma warning restore CS0618 // Type or member is obsolete
            listener.Start();
            Started = true;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    using (var client = listener.AcceptTcpClient())
                    {
                        var networkStream = client.GetStream();
                        var data = new byte[4 * 1024 * 1024];
                        while (networkStream.Read(data, 0, data.Length) != 0)
                        {
                        }

                        var response =
"""
HTTP/1.1 200 OK
badheader

the body
""";
                        var responseBytes = System.Text.Encoding.ASCII.GetBytes(response);
                        networkStream.Write(responseBytes, 0, responseBytes.Length);
                    }
                }
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
