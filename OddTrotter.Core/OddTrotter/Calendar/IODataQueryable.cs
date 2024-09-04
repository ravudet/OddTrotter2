namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    using OddTrotter.AzureBlobClient;
    using OddTrotter.GraphClient;

    /*
    how to generate? .../calendar?$expand=events($filter={fitler})
    */

    public sealed class GraphCalendarContext : IODataInstanceContext
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri calendarUri;

        public GraphCalendarContext(IGraphClient graphClient, RelativeUri calendarUri)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri;

            this.Events = new CalendarEventCollectionContext(this.graphClient, this.calendarUri);
        }

        public IODataCollectionContext<GraphCalendarEvent> Events { get; }

        private sealed class CalendarEventCollectionContext : IODataCollectionContext<GraphCalendarEvent>
        {
            private readonly IGraphClient graphClient;

            private readonly RelativeUri eventsUri;

            /// <summary>
            /// TODO properly document
            /// prefixed with '$filter' if not null
            /// </summary>
            private readonly string? filter;

            /// <summary>
            /// TODO properly document
            /// prefixed with '$select' if not null
            /// </summary>
            private readonly string? select;

            /// <summary>
            /// TODO properly document
            /// prefixed with '$top' if not null
            /// </summary>
            private readonly string? top;

            /// <summary>
            /// TODO properly document
            /// prefixed with '$orderBy' if not null
            /// </summary>
            private readonly string? orderBy;

            public CalendarEventCollectionContext(IGraphClient graphClient, RelativeUri calendarUri)
                : this(
                      graphClient, 
                      new Uri(calendarUri.OriginalString.TrimEnd('/') + "/events", UriKind.Relative).ToRelativeUri(), 
                      null,
                      null,
                      null,
                      null)
            {
            }

            private CalendarEventCollectionContext(
                IGraphClient graphClient, 
                RelativeUri eventsUri, 
                string? filter, 
                string? select,
                string? top,
                string? orderBy)
            {
                this.graphClient = graphClient;
                this.eventsUri = eventsUri;
                this.filter = filter;
                this.select = select;
                this.top = top;
                this.orderBy = orderBy;
            }

            /*
            var url =
                $"/me/calendar/events?" +
                $"$select=body,start,subject,responseStatus,webLink&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss.000000")}' and isCancelled eq false";
            */

            public IODataCollectionContext<GraphCalendarEvent> Filter()
            {
                return new CalendarEventCollectionContext(
                    this.graphClient,
                    this.eventsUri,
                    "$filter=id eq 'asdf'",
                    this.select,
                    this.top,
                    this.orderBy);
            }

            public IODataCollectionContext<GraphCalendarEvent> Select<TProperty>(Expression<Func<GraphCalendarEvent, TProperty>> selector)
            {
                //// TODO prevent two selects of the same property
                //// TODO do any other validations needed for the spec to be accureate
                if (selector.Body is MemberExpression memberExpression)
                {
                    var propertyPath = TraverseMemberExpression(memberExpression, Enumerable.Empty<MemberExpression>());
                    return new CalendarEventCollectionContext(
                        this.graphClient,
                        this.eventsUri,
                        this.filter,
                        this.select == null ? $"$select={propertyPath}" : $"{this.select},{propertyPath}",
                        this.top,
                        this.orderBy);
                }
                else
                {
                    throw new Exception("TODO only member expressions are allowed");
                }
            }

            private string TraverseMemberExpression(MemberExpression expression, IEnumerable<MemberExpression> previousExpressions)
            {
                //// TODO should this "replace" with a constant expression of the property path?
                if (expression.Expression?.NodeType != ExpressionType.Parameter)
                {
                    if (expression.Expression is MemberExpression memberExpression)
                    {
                        return TraverseMemberExpression(memberExpression, previousExpressions.Append(expression));
                    }
                }
                else if (expression.Member.Name == nameof(GraphCalendarEvent.Id))
                {
                    return "id"; //// TODO you should generalize at some point and pull this from a c# attribute
                }
                else if (expression.Member.Name == nameof(GraphCalendarEvent.Body))
                {
                    var propertyPath = new StringBuilder("body");
                    foreach (var previousExpression in previousExpressions)
                    {
                        if (previousExpression.Member.Name == nameof(BodyStructure.Content))
                        {
                            propertyPath.Append("/content");
                        }
                        else
                        {
                            throw new Exception("tODO");
                        }
                    }

                    return propertyPath.ToString();
                }
                else if (expression.Member.Name == nameof(GraphCalendarEvent.Start))
                {
                    var propertyPath = new StringBuilder("start");
                    foreach (var previousExpression in previousExpressions)
                    {
                        if (previousExpression.Member.Name == nameof(TimeStructure.DateTime))
                        {
                            propertyPath.Append("/dateTime");
                        }
                        if (previousExpression.Member.Name == nameof(TimeStructure.TimeZone))
                        {
                            propertyPath.Append("/timeZone");
                        }
                        else
                        {
                            throw new Exception("tODO");
                        }
                    }

                    return propertyPath.ToString();
                }
                else if (expression.Member.Name == nameof(GraphCalendarEvent.End))
                {
                    var propertyPath = new StringBuilder("end");
                    foreach (var previousExpression in previousExpressions)
                    {
                        if (previousExpression.Member.Name == nameof(TimeStructure.DateTime))
                        {
                            propertyPath.Append("/dateTime");
                        }
                        if (previousExpression.Member.Name == nameof(TimeStructure.TimeZone))
                        {
                            propertyPath.Append("/timeZone");
                        }
                        else
                        {
                            throw new Exception("tODO");
                        }
                    }

                    return propertyPath.ToString();
                }
                else if (expression.Member.Name == nameof(GraphCalendarEvent.Subject))
                {
                    return "subject";
                }
                else if (expression.Member.Name == nameof(GraphCalendarEvent.ResponseStatus))
                {
                    var propertyPath = new StringBuilder("responseStatus");
                    foreach (var previousExpression in previousExpressions)
                    {
                        if (previousExpression.Member.Name == nameof(ResponseStatusStructure.Response))
                        {
                            propertyPath.Append("/response");
                        }
                        if (previousExpression.Member.Name == nameof(ResponseStatusStructure.Time))
                        {
                            propertyPath.Append("/time");
                        }
                        else
                        {
                            throw new Exception("tODO");
                        }
                    }

                    return propertyPath.ToString();
                }
                else if (expression.Member.Name == nameof(GraphCalendarEvent.WebLink))
                {
                    return "webLink";
                }

                throw new Exception("TODO not a known member; do you really want to be strict about this? well, actually, you probably *should* be consistent about ti because even if they select a property, you won't deserialiez it if you don't know about; at the same time, though, you're efectively doing what the odata webapi stuff does and hide things; m,aybe you should expose the url somewhere, and if you do that, then it might make sense to allow properties that you're not aware of");
            }

            public IODataCollectionContext<GraphCalendarEvent> Top(int count)
            {
                if (this.top != null)
                {
                    throw new Exception("tODO");
                }

                return new CalendarEventCollectionContext(
                    this.graphClient,
                    this.eventsUri,
                    this.filter,
                    this.select,
                    $"$top={count}",
                    this.orderBy);
            }

            public IODataCollectionContext<GraphCalendarEvent> OrderBy<TProperty>(Expression<Func<GraphCalendarEvent, TProperty>> selector)
            {
                //// TODO validate that you only allow expressions that are supported by graph?
                if (selector.Body is MemberExpression memberExpression)
                {
                    var propertyPath = TraverseMemberExpression(memberExpression, Enumerable.Empty<MemberExpression>());
                    return new CalendarEventCollectionContext(
                        this.graphClient,
                        this.eventsUri,
                        this.filter,
                        this.select,
                        this.top,
                        $"$orderby={propertyPath}");
                }
                else
                {
                    throw new Exception("TODO only member expressions are allowed");
                }
            }

            public ODataCollection<GraphCalendarEvent> Values //// TODO shouldn't this return type be something odata-y? like, the properties need to be marked as selected and stuff, right?
            {
                get
                {
                    var queryOptionsList = new[]
                    {
                        this.select,
                        this.filter,
                        this.top,
                    }.Where(option => option != null);

                    var queryOptions = string.Join("&", queryOptionsList);

                    var requestUri = string.IsNullOrEmpty(queryOptions) ?
                        this.eventsUri :
                        new Uri($"{this.eventsUri.OriginalString}?{queryOptions}", UriKind.Relative).ToRelativeUri();
                    return GetCollection<GraphCalendarEvent>(this.graphClient, requestUri);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="graphClient"></param>
            /// <param name="uri"></param>
            /// <returns></returns>
            /// <exception cref="InvalidAccessTokenException">
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
            /// <exception cref="InvalidAccessTokenException">
            /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request
            /// </exception>
            /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
            /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
            private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, RelativeUri uri)
            {
                using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
                {
                    return await ReadPage<T>(httpResponse).ConfigureAwait(false); //// TODO add configure await to todolistservice
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
            /// <exception cref="InvalidAccessTokenException">
            /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request
            /// </exception>
            /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
            /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
            private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, AbsoluteUri uri)
            {
                using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
                {
                    return await ReadPage<T>(httpResponse).ConfigureAwait(false); //// TODO add configure await to todolistservice
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

            private sealed class ODataCollectionPage<T>
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
        }
    }

    public interface IODataInstanceContext
    {
        //// TODO
    }

    public interface IODataCollectionContext<T>
    {
        //// TODO do you want this? IEnumerator<T> GetEnumerator();

        ODataCollection<T> Values { get; }

        IODataCollectionContext<T> Select<TProperty>(Expression<Func<T, TProperty>> selector);

        IODataCollectionContext<T> Top(int count);

        IODataCollectionContext<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> selector);

        IODataCollectionContext<T> Filter();

        //// TODO T this[somekey?] { get; }
    }

    public sealed class ODataCollection<T>
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
}
