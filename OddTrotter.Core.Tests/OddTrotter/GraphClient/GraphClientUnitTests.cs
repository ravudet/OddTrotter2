namespace OddTrotter.GraphClient
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OddTrotter.Calendar;

    /// <summary>
    /// Unit tests for <see cref="GraphClient"/>
    /// </summary>
    [TestClass]
    public sealed class GraphClientUnitTests
    {
        private sealed class MockGraphClient : IGraphClient
        {
            public Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> PatchAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> PostAsync(RelativeUri relativeUri, HttpContent httpContent)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void Test()
        {
            /*
            var url =
                $"/me/calendar/events?" +
                $"$select=body,start,subject,responseStatus,webLink&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss.000000")}' and isCancelled eq false";
            */

            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events;


            var eventsIdSelected = events
                .Select(calendarEvent => calendarEvent.Id);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            events = eventsIdSelected
                .Filter()
                .Select(calendarEvent => calendarEvent.Body)
                .Select(calendarEvent => calendarEvent.Start)
                .Select(calendarEvent => calendarEvent.Subject)
                .Select(calendarEvent => calendarEvent.ResponseStatus)
                .Select(calendarEvent => calendarEvent.WebLink)
                .Select(calendarEvent => calendarEvent.Body.Content);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var values = events.Values;
        }

        /// <summary>
        /// Creates a new <see cref="GraphClient"/> with a <see langword="null"> access token
        /// </summary>
        [TestMethod]
        public void NullAccessToken()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new GraphClient(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                new GraphClientSettings.Builder().Build()).Dispose());
        }

        /// <summary>
        /// Creates a new <see cref="GraphClient"/> with a whitespace access token
        /// </summary>
        [TestMethod]
        public void WhitespaceAccessToken()
        {
            Assert.ThrowsException<ArgumentException>(() => new GraphClient(
                "   \t",
                new GraphClientSettings.Builder().Build()).Dispose());
        }

        /// <summary>
        /// Creates a new <see cref="GraphClient"/> with a <see langword="null"> settings
        /// </summary>
        [TestMethod]
        public void NullSettings()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new GraphClient(
                "sometoken",
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ).Dispose());
        }

        /// <summary>
        /// Creates a new <see cref="GraphClient"/> with a <see langword="null"> settings
        /// </summary>
        [TestMethod]
        public void InvalidAccessToken()
        {
            Assert.ThrowsException<ArgumentException>(() => new GraphClient(
                Environment.NewLine + "sometoken",
                new GraphClientSettings.Builder().Build()).Dispose());
        }

        /// <summary>
        /// Retrieves a <see langword="null"/> URI
        /// </summary>
        [TestMethod]
        public async Task GetNullRelativeUri()
        {
            using (var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => graphClient.GetAsync(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    (RelativeUri)null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    )).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Retrieves a <see langword="null"/> URI
        /// </summary>
        [TestMethod]
        public async Task GetNullAbsoluteUri()
        {
            using (var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => graphClient.GetAsync(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    (AbsoluteUri)null
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    )).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Updates a <see langword="null"/> URI
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PatchNullUri()
        {
            using (var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build()))
            {
                using (var content = new StringContent(string.Empty))
                {
                    await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => graphClient.PatchAsync(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                        content
                        )).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Updates with <see langword="null"/> content
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PatchNullContent()
        {
            using (var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => graphClient.PatchAsync(
                    new Uri("/someuri", UriKind.Relative).ToRelativeUri(),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    )).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates a <see langword="null"/> URI
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostNullUri()
        {
            using (var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build()))
            {
                using (var content = new StringContent(string.Empty))
                {
                    await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => graphClient.PostAsync(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                        content
                        )).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Creates with <see langword="null"/> content
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostNullContent()
        {
            using (var graphClient = new GraphClient("sometoken", new GraphClientSettings.Builder().Build()))
            {
                await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => graphClient.PostAsync(
                    new Uri("/someuri", UriKind.Relative).ToRelativeUri(),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    )).ConfigureAwait(false);
            }
        }
    }
}
