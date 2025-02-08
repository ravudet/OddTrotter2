﻿namespace OddTrotter.TodoList
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.V2;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Sources;
    using System.Xml.Linq;

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
            : this(memoryCache, graphClient, azureBlobClient, TodoListServiceSettings.Default)
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
                /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="LastRecordedEventTimeStamp"/> is not UTC</exception>
                public OddTrotterTodoListData Build()
                {
                    if (this.LastRecordedEventTimeStamp == null)
                    {
                        throw new ArgumentNullException(nameof(this.LastRecordedEventTimeStamp));
                    }

                    if (this.LastRecordedEventTimeStamp?.Kind != DateTimeKind.Utc)
                    {
                        throw new ArgumentOutOfRangeException($"The 'Kind' of '{nameof(this.LastRecordedEventTimeStamp)}' must be '{nameof(DateTimeKind.Utc)}'");
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

            if (builder.LastRecordedEventTimeStamp == null || builder.LastRecordedEventTimeStamp?.Kind != DateTimeKind.Utc)
            {
                builder.LastRecordedEventTimeStamp = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
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
        /// <exception cref="UnauthorizedAccessTokenException">
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

        /// <summary>
        /// TODO is this class a good idea?
        /// </summary>
        private sealed class GraphClientToOdataClient : IHttpClient
        {
            private readonly IGraphClient graphClient;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="graphClient"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphClient"/> is <see langword="null"/></exception>
            public GraphClientToOdataClient(IGraphClient graphClient)
            {
                if (graphClient == null)
                {
                    throw new ArgumentNullException(nameof(graphClient));
                }

                this.graphClient = graphClient;
            }

            /// <inheritdoc/>
            public async Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri, IEnumerable<HttpHeader> headers)
            {
                //// TODO honor the headers if you still end up using this class
                return await this.graphClient.GetAsync(absoluteUri).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryResult"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="queryResult"/> is <see langword="null"/></exception>
        private static TodoListResultBuilder Convert(QueryResult<Either<Calendar.CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException> queryResult, DateTime lastRecordedEventTimeStamp)
        {
            ArgumentNullException.ThrowIfNull(queryResult);

            var builder = new TodoListResultBuilder(lastRecordedEventTimeStamp);

            while (queryResult is QueryResult<Either<Calendar.CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException>.Element element)
            {
                element.Value.Visit(
                    (left, context) =>
                    {
                        if (left.Value.Start < context.EndTimestamp)
                        {
                            context.EndTimestamp = left.Value.Start.DateTime; //// TODO why did you choose datetimeoffset some places and datetime others?
                        }

                        IEnumerable<string> parsedBody;
                        try
                        {
                            parsedBody = ParseEventBody(left.Value.Body);
                        }
                        catch (Exception exception)
                        {
                            context.BodyParseErrors.Add(exception);
                            return new Nothing();
                        }

                        context.TodoList.AppendJoin(Environment.NewLine, parsedBody).AppendLine();
                        return new Nothing();
                    },
                    (right, context) =>
                    {
                        //// TODO do you want to stop recording the endtimestamp if a translation error occurred, or should the user be expected to handle it at that point? if the user is expected to handle it, it'd probably be good to put the errors in a more permanent storage so that a browser window mishap doesn't cause data loss
                        //// TODO i think you should let the user handle it because otherwise a calendar error will get them permanently stuck at a certain timestamp and they will need to actually go to the calendar event and fix it, instead of just checking that the error can be skipped and ignoring it, letting the next refresh remove it; you *will* want a way to persist the errors though for the browser mishap reason
                        context.TranslationErrors.Add(right.Value);
                        return new Nothing();
                    },
                    builder);

                queryResult = element.Next();
            }

            if (queryResult is QueryResult<Either<Calendar.CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException>.Partial partial)
            {
                builder.PagingError = partial.Error;
            }

            return builder;
        }

        private sealed class TodoListResultBuilder
        {
            public TodoListResultBuilder(DateTime lastRecordedEventTimeStamp)
            {
                this.TodoList = new StringBuilder();
                this.TranslationErrors = new List<CalendarEventsContextTranslationException>();
                this.BodyParseErrors = new List<Exception>();
                this.EndTimestamp = lastRecordedEventTimeStamp;
            }

            public StringBuilder TodoList { get; set; }

            public DateTime EndTimestamp { get; set; }

            public List<CalendarEventsContextTranslationException> TranslationErrors { get; set; }

            public List<Exception> BodyParseErrors { get; set; } //// TODO use the right tpye of elements

            public CalendarEventsContextPagingException? PagingError { get; set; }
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
        /// TODO handle all exceptions from here onward to the caller
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// <exception cref="UnauthorizedAccessTokenException">
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

            var originalLastRecordedEventTimeStamp = oddTrotterTodoList.LastRecordedEventTimeStamp;



            // TODO write up code quality list and create blog posts
            // code quality:
            // separate files
            // comment line at the top of the file
            // namespaces
            // `using` statements
            // dead code
            // line lengths
            // precondition checks
            //   `argumentnullexception.throwifnull`
            //   `argumentnullinline.throwifnull`
            // exception documentation
            // todos
            // `sealed` (classes and overriden methods)
            // anything that isn't nailed down 100% should be `internal`
            // `this.`
            // give caught exceptions good variable names
            // `using` blocks
            // scope suppressions as narrowly as possible
            // unit tests
            // do everything above for unit tests

            // temporary code quality (you should really do better than the below, but this is for convenience at this point):
            // at least put the word "placeholder" in the xmldoc summary of methods that you've done exception docs for

            //// TODO eitherunittests.applyleftexception is an exception where a `throw` type might be useful; are there use-cases that aren't testing though?

            //// TODO you are doing the "either" stuff right now because it is foundational to the query context stuff, and the query context stuff will need some diligence and effort around adding the third type parameter that allows mixins to work correctly; do query contexts next probably and just know that you're going to need to put the effort in
            //// TODO address todos TODO FUTURE, TODO TOPIC, and TODO TODO
            //// TODO separate files and namespaces, remove dead code, normalzie line lengths, stuff like that
            //// TODO do as much unit testing as you can
            //// TODO any remaining TODO TODOs + address TODO TOPICs
            //// TODO finish unit tests
            //// TODO make sure anything that is not completely accurate is marked as `internal`
            //// TODO do another pass (you will likely need at least one more pass after this)
            //// TODO you should be able to cast QueryResult<Either<CalendarEvent, GraphCalendarEvent>, IOException> to QueryResult<Either<CalendarEvent, GraphCalendarEvent>, Exception>
            //// TODO write tests for todolistservice that confirm the URLs
            var odataClient = new GraphClientToOdataClient(this.graphClient);
            var odataCalendarEventsContext = new OdataCalendarEventsContext(
                OdataServiceRoot.MicrosoftGraph, //// TODO get from this.graphclient
                odataClient);
            var graphOdataStructuredContext = new GraphOdataStructuredContext(odataCalendarEventsContext, "TODO get from this.graphclient");
            var graphCalendarEventsContext = new GraphCalendarEventsEvaluator(graphOdataStructuredContext);
            var calendarEventsContextSettings = new CalendarEventsContextSettings.Builder()
            {
                PageSize = this.calendarEventPageSize,
                //// TODO configure firstinstanceinserieslookahead
            }.Build();
            var calendarEventsContext = new CalendarEventsContext(
                graphCalendarEventsContext,
                new UriPath("/me/calendar"),
                originalLastRecordedEventTimeStamp,
                calendarEventsContextSettings);

            calendarEventsContext = calendarEventsContext
                .Where(CalendarEventsContext.StartLessThanNow)
                .Where(CalendarEventsContext.IsNotCancelled);
            //// TODO the paging exception will have the token exceptions in it; maybe the `page` method and `evaluate` should actually throw these for the first page, but not for the subsequent ones?
            var calendarEvents = await calendarEventsContext.Evaluate().ConfigureAwait(false);

            //// TODO this is actually pretty weird; you did a good job separating when the queries are in-memory vs client-based; but, in this todolistservice you actually want them all to be given to the calendareventscontext so that it can do in-memory filtering to prevent network calls when getting the series events; this further exacerbates the issue around queryresult<either, error> because in these in-memory ones, you really do want the either (and you want the caller to be able to tell us the behavior for both sides of the either) //// TODO is this last part about the either really the case? isn't it *actually* that the calendareventcontext knows that we should always surface errors, and as written we are putting the consistency burden on the todolistservice?
            var todoListEvents2 = calendarEvents
                .Where(calendarEvent => 
                    calendarEvent.Visit(
                        left => left.Subject.Contains("todo list", StringComparison.OrdinalIgnoreCase),
                        right => true))
                .Where(calendarEvent => //// TODO this should go in the "on the service" portion
                    calendarEvent
                        .Visit(
                            left => left.Start > originalLastRecordedEventTimeStamp, // there's a bug in the graph api; it treats gt as ge, so we need to do this extra check locally
                            right => true));

            var resultBuilder = Convert(todoListEvents2, originalLastRecordedEventTimeStamp);
            var result2 = new TodoListResult(
                resultBuilder.TodoList.ToString(),
                originalLastRecordedEventTimeStamp,
                resultBuilder.EndTimestamp,
                resultBuilder.PagingError?.ToString(),
                Enumerable.Empty<CalendarEvent>(),
                resultBuilder.TranslationErrors.Select(exception => (default(CalendarEvent)!, (Exception)exception)),
                Enumerable.Empty<CalendarEvent>(),
                resultBuilder.BodyParseErrors.Select(exception => (default(CalendarEvent)!, exception)));






            var instanceEventsCollection = GetEvents(this.graphClient, originalLastRecordedEventTimeStamp, DateTime.UtcNow, this.calendarEventPageSize);
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
                        @event => !string.Equals(@event?.Start?.TimeZone, "utc", StringComparison.OrdinalIgnoreCase) ? throw new InvalidOperationException("the event did not have a known time zone in its start time") :  DateTime.SpecifyKind(DateTime.Parse(
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                            @event
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
                                .Start
                                .DateTime),
                            DateTimeKind.Utc)));
            var todoListEventsAggregatedStartParseFailures = todoListEventsWithPotentiallyParsedStarts
                .ApplyAggregation(
                    Enumerable.Empty<(CalendarEvent, Exception)>(), 
                    (failures, tuple) => tuple.Item2.IsError ? failures.Append((tuple.Item1, tuple.Item2.Error)) : failures);
            var todoListEventsTuplesWithParsedStarts = todoListEventsAggregatedStartParseFailures
                .Where(tuple => !tuple.Item2.IsError)
                .Select(tuple => (tuple.Item1, tuple.Item2.Value))
                .Where(tuple => tuple.Value > oddTrotterTodoList.LastRecordedEventTimeStamp); //// TODO 
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








            var newOddTrotterTodoList2 = new OddTrotterTodoListData.Builder()
            {
                LastRecordedEventTimeStamp = result2.EndTimestamp,
            };







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
                originalLastRecordedEventTimeStamp,
                newOddTrotterTodoList.LastRecordedEventTimeStamp.Value,
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
        /// <exception cref="UnauthorizedAccessTokenException">
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
            var instanceEvents = GetInstanceEvents(graphClient, startTime, endTime, pageSize);
            var seriesEvents = GetSeriesEvents(graphClient, startTime, endTime, pageSize);
            //// TODO merge the sorted sequences instead of concat
            return new ODataCollection<CalendarEvent>(
                instanceEvents.Elements.Concat(seriesEvents.Elements), 
                instanceEvents.LastRequestedPageUrl ?? seriesEvents.LastRequestedPageUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        private static ODataCollection<CalendarEvent> GetInstanceEvents(IGraphClient graphClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            //// TODO starttime and endtime should be done through a queryable
            var url = GetInstanceEventsUrl(startTime, pageSize) + $" and start/dateTime lt '{endTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}'";
            return GetCollection<CalendarEvent>(graphClient, new Uri(url, UriKind.Relative).ToRelativeUri());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private static string GetInstanceEventsUrl(DateTime startTime, int pageSize)
        {
            //// TODO make the calendar that's used configurable?
            var url =
                $"/me/calendar/events?" +
                $"$select=body,start,subject&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}' and isCancelled eq false";
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        /// <remarks>
        /// The returned <see cref="ODataCollection{CalendarEvent}"/> will have its <see cref="ODataCollection{T}.LastRequestedPageUrl"/> be one of three values:
        /// 1. <see langword="null"/> if no errors occurred retrieve any of the data
        /// 2. The URL of series master entity for which an error occurred while retrieving the instance events
        /// 3. The URL of the nextLink for which an error occurred while retrieving the that URL's page
        /// </remarks>
        private static ODataCollection<CalendarEvent> GetSeriesEvents(IGraphClient graphClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            var seriesEventMasters = GetSeriesEventMasters(graphClient, pageSize);
            //// TODO you could actually filter by subject here before making further requests
            var seriesInstanceEvents = seriesEventMasters
                .Elements
                .Select(series => (series, GetFirstSeriesInstanceInRange(graphClient, series, startTime, endTime).ConfigureAwait(false).GetAwaiter().GetResult()))
                .ToV2Enumerable();
            var seriesInstanceEventsWithFailures = seriesInstanceEvents
                .ApplyAggregation((string?)null, (failedMasterId, tuple) => tuple.Item2 == null ? tuple.series.Id : null);
            var seriesInstanceEventsWithoutFailures = seriesInstanceEventsWithFailures
                .TakeWhile(tuple => tuple.Item2 != null) //// TODO if you can move this before the applyaggregation somehow, that would be ideal since we then wouldn't continue retrieve the first series instance if we already know we aren't returing anymore of them
                .Where(tuple => tuple.Item2?.Any() == true)
                .Select(tuple => new CalendarEvent() { Body = tuple.series.Body, Id = tuple.series.Id, Subject = tuple.series.Subject, Start = tuple.Item2?.First().Start });

            // because the second parameter of lastRequestedPageUrl is a fully realized value, we need to have actually enumerated the elements that we expect to return in the
            // first parameter; in this case, we have enumerated the elements because accessing seriesInstanceEventsWithFailures.Aggregation will enumerate enough events to
            // perform the aggregation; in a previous iteration of this method, the elements were enumerated with a .ToList() call
            return new ODataCollection<CalendarEvent>(
                seriesInstanceEventsWithoutFailures, 
                seriesInstanceEventsWithFailures.Aggregation == null ? 
                    seriesEventMasters.LastRequestedPageUrl : 
                    $"/me/calendar/events/{seriesInstanceEventsWithFailures.Aggregation}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        private static ODataCollection<CalendarEvent> GetSeriesEventMasters(IGraphClient graphClient, int pageSize)
        {
            //// TODO make the calendar that's used configurable?
            var url = $"/me/calendar/events?" + 
                $"$select=body,start,subject&" + 
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                "$filter=type eq 'seriesMaster' and isCancelled eq false";
            return GetCollection<CalendarEvent>(graphClient, new Uri(url, UriKind.Relative).ToRelativeUri());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="seriesMaster"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>The first instance of the <paramref name="seriesMaster"/> event, or <see langword="null"/> if an error occurred while retrieveing the first instance</returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        private static async Task<IEnumerable<CalendarEvent>?> GetFirstSeriesInstanceInRange(IGraphClient graphClient, CalendarEvent seriesMaster, DateTime startTime, DateTime endTime)
        {
            //// TODO this method would be much better as some sort of "TryGet" variant (though it does still have exceptions that it throws...), but being async, we can't have the needed out parameters
            var url = $"/me/calendar/events/{seriesMaster.Id}/instances?startDateTime={startTime}&endDateTime={endTime}&$top=1&$select=id,start,subject,body&$filter=isCancelled eq false";
            HttpResponseMessage? httpResponse = null;
            try
            {
                try
                {
                    httpResponse = await graphClient.GetAsync(new Uri(url, UriKind.Relative).ToRelativeUri()).ConfigureAwait(false);
                }
                catch (HttpRequestException)
                {
                    return null;
                }

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                ODataCollectionPage<CalendarEvent>.Builder? odataCollection;
                try
                {
                    odataCollection = JsonSerializer.Deserialize<ODataCollectionPage<CalendarEvent>.Builder>(httpResponseContent);
                }
                catch (JsonException)
                {
                    return null;
                }

                return odataCollection?.Value;
            }
            finally
            {
                httpResponse?.Dispose();
            }
        }

        /*private IV2Enumerable<CalendarEvent> GetPendingFutureCalendarEvents(HttpClient httpClient, DateTime startTime)
        {
            return this.GetFutureCalendarEvents(httpClient, startTime)
                //// TODO remove literals
                .Where(calendarEvent => !string.Equals(calendarEvent.ResponseStatus.Response, "accepted") && !string.Equals(calendarEvent.ResponseStatus.Response, "organizer") && !string.Equals(calendarEvent.ResponseStatus.Response, "tentativelyAccepted"));
        }

        private static IEnumerable<CalendarEvent> GetSingleInstanceEvents(HttpClient httpClient, DateTime startTime, int pageSize)
        {
            var url = $"https://graph.microsoft.com/v1.0/me/calendar/events?$select=subject,responseStatus,webLink&$top={pageSize}&$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToString("yyyy-MM-ddThh:mm:ss.000000")}' and isCancelled eq false";
            return GetCollection<CalendarEvent>(httpClient, url);
        }

        private static IV2Enumerable<CalendarEvent> GetSeriesEvents(HttpClient httpClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            return new GetSeriesEventsEnumerable(httpClient, startTime, endTime, pageSize);
        }

        private sealed class GetSeriesEventsEnumerable : IWhereEnumerable<CalendarEvent>
        {
            private readonly HttpClient httpClient;

            private readonly DateTime startTime;

            private readonly DateTime endTime;

            private readonly int pageSize;

            private readonly Func<CalendarEvent, bool>? predicate;

            public GetSeriesEventsEnumerable(HttpClient httpClient, DateTime startTime, DateTime endTime, int pageSize)
                : this(httpClient, startTime, endTime, pageSize, null)
            {
            }

            private GetSeriesEventsEnumerable(HttpClient httpClient, DateTime startTime, DateTime endTime, int pageSize, Func<CalendarEvent, bool>? predicate)
            {
                this.httpClient = httpClient;
                this.startTime = startTime;
                this.endTime = endTime;
                this.pageSize = pageSize;
                this.predicate = predicate;
            }

            public IEnumerator<CalendarEvent> GetEnumerator()
            {
                var seriesEvents = GetSeriesEvents(this.httpClient, this.pageSize);
                if (this.predicate != null)
                {
                    seriesEvents = seriesEvents.Where(this.predicate);
                }

                return seriesEvents
                    .Where(series => GetFirstSeriesInstanceInRange(httpClient, series, startTime, endTime).ConfigureAwait(false).GetAwaiter().GetResult().Any())
                    .GetEnumerator();
            }

            public IV2Enumerable<CalendarEvent> Where(Func<CalendarEvent, bool> predicate)
            {
                return new GetSeriesEventsEnumerable(this.httpClient, this.startTime, this.endTime, this.pageSize, this.predicate == null ? predicate : calendarEvent => this.predicate(calendarEvent) && predicate(calendarEvent));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private static async Task<IEnumerable<CalendarEvent>> GetFirstSeriesInstanceInRange(HttpClient httpClient, CalendarEvent seriesMaster, DateTime startTime, DateTime endTime)
        {
            var url = $"https://graph.microsoft.com/v1.0/me/calendar/events/{seriesMaster.Id}/instances?startDateTime={startTime}&endDateTime={endTime}&$top=1&$select=id&$filter=isCancelled eq false";
            using (var httpResponse = await httpClient.GetAsync(url).ConfigureAwait(false))
            {
                httpResponse.EnsureSuccessStatusCode();
                var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                var odataCollection = JsonSerializer.Deserialize<ODataCollection<CalendarEvent>>(httpResponseContent);
                return odataCollection.Value;
            }
        }

        private static IEnumerable<CalendarEvent> GetSeriesEvents(HttpClient httpClient, int pageSize)
        {
            var url = $"https://graph.microsoft.com/v1.0/me/calendar/events?$select=subject,responseStatus,webLink&$top={pageSize}&$filter=type eq 'seriesMaster' and isCancelled eq false";
            return GetCollection<CalendarEvent>(httpClient, url);
        }*/

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphClient"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        private static ODataCollection<T> GetCollection<T>(IGraphClient graphClient, RelativeUri uri)
        {
            //// TODO use linq instead of a list
            var elements = new List<T>();
            ODataCollectionPage<T> page;
            try
            {
                //// TODO would it make sense to have a method like "bool TryGetPage(IGraphClient, RelativeUri, out ODataCollectionPage, out ODataCollection)" ?
                page = GetPage<T>(graphClient, uri).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception e) when (e is HttpRequestException || e is GraphException || e is JsonException)
            {
                return new ODataCollection<T>(elements, uri.OriginalString);
            }

            elements.AddRange(page.Value);
            var nextLink = page.NextLink;
            while (nextLink != null)
            {
                AbsoluteUri nextLinkUri;
                try
                {
                    nextLinkUri = new Uri(nextLink, UriKind.Absolute).ToAbsoluteUri();
                }
                catch (UriFormatException)
                {
                    return new ODataCollection<T>(elements, nextLink);
                }

                try
                {
                    page = GetPage<T>(graphClient, nextLinkUri).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception e) when (e is HttpRequestException || e is GraphException || e is JsonException)
                {
                    return new ODataCollection<T>(elements, nextLinkUri.OriginalString);
                }

                elements.AddRange(page.Value);
                nextLink = page.NextLink;
            }

            return new ODataCollection<T>(elements);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphClient"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request
        /// </exception>
        /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
        /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
        private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, RelativeUri uri)
        {
            using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
            {
                return await ReadPage<T>(httpResponse);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphClient"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">
        /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
        /// </exception>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request
        /// </exception>
        /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
        /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
        private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, AbsoluteUri uri)
        {
            using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
            {
                return await ReadPage<T>(httpResponse);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
        /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
        private static async Task<ODataCollectionPage<T>> ReadPage<T>(HttpResponseMessage httpResponse)
        {
            var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new GraphException(httpResponseContent, e);
            }

            var odataCollectionPage = JsonSerializer.Deserialize<ODataCollectionPage<T>.Builder>(httpResponseContent);
            if (odataCollectionPage == null)
            {
                throw new JsonException($"Deserialized value was null. Serialized value was '{httpResponseContent}'");
            }

            if (odataCollectionPage.Value == null)
            {
                throw new JsonException($"The value of the collection JSON property was null. The serialized value was '{httpResponseContent}'");
            }

            return odataCollectionPage.Build();
        }
    }

    public sealed class ODataCollectionPage<T>
    {
        private ODataCollectionPage(IEnumerable<T> value, string? nextLink)
        {
            this.Value = value;
            this.NextLink = nextLink;
        }

        public IEnumerable<T> Value { get; }

        /// <summary>
        /// Gets the URL of the next page in the collection, <see langword="null"/> if there are no more pages in the collection
        /// </summary>
        public string? NextLink { get; }

        public sealed class Builder
        {
            [JsonPropertyName("value")]
            public IEnumerable<T>? Value { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string? NextLink { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <see cref="Value"/> is <see langword="null"/></exception>
            public ODataCollectionPage<T> Build()
            {
                if (this.Value == null)
                {
                    throw new ArgumentNullException(nameof(this.Value));
                }

                return new ODataCollectionPage<T>(this.Value, this.NextLink);
            }
        }
    }

    /*public interface IAggregatedEnumerable<TElement, TAggregate> : IEnumerable<TElement>
    {
        public TAggregate Aggregate { get; }
    }

    public interface IFurtherAggregatedEnumerable<TElement, TAggregate1, TAggregate2, TEnumerable> : IAggregatedEnumerable<TElement, TAggregate2> where TEnumerable : IAggregatedEnumerable<TElement, TAggregate1>
    {
        public TEnumerable Enumerable { get; }
    }

    public static class AggregationExtension
    {
        public static void DoWork()
        {
            var data = new[] { "asdf", "zxcvzxcv" };
            var aggregatedData = data
                .ApplyAggregate(0, (aggregate, element) => Math.Max(aggregate, element.Length))
                .ApplyAggregate2(string.Empty, (aggregate, element) => StringComparer.OrdinalIgnoreCase.Compare(aggregate, element) > 0 ? aggregate : element)
                .ApplyAggregate2('\0', (aggregate, element) => aggregate > element[0] ? aggregate : element[0]);
        }

        public static IAggregatedEnumerable<TElement, TAggregate> ApplyAggregate<TElement, TAggregate>(this IEnumerable<TElement> enumerable, TAggregate seed, Func<TAggregate, TElement, TAggregate> aggregator)
        {
            return new AggregatedEnumerable<TElement, TAggregate>(enumerable, seed, aggregator);
        }

        public static IFurtherAggregatedEnumerable<TElement, TAggregate1, TAggregate2, TEnumerable> ApplyAggregate2<TElement, TAggregate1, TAggregate2, TEnumerable>(this TEnumerable enumerable, TAggregate2 seed, Func<TAggregate2, TElement, TAggregate2> aggregator) where TEnumerable : IAggregatedEnumerable<TElement, TAggregate1>
        {
            return new FurtherAggregatedEnumerable<TElement, TAggregate1, TAggregate2, TEnumerable>(enumerable, seed, aggregator);
        }

        private sealed class FurtherAggregatedEnumerable<TElement, TAggregate1, TAggregate2, TEnumerable> : IFurtherAggregatedEnumerable<TElement, TAggregate1, TAggregate2, TEnumerable> where TEnumerable : IAggregatedEnumerable<TElement, TAggregate1>
        {
            public FurtherAggregatedEnumerable(TEnumerable enumerable, TAggregate2 seed, Func<TAggregate2, TElement, TAggregate2> aggregator)
            {
                this.enumerable = enumerable;
                this.seed = seed;
                this.aggregator = aggregator;
                this.isAggregated = false;
            }

            public TEnumerable Enumerable => throw new NotImplementedException();

            public TAggregate2 Aggregate => throw new NotImplementedException();

            public IEnumerator<TElement> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class AggregatedEnumerable<TElement, TAggregate> : IAggregatedEnumerable<TElement, TAggregate>
        {
            private readonly IEnumerable<TElement> enumerable;

            private readonly TAggregate seed;

            private readonly Func<TAggregate, TElement, TAggregate> aggregator;

            private bool isAggregated;

            private TAggregate aggregate;

            public AggregatedEnumerable(IEnumerable<TElement> enumerable, TAggregate seed, Func<TAggregate, TElement, TAggregate> aggregator)
            {
                this.enumerable = enumerable;
                this.seed = seed;
                this.aggregator = aggregator;
                this.isAggregated = false;
            }

            public TAggregate Aggregate
            {
                get
                {
                    if (!this.isAggregated)
                    {
                        foreach (var element in this)
                        {
                        }
                    }

                    return this.aggregate;
                }
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                var aggregate = this.seed;
                foreach (var element in this.enumerable)
                {
                    aggregate = this.aggregator(aggregate, element);
                    yield return element;
                }

                this.aggregate = aggregate;
                this.isAggregated = true;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }*/

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

        /*public static PossibleError<TValueResult, TErrorResult> Map<TValue, TError, TValueResult, TErrorResult>(
            this PossibleError<TValue, TError> possibleError, 
            Func<TValue, TValueResult> valueSelector, 
            Func<TError, TErrorResult> errorSelector)
        {
            if (possibleError.IsError)
            {
                return PossibleError.Error<TValueResult, TErrorResult>(errorSelector(possibleError.Error));
            }
            else
            {
                return PossibleError.Value<TValueResult, TErrorResult>(valueSelector(possibleError.Value));
            }
        }

        public static IEnumerable<PossibleError<TValue, TError>> Distribute<TValue, TError>(this PossibleError<IEnumerable<TValue>, TError> possibleError)
        {
            if (possibleError.IsError)
            {
                return new[] { PossibleError.Error<TValue, TError>(possibleError.Error) };
            }
            else
            {
                return possibleError.Value.Select(value => PossibleError.Value<TValue, TError>(value));
            }
        }

        public static IEnumerable<TValue> Enumerate<TValue, TError>(this IEnumerable<PossibleError<TValue, TError>> self, Action<TError> aggregator)
        {
            foreach (var element in self)
            {
                if (element.IsError)
                {
                    aggregator(element.Error);
                }
                else
                {
                    yield return element.Value;
                }
            }
        }*/

        /*public static IAggergatorEnumerable<TElement, TAggregate> ApplyAggregation<TElement, TAggregate>(
            this IEnumerable<TElement> enumerable, 
            TAggregate seed, 
            Func<TAggregate, TElement, TAggregate> aggregator) 
        {
            return new AggregatorEnumerable<TElement, TAggregate>(enumerable, seed, aggregator);
        }

        private sealed class AggregatorEnumerable<TElement, TAggregate> : IAggergatorEnumerable<TElement, TAggregate>
        {
            private readonly IEnumerable<TElement> enumerable;

            private readonly TAggregate seed;

            private readonly Func<TAggregate, TElement, TAggregate> aggregator;

            public AggregatorEnumerable(
                IEnumerable<TElement> enumerable,
                TAggregate seed,
                Func<TAggregate, TElement, TAggregate> aggregator)
            {
                this.enumerable = enumerable;
                this.seed = seed;
                this.aggregator = aggregator;

                this.IsAggregated = false;
                this.Aggregation = this.seed;
            }

            public bool IsAggregated { get; private set; }

            public TAggregate Aggregation { get; private set; }

            public IEnumerator<TElement> GetEnumerator()
            {
                var accumulate = this.seed;
                foreach (var element in this.enumerable)
                {
                    accumulate = this.aggregator(accumulate, element);
                    yield return element;
                }

                this.Aggregation = accumulate;
                this.IsAggregated = true;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }*/
    }

    /*public interface IAggergatorEnumerable<TElement, TAggregate> : IEnumerable<TElement>
    {
        public bool IsAggregated { get; }

        public TAggregate Aggregation { get; }
    }*/

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