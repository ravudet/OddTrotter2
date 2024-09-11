namespace OddTrotter.TodoList
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.V2;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Fx.OdataPocRoot.GraphClient;
    using Fx.OdataPocRoot.Odata;
    using Microsoft.Extensions.Caching.Memory;
    using OddTrotter.AzureBlobClient;
    using OddTrotter.Calendar;
    using OddTrotter.GraphClient;

    public sealed class TodoListService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IGraphClient graphClient;

        private readonly IAzureBlobClient azureBlobClient;

        private readonly string todoListDataBlobName;

        private readonly int calendarEventPageSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="azureBlobClient"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="memoryCache"/> or <paramref name="graphClient"/> or <paramref name="azureBlobClient"/> is <see langword="null"/>
        /// </exception>
        public TodoListService(IMemoryCache memoryCache, IGraphClient graphClient, IAzureBlobClient azureBlobClient)
            : this(memoryCache, graphClient, azureBlobClient, new TodoListServiceSettings.Builder().Build())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="azureBlobClient"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="memoryCache"/> or <paramref name="graphClient"/> or <paramref name="azureBlobClient"/> or <paramref name="settings"/> is
        /// <see langword="null"/>
        /// </exception>
        public TodoListService(IMemoryCache memoryCache, IGraphClient graphClient, IAzureBlobClient azureBlobClient, TodoListServiceSettings settings)
        {
            if (memoryCache == null)
            {
                throw new ArgumentNullException(nameof(memoryCache));
            }

            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            if (azureBlobClient == null)
            {
                throw new ArgumentNullException(nameof(azureBlobClient));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.memoryCache = memoryCache;
            this.graphClient = graphClient;
            this.azureBlobClient = azureBlobClient;
            this.todoListDataBlobName = settings.TodoListDataBlobName;
            this.calendarEventPageSize = settings.CalendarEventPageSize;
        }

        private sealed class OddTrotterTodoListData
        {
            private OddTrotterTodoListData(DateTime lastRecordedEventTimeStamp)
            {
                //// TODO version this
                this.LastRecordedEventTimeStamp = lastRecordedEventTimeStamp;
            }

            public DateTime LastRecordedEventTimeStamp { get; }

            public sealed class Builder
            {
                [JsonPropertyName("lastRecordedEventTimeStamp")]
                public DateTime? LastRecordedEventTimeStamp { get; set; }

                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                /// <exception cref="ArgumentNullException">Thrown if <see cref="LastRecordedEventTimeStamp"/> is <see langword="null"/></exception>
                public OddTrotterTodoListData Build()
                {
                    if (this.LastRecordedEventTimeStamp == null)
                    {
                        throw new ArgumentNullException(nameof(this.LastRecordedEventTimeStamp));
                    }

                    return new OddTrotterTodoListData(this.LastRecordedEventTimeStamp.Value);
                }
            }
        }

        private static OddTrotterTodoListData.Builder SetDefaults(OddTrotterTodoListData.Builder? builder)
        {
            if (builder == null)
            {
                builder = new OddTrotterTodoListData.Builder();
            }

            if (builder.LastRecordedEventTimeStamp == null)
            {
                builder.LastRecordedEventTimeStamp = DateTime.MinValue;
            }

            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidBlobNameException">
        /// Thrown if the configured blob name to use for the todo list blob results in an invalid URL or points to a blob that cannot be read
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
        /// </exception>
        /// <exception cref="InvalidSasTokenException">Thrown if the configured azure blob container SAS token is not a valid SAS token</exception>
        /// <exception cref="SasTokenNoReadPrivilegesException">
        /// Thrown if the configured azure blob container SAS token does not have read permissions for the todo list blob
        /// </exception>
        /// <exception cref="AzureStorageException">Thrown if azure storage produced an error while reading the blob contents or writing back the blob content</exception>
        /// <exception cref="MalformedBlobDataException">Thrown if the contents of the todo list blob were not in the expected format</exception>
        /// <exception cref="InvalidAccessTokenException">
        /// Thrown if the access token configured on the provided <see cref="IGraphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        /// <exception cref="SasTokenNoWritePrivilegesException">
        /// Thrown if the configured azure blob container SAS token does not have write or create permissions for the todo list blob
        /// </exception>
        /// <exception cref="InvalidBlobDataException">Thrown if the todo list data could not be written to the blob</exception>
        public async Task<TodoListResult> RetrieveTodoList()
        {
            //// TODO this throws a lot of exceptions potentially, so you should just use a different implementation and then handle the exceptions *that* implementation throws
            var result = await this
                .memoryCache
                .GetOrCreateAsync("todolist", async entry => await RetrieveTodoListImpl().ConfigureAwait(false))
                .ConfigureAwait(false);
            //// TODO because this.memoryCache is scoped outside of this instance, it *may* actually have null entries; we should create a new IMemoryCache interface that guarantees no null values
#pragma warning disable CS8603 // Possible null reference return.
            return result;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public static Func<DateTime> Now { get; set; } = () => DateTime.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidBlobNameException">
        /// Thrown if the configured blob name to use for the todo list blob results in an invalid URL or points to a blob that cannot be read
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
        /// </exception>
        /// <exception cref="InvalidSasTokenException">Thrown if the configured azure blob container SAS token is not a valid SAS token</exception>
        /// <exception cref="SasTokenNoReadPrivilegesException">
        /// Thrown if the configured azure blob container SAS token does not have read permissions for the todo list blob
        /// </exception>
        /// <exception cref="AzureStorageException">Thrown if azure storage produced an error while reading the blob contents or writing back the blob content</exception>
        /// <exception cref="MalformedBlobDataException">Thrown if the contents of the todo list blob were not in the expected format</exception>
        /// <exception cref="InvalidAccessTokenException">
        /// Thrown if the access token configured on the provided <see cref="IGraphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        /// <exception cref="SasTokenNoWritePrivilegesException">
        /// Thrown if the configured azure blob container SAS token does not have write or create permissions for the todo list blob
        /// </exception>
        /// <exception cref="InvalidBlobDataException">Thrown if the todo list data could not be written to the blob</exception>
        private async Task<TodoListResult> RetrieveTodoListImpl()
        {
            OddTrotterTodoListData oddTrotterTodoList;
            using (var todoListDataBlobResponse = await this.azureBlobClient.GetAsync(this.todoListDataBlobName).ConfigureAwait(false))
            {
                OddTrotterTodoListData.Builder? oddTrotterTodoListDataBuilder;
                if (todoListDataBlobResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    oddTrotterTodoListDataBuilder = new OddTrotterTodoListData.Builder();
                }
                else
                {
                    var todoListBlobResponseContent = await todoListDataBlobResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        todoListDataBlobResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        throw new AzureStorageException(todoListBlobResponseContent, e);
                    }

                    try
                    {
                        oddTrotterTodoListDataBuilder = JsonSerializer.Deserialize<OddTrotterTodoListData.Builder>(todoListBlobResponseContent);
                    }
                    catch (JsonException e)
                    {
                        throw new MalformedBlobDataException(todoListBlobResponseContent, e);
                    }
                }

                oddTrotterTodoList = SetDefaults(oddTrotterTodoListDataBuilder).Build();
            }

            var instanceEventsCollection = GetEvents(this.graphClient, oddTrotterTodoList.LastRecordedEventTimeStamp, Now(), this.calendarEventPageSize);
            var instanceEvents = instanceEventsCollection.Elements.ToV2Enumerable();
            var todoListEvents = instanceEvents
                .Where(instanceEvent => instanceEvent.Subject?.Contains("todo list", StringComparison.OrdinalIgnoreCase) == true);
            var instanceEventsAggregatedWithoutStarts = todoListEvents
                .ApplyAggregation(Enumerable.Empty<CalendarEvent>(), (withoutStarts, @event) => @event.Start?.DateTime == null ? withoutStarts.Append(@event) : withoutStarts);
            var todoListEventsWithStarts = instanceEventsAggregatedWithoutStarts
                .Where(@event => @event.Start?.DateTime != null);
            var todoListEventsWithPotentiallyParsedStarts = todoListEventsWithStarts
                .Select(
                    instanceEvent => PossibleError.FromThrowable(
                        instanceEvent,
                        @event => DateTime.Parse(
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
                            @event.Start.DateTime)));
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var todoListEventsAggregatedStartParseFailures = todoListEventsWithPotentiallyParsedStarts
                .ApplyAggregation(
                    Enumerable.Empty<(CalendarEvent, Exception)>(),
                    (failures, tuple) => tuple.Item2.IsError ? failures.Append((tuple.Item1, tuple.Item2.Error)) : failures);
            var todoListEventsTuplesWithParsedStarts = todoListEventsAggregatedStartParseFailures
                .Where(tuple => !tuple.Item2.IsError)
                .Select(tuple => (tuple.Item1, tuple.Item2.Value))
                .Where(tuple => tuple.Value > oddTrotterTodoList.LastRecordedEventTimeStamp); //// TODO there's a bug in the graph api; it treats gt as ge
            var todoListEventsAggregatedLastSeenTimestamp = todoListEventsTuplesWithParsedStarts
                .ApplyAggregation((DateTime?)null, (lastSeenTimeStamp, tuple) => Comparer<DateTime?>.Default.Max(lastSeenTimeStamp, tuple.Value));
            var todoListEventsWithParsedStarts = todoListEventsAggregatedLastSeenTimestamp
                .Select(tuple => tuple.Item1);
            var todoListEventsAggregatedWithoutBodies = todoListEventsWithParsedStarts
                .ApplyAggregation(Enumerable.Empty<CalendarEvent>(), (withoutBodies, @event) => @event.Body?.Content == null ? withoutBodies.Append(@event) : withoutBodies);
            var todoListEventsWithBodies = todoListEventsAggregatedWithoutBodies
                .Where(@event => @event.Body?.Content != null);
            var todoListEventsWithPotentiallyParsedBodies = todoListEventsWithBodies
                .Select(
                    instanceEvent => PossibleError.FromThrowable( //// TODO generalize this fromthrowable thing? it would prevent you from re-computing the nullable stuff above
                        instanceEvent,
                        @event => ParseEventBody(
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
                            @event.Body.Content)));
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var todoListEventsAggregatedBodyParseFailures = todoListEventsWithPotentiallyParsedBodies
                .ApplyAggregation(
                    Enumerable.Empty<(CalendarEvent, Exception)>(),
                    (failures, tuple) => tuple.Item2.IsError ? failures.Append((tuple.Item1, tuple.Item2.Error)) : failures);
            var todoListEventsTuplesWithParsedBodies = todoListEventsAggregatedBodyParseFailures
                .Where(tuple => !tuple.Item2.IsError)
                .Select(tuple => (tuple.Item1, tuple.Item2.Value));
            var todoListEventsWithParsedBodies = todoListEventsTuplesWithParsedBodies
                .SelectMany(tuple => tuple.Value);

            var newData = string.Join(Environment.NewLine, todoListEventsWithParsedBodies);

            var newOddTrotterTodoList = new OddTrotterTodoListData.Builder()
            {
                LastRecordedEventTimeStamp = todoListEventsAggregatedLastSeenTimestamp.Aggregation ?? oddTrotterTodoList.LastRecordedEventTimeStamp,
            };
            using (var blobRequestContent = new StringContent(JsonSerializer.Serialize(newOddTrotterTodoList)))
            {
                // we let all exceptions and errors from here on prevent returning the todo list because subsequent requests for the todo list will not have the latest timestamp, meaning that duplicate data may be presented to the caller in those cases; it's a shame that we did all this work for nothing, but it's better to have data consistency even if it means we waste some CPU cycles, especially considering that the caller likely doesn't have a way to consistently recover from these errors for themselves
                using (var blobUploadResponse = await this.azureBlobClient.PutAsync(this.todoListDataBlobName, blobRequestContent).ConfigureAwait(false))
                {
                    try
                    {
                        blobUploadResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        var blobUploadResponseContent = await blobUploadResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new AzureStorageException(blobUploadResponseContent, e);
                    }
                }
            }

            return new TodoListResult(
                newData,
                instanceEventsCollection.LastRequestedPageUrl,
                instanceEventsAggregatedWithoutStarts.Aggregation,
                todoListEventsAggregatedStartParseFailures.Aggregation,
                todoListEventsAggregatedWithoutBodies.Aggregation,
                todoListEventsAggregatedBodyParseFailures.Aggregation);
        }

        /// <summary>
        /// TODO you have avoided documenting the exceptions here by using PossibleError.FromThrowable; if you ever call this from somewhere else, you need to document the
        /// exceptions
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private static IEnumerable<string> ParseEventBody(string body)
        {
            //// TODO do you need to document anything here?
            body = body.Replace("&nbsp;", string.Empty);

            // the calendar api returns html bodies that are malformed xml; the head element contains a meta element that doesn't close
            var bodyElement = "<body>";
            var bodyCloseElement = "</body>";
            body = $"<html>{body.Substring(0, body.IndexOf(bodyCloseElement)).Substring(body.IndexOf(bodyElement) + bodyElement.Length)}</html>";

            var document = XDocument.Parse(body);
            var links = document.Descendants("a").Reverse();
            foreach (var link in links)
            {
                link.ReplaceWith(link.Value);
            }

            return document.Descendants("p").Select(element => element.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="InvalidAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        /// <remarks>
        /// The returned <see cref="ODataCollection{CalendarEvent}"/> will have its <see cref="ODataCollection{T}.LastRequestedPageUrl"/> be one of three values:
        /// 1. <see langword="null"/> if no errors occurred retrieve any of the data
        /// 2. The URL of series master entity for which an error occurred while retrieving the instance events
        /// 3. The URL of the nextLink for which an error occurred while retrieving the that URL's page
        /// </remarks>
        private static ODataCollection<CalendarEvent> GetEvents(IGraphClient graphClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri($"/me/calendar", UriKind.Relative).ToRelativeUri());
            var calendarContext = new CalendarContext(graphCalendarContext, startTime, endTime); //// TODO constructor injection
            var query = calendarContext
                .Events
                .OrderBy(calendarEvent => calendarEvent.Start.DateTime)
                .Where(calendarEvent => calendarEvent.IsCancelled == false);
            var queried = query.ToArray(); //// TODO is there an async queryable?

            return new ODataCollection<CalendarEvent>(
                queried.Select(_ => new CalendarEvent() 
                {  
                    Body = new BodyStructure() 
                    { 
                        Content = _.Body?.Content 
                    }, 
                    Id = _.Id, 
                    ResponseStatus = new ResponseStatusStructure()
                    {
                        Response = _.ResponseStatus?.Response,
                        Time = _.ResponseStatus?.Time,
                    },
                    Start = new TimeStructure()
                    {
                        DateTime = _.Start?.DateTime.ToString(),
                        TimeZone = _.Start?.TimeZone,
                    },
                    Subject = _.Subject, 
                    WebLink = _.WebLink ,
                }),
                null); //// TODO get the last request page uri
        }

        private sealed class ODataCollection<T>
        {
            public ODataCollection(IEnumerable<T> elements)
                : this(elements, null)
            {
            }

            public ODataCollection(IEnumerable<T> elements, string? lastRequestedPageUrl)
            {
                this.Elements = elements;
                this.LastRequestedPageUrl = lastRequestedPageUrl;
            }

            public IEnumerable<T> Elements { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// <see langword="null"/> if there are collection was read to completeness; has a value if reading a next link produced an error, where the value is the nextlink that
            /// produced the error
            /// </remarks>
            public string? LastRequestedPageUrl { get; }
        }

        public static class PossibleError
        {
            public static PossibleError<TValue, TError>.ConcreteValue Value<TValue, TError>(TValue value)
            {
                return new PossibleError<TValue, TError>.ConcreteValue(value);
            }

            public static PossibleError<TValue, TError>.ConcreteError Error<TValue, TError>(TError error)
            {
                return new PossibleError<TValue, TError>.ConcreteError(error);
            }

            public static (TSource, PossibleError<TTarget, Exception>) FromThrowable<TSource, TTarget>(TSource source, Func<TSource, TTarget> func)
            {
                try
                {
                    return (source, PossibleError.Value<TTarget, Exception>(func(source)));
                }
                catch (Exception e)
                {
                    return (source, PossibleError.Error<TTarget, Exception>(e));
                }
            }
        }

        public abstract class PossibleError<TValue, TError>
        {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            private PossibleError()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            {
            }

            public abstract bool IsError { get; } //// TODO i don't think you need inheritance here

            public TValue Value { get; protected set; }

            public TError Error { get; protected set; }

            public sealed class ConcreteValue : PossibleError<TValue, TError>
            {
                public ConcreteValue(TValue value)
                {
                    base.Value = value;
                }

                public override bool IsError => false;
            }

            public sealed class ConcreteError : PossibleError<TValue, TError>
            {
                public ConcreteError(TError error)
                {
                    base.Error = error;
                }

                public override bool IsError => true;
            }
        }
    }
}