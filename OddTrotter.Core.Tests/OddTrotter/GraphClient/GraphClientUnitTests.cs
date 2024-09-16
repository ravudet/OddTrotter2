namespace OddTrotter.GraphClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OddTrotter.Calendar;
    using OddTrotter.TodoList;

    [TestClass]
    public sealed class MigrationTestsOdataPocRootCalendarContext
    {
        private sealed class MockGraphClient : IGraphClient
        {
            public string CalledUri { get; private set; } = string.Empty;

            public async Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri)
            {
                this.CalledUri = relativeUri.OriginalString;

                var absoluteUri = new AbsoluteUri(new Uri("https://localhost/" + relativeUri.OriginalString.TrimStart('/'), UriKind.Absolute)).ToAbsoluteUri();

                if (string.Equals(absoluteUri.LocalPath, "/me/calendar"))
                {
                    var content = new StringContent("{\"id\":\"calendar_id\",\"events\":[{\"id\":\"event_id_1\"}]}");
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    responseMessage.Content = content;
                    return await Task.FromResult(responseMessage);
                }
                else if (string.Equals(absoluteUri.LocalPath, "/me/calendar/events"))
                {
                    var content = new StringContent("{\"value\":[]}");
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    responseMessage.Content = content;
                    return await Task.FromResult(responseMessage);
                }
                else
                {
                    throw new NotSupportedException("TODO");
                }
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
        public async Task SelectsNew()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new Fx.OdataPocRoot.GraphContext.CalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri(), new Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations.SelectToStringVisitor());
            var calendar = await graphCalendarContext
                .Select(calendar => calendar.Id)
                .Select(calendar => calendar.Events)
                .Evaluate()
                .ConfigureAwait(false);

            Assert.AreEqual("/me/calendar?$select=id,events", graphClient.CalledUri);
        }
    }

    //// TODO remove the need for this directive
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    [TestClass]
    public sealed class MigrationTestsODataQueryableUnitTests
    {
        private sealed class MockGraphClient : IGraphClient
        {
            public string CalledUri { get; private set; } = string.Empty;

            public async Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri)
            {
                this.CalledUri = relativeUri.OriginalString;

                var content = new StringContent("{\"value\":[]}");
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = content;
                return await Task.FromResult(responseMessage);
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
        public void Selects()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events
                .Select(calendarEvent => calendarEvent.Id)
                .Select(calendarEvent => calendarEvent.Body)
                .Select(calendarEvent => calendarEvent.Start)
                .Select(calendarEvent => calendarEvent.Subject)
                .Select(calendarEvent => calendarEvent.ResponseStatus)
                .Select(calendarEvent => calendarEvent.WebLink)
                .GetValues().GetAwaiter().GetResult(); //// TODO make this async

            Assert.AreEqual("/me/calendar/events?$select=id,body,start,subject,responseStatus,webLink", graphClient.CalledUri);
        }

        [TestMethod]
        public void NestedSelect()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events
                .Select(calendarEvent => calendarEvent.Body.Content)
                .GetValues().GetAwaiter().GetResult(); //// TODO make this async

            Assert.AreEqual("/me/calendar/events?$select=body/content", graphClient.CalledUri);
        }

        [TestMethod]
        public void Top()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events
                .Top(5)
                .GetValues().GetAwaiter().GetResult(); //// TODO make this async

            Assert.AreEqual("/me/calendar/events?$top=5", graphClient.CalledUri);
        }

        [TestMethod]
        public void OrderBy()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events
                .OrderBy(calendarEvent => calendarEvent.Start.DateTime)
                .GetValues().GetAwaiter().GetResult(); //// TODO make this async

            Assert.AreEqual("/me/calendar/events?$orderby=start/dateTime", graphClient.CalledUri);
        }

        [TestMethod]
        public void Filter()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events
                .Filter(calendarEvent => calendarEvent.Type == "singleInstance")//// && calendarEvent.Start.DateTime > startTime && calendarEvent.IsCancelled == false)
                .GetValues().GetAwaiter().GetResult(); //// TODO make this async

            Assert.AreEqual("/me/calendar/events?$filter=type eq 'singleInstance'", graphClient.CalledUri);
        }

        [TestMethod]
        public void FluentFilter()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events
                .Filter(calendarEvent => calendarEvent.Type == "singleInstance")
                .Filter(calendarEvent => calendarEvent.IsCancelled == false)
                .GetValues().GetAwaiter().GetResult(); //// TODO make this async

            Assert.AreEqual("/me/calendar/events?$filter=type eq 'singleInstance' and isCancelled eq false", graphClient.CalledUri);
        }

        [TestMethod]
        public void FilterWithDateTimeClosure()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var startTime = DateTime.Parse("2024-09-03");
            var events = graphCalendarContext.Events
                .Filter(calendarEvent => calendarEvent.Start.DateTime > startTime)
                .GetValues().GetAwaiter().GetResult(); //// TODO make this async

            Assert.AreEqual("/me/calendar/events?$filter=start/dateTime gt '2024-09-03T12:00:00.000000'", graphClient.CalledUri);
        }

        [TestMethod]
        public void FilterWithDateTimeParse()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events
                .Filter(calendarEvent => calendarEvent.Start.DateTime > DateTime.Parse("2024-09-03"))
                .GetValues().GetAwaiter().GetResult(); //// TODO make this async

            Assert.AreEqual("/me/calendar/events?$filter=start/dateTime gt '2024-09-03'", graphClient.CalledUri);
        }
    }

    [TestClass]
    public sealed class MigrationTestsCalendarContextUnitTests
    {
        private sealed class MockGraphClient : IGraphClient
        {
            public List<string> CalledUris { get; } = new List<string>();

            public async Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri)
            {
                this.CalledUris.Add(relativeUri.OriginalString);

                var content =
"""
{
    "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#users('some_user')/calendar/events",
    "value": [
        {
            "id": "some_id",
            "subject": "testing",
            "body": {
                "contentType": "html",
                "content": "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body><p>some data</p></body></html>"
            },
            "start": {
                "dateTime": "2024-05-17T23:30:00.0000000",
                "timeZone": "UTC"
            },
            "end": {
                "dateTime": "2024-05-17T23:30:00.0000000",
                "timeZone": "UTC"
            },
            "responseStatus": {
                "response": "asdf",
                "time": "Asdf"
            },
            "webLink": "somelinke",
            "type": "seriesMaster",
            "isCancelled": false
        }
    ]
}
""";

                var httpContent = new StringContent(content);
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = httpContent;
                return await Task.FromResult(responseMessage);
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
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var calendarContext = new CalendarContext(graphCalendarContext, DateTime.Parse("2024-09-03"), DateTime.Parse("2024-09-30"));
            var events = calendarContext.Events.ToArray();

            CollectionAssert.AreEqual(
                new[]
                {
                    "/me/calendar/events?$select=id,body,start,end,subject,responseStatus,webLink,type,isCancelled&$filter=type eq 'singleInstance' and start/dateTime gt '2024-09-03T07:00:00.000000' and start/dateTime lt '2024-09-30T07:00:00.000000'&$top=50",
                    "/me/calendar/events?$select=id,body,start,end,subject,responseStatus,webLink,type,isCancelled&$filter=type eq 'seriesMaster'&$top=50",
                    "/me/calendar/events/some_id/instances?startDateTime=9/3/2024 12:00:00 AM&endDateTime=9/30/2024 12:00:00 AM&$select=id,body,start,end,subject,responseStatus,webLink,type,isCancelled&$top=1",
                },
                graphClient.CalledUris);
        }

        [TestMethod]
        public void Test2()
        {
            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var calendarContext = new CalendarContext(graphCalendarContext, DateTime.Parse("2024-09-03"), DateTime.Parse("2024-09-30"));
            var uncanceledEvents = calendarContext
                .Events
                .Where(calendarEvent => calendarEvent.IsCancelled == false)
                .ToArray();

            CollectionAssert.AreEqual(
                new[]
                {
                    "/me/calendar/events?$select=id,body,start,end,subject,responseStatus,webLink,type,isCancelled&$filter=type eq 'singleInstance' and start/dateTime gt '2024-09-03T07:00:00.000000' and start/dateTime lt '2024-09-30T07:00:00.000000' and isCancelled eq false&$top=50",
                    "/me/calendar/events?$select=id,body,start,end,subject,responseStatus,webLink,type,isCancelled&$filter=type eq 'seriesMaster' and isCancelled eq false&$top=50",
                    "/me/calendar/events/some_id/instances?startDateTime=9/3/2024 12:00:00 AM&endDateTime=9/30/2024 12:00:00 AM&$select=id,body,start,end,subject,responseStatus,webLink,type,isCancelled&$filter=isCancelled eq false&$top=1",
                },
                graphClient.CalledUris);
        }
    }

#pragma warning restore CS8602 // Dereference of a possibly null reference.

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
            //// TODO topic 1
            //// i think what i should do is have an adapter from linq AST (expression) to odata AST, and then have something that converts an odata AST to a string
            //// this will need to have an extension point for the "property path" traversal
            //// this will also need to have extension points for things that aren't supported; for example, we want to have a way to allow datetime.parse be converted into the odata '{the_datetime}'; we should allow the caller to specify *additional* things like this that they want to support by convention

            //// conslusions:
            //// yes, use ASTs
            //// also use a visitor for the external extensibility; these are actually "hooks" and have clear hook points (the places where you throw the exceptions in the esles branches)
            //// each context should have a different confiurable visitor for each query parameter

            //// TODO topic 2
            //// i think i should force the use of closures at the moment for an convention-based convenience, and then extend this later (meaning, get rid of datetime.parse and force someone to set a local variable with the parsed datetime)
            //// maybe keep the datetime.parse code just so you can remember where it goes and how it looks?
            //// i should also add support for non-local variable closures before adding support for datetime.parse kind of things

            //// conclusions:
            //// i should not implement any convenience methods, but instead should have *only* the extensibility mechanism; a third party can release a set of "useful" or "common" extensions, but it should not be the odata parser
            //// for completeness, go ahead and implement static closures too

            //// TODO topic 3
            //// what does the code look like that would accept or reject a specific filter?

            /*
            var url =
                $"/me/calendar/events?" +
                $"$select=body,start,subject,responseStatus,webLink&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss.000000")}' and isCancelled eq false";
            */


            //// TODO use an anonymous type to do multiple selects in one method call?

            var graphClient = new MockGraphClient();
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri("/me/calendar", UriKind.Relative).ToRelativeUri());
            var events = graphCalendarContext.Events;

            var startTime = DateTime.Now.ToUniversalTime();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            events = events
                .Select(calendarEvent => calendarEvent.Id)
                .Select(calendarEvent => calendarEvent.Body)
                .Select(calendarEvent => calendarEvent.Start)
                .Select(calendarEvent => calendarEvent.Subject)
                .Select(calendarEvent => calendarEvent.ResponseStatus)
                .Select(calendarEvent => calendarEvent.WebLink)
                .Select(calendarEvent => calendarEvent.Body.Content)
                .Top(5)
                .OrderBy(calendarEvent => calendarEvent.Start.DateTime)
                .Filter(calendarEvent => calendarEvent.Type == "singleInstance" && calendarEvent.Start.DateTime > startTime && calendarEvent.IsCancelled == false)
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                ;
            var values = events.GetValues().GetAwaiter().GetResult(); //// TODO make async
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
