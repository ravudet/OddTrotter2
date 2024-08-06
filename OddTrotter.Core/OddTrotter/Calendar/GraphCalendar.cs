namespace OddTrotter.Calendar
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.V2;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using OddTrotter.AzureBlobClient;
    using OddTrotter.GraphClient;
    using OddTrotter.TodoList;

    public class GraphCalendarEvent
    {
        public GraphCalendarEvent(string id)
        {
            this.Id = id;   
        }

        public string Id { get; }

        //// TODO add *all* of the properties here
    }

    public sealed class GraphCalendar : IV2Enumerable<GraphCalendarEvent>
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri calendarUri;

        public GraphCalendar(IGraphClient graphClient, string userId, string calendarId)
        {
            this.graphClient = graphClient;
            this.calendarUri = new Uri($"/users/{userId}/calendars/{calendarId}/events", UriKind.Relative).ToRelativeUri();
        }

        public IEnumerator<GraphCalendarEvent> GetEnumerator()
        {
            using (var httpResponse = this.graphClient.GetAsync(this.calendarUri).ConfigureAwait(false).GetAwaiter().GetResult())
            {

            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private async IAsyncEnumerable<GraphCalendarEvent> GetInstanceEvents(RelativeUri calendarUri)
        {
            var instanceEventsUri = new Uri(calendarUri, "?$filter=type eq 'singleInstance'").ToRelativeUri();
            await foreach (var graphCalendarEvent in GetCollection<GraphCalendarEvent>(instanceEventsUri).ConfigureAwait(false))
            {
                yield return graphCalendarEvent;
            }
        }

        private async IAsyncEnumerable<T> GetCollection<T>(RelativeUri collectionUri)
        {
            var page = await GetFirstPage<T>(collectionUri).ConfigureAwait(false); //// TODO handle exceptions
            foreach (var element in page.Value!) //// TODO nullable
            {
                yield return element;
            }

            while (page.NextLink != null)
            {
                var nextLinkUri = new Uri(page.NextLink, UriKind.Absolute).ToAbsoluteUri(); //// TODO handle exceptions
                page = await GetSubsequentPage<T>(nextLinkUri).ConfigureAwait(false); //// TODO handle exceptions
                foreach (var element in page.Value!) //// TODO nullable
                {
                    yield return element;
                }
            }
        }

        private async Task<ODataCollectionPage<T>> GetFirstPage<T>(RelativeUri collectionUri)
        {
            using (var httpResponseMessage = await this.graphClient.GetAsync(collectionUri).ConfigureAwait(false))
            {
                return await ReadPage<T>(httpResponseMessage).ConfigureAwait(false);
            }
        }

        private async Task<ODataCollectionPage<T>> GetSubsequentPage<T>(AbsoluteUri pageUri)
        {
            using (var httpResponseMessage = await this.graphClient.GetAsync(pageUri).ConfigureAwait(false))
            {
                return await ReadPage<T>(httpResponseMessage).ConfigureAwait(false);
            }
        }

        private async Task<ODataCollectionPage<T>> ReadPage<T>(HttpResponseMessage httpResponseMessage)
        {
            var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new GraphException(httpResponseContent, e);
            }

            var odataCollectionPage = JsonSerializer.Deserialize<ODataCollectionPage<T>>(httpResponseContent);
            if (odataCollectionPage == null)
            {
                throw new JsonException($"Deserialized value was null. Serialized value was '{httpResponseContent}'");
            }

            if (odataCollectionPage.Value == null)
            {
                throw new JsonException($"The value of the collection JSON property was null. The serialized value was '{httpResponseContent}'");
            }

            return odataCollectionPage;
        }

        private sealed class ODataCollectionPage<T>
        {
            [JsonPropertyName("value")]
            public IEnumerable<T>? Value { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string? NextLink { get; set; }
        }
    }
}
