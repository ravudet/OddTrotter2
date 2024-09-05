namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Net.Quic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using OddTrotter.AzureBlobClient;
    using OddTrotter.GraphClient;

    /*
    how to generate? .../calendar?$expand=events($filter={fitler})
    */

    public class GraphCalendarContextEvent //// TODO deal with property nullability
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri eventUri;

        public GraphCalendarContextEvent(IGraphClient graphClient, RelativeUri eventUri, GraphCalendarContextEvent graphCalendarContextEvent) //// TODO it would be nice if this were private...
        {
            this.graphClient = graphClient;
            this.eventUri = eventUri;

            this.Id = graphCalendarContextEvent.Id;
            this.Body = graphCalendarContextEvent.Body; //// TODO these also need copy constructors
            this.Start = graphCalendarContextEvent.Start;
            this.End = graphCalendarContextEvent.End;
            this.Subject = graphCalendarContextEvent.Subject;
            this.ResponseStatus = graphCalendarContextEvent.ResponseStatus;
            this.WebLink = graphCalendarContextEvent.WebLink;
            this.Type = graphCalendarContextEvent.Type;
            this.IsCancelled = graphCalendarContextEvent.IsCancelled;

            this.Instances = new InstancesContext(graphClient, eventUri);
        }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("body")]
        public BodyStructure? Body { get; set; }

        [JsonPropertyName("start")]
        public TimeStructure? Start { get; set; }

        [JsonPropertyName("end")]
        public TimeStructure? End { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("responseStatus")]
        public ResponseStatusStructure? ResponseStatus { get; set; }

        [JsonPropertyName("webLink")]
        public string? WebLink { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("isCancelled")]
        public bool? IsCancelled { get; set; }

        //// TODO add *all* of the properties here

        public IODataCollectionContext<GraphCalendarContextEvent> Instances { get; } //// TODO what if a caller whats to filter events based on instances?

        private sealed class InstancesContext : IODataCollectionContext<GraphCalendarContextEvent>
        {
            private readonly IGraphClient graphClient;

            private readonly RelativeUri instancesUri;

            public InstancesContext(IGraphClient graphClient, RelativeUri eventUri)
            {
                this.graphClient = graphClient;
                this.instancesUri = new Uri(eventUri.OriginalString.TrimEnd('/') + "/instances", UriKind.Relative).ToRelativeUri();
            }

            public ODataCollection<GraphCalendarContextEvent> Values => throw new NotImplementedException();

            public IODataCollectionContext<GraphCalendarContextEvent> Filter(Expression<Func<GraphCalendarContextEvent, bool>> predicate)
            {
                throw new NotImplementedException();
            }

            public IODataCollectionContext<GraphCalendarContextEvent> OrderBy<TProperty>(Expression<Func<GraphCalendarContextEvent, TProperty>> selector)
            {
                throw new NotImplementedException();
            }

            public IODataCollectionContext<GraphCalendarContextEvent> Select<TProperty>(Expression<Func<GraphCalendarContextEvent, TProperty>> selector)
            {
                throw new NotImplementedException();
            }

            public IODataCollectionContext<GraphCalendarContextEvent> Top(int count)
            {
                throw new NotImplementedException();
            }
        }
    }

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

        public IODataCollectionContext<GraphCalendarContextEvent> Events { get; }

        private sealed class CalendarEventCollectionContext : IODataCollectionContext<GraphCalendarContextEvent>
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

#pragma warning disable IDE1006 // Naming Styles
            public static bool op_Equality(string first, string second)
#pragma warning restore IDE1006 // Naming Styles
            {
                return true;
            }

            public IODataCollectionContext<GraphCalendarContextEvent> Filter(Expression<Func<GraphCalendarContextEvent, bool>> predicate)
            {
                //// TODO how do you go about making this more complete? and is there a way to make it able to validate according to what graph actually supports?

                var filterBuilder = new StringBuilder();
                /*if (predicate.Body is BinaryExpression binaryExpression)
                {
                    TraverseBinaryExpression(binaryExpression, filterBuilder);

                    if (binaryExpression.Left is MemberExpression leftMemberExpression)
                    {
                        filterBuilder.Append(TraverseMemberExpression(leftMemberExpression, Enumerable.Empty<MemberExpression>()));
                    }

                    if (binaryExpression.Left.Type == typeof(DateTime))
                    {
                        // we support some parsing methods for datetime
                        //// TODO also add support for guid?
                        //// TODO for anything that's not a literal, can you compile it and evaluate? maybe, but what about cases where you hvae a method that uses the predicate parameter as an argument (e.g. event => DoSomething(event))
                    }
                    else
                    {


                        Expression<Func<string, string, bool>> expression = (first, second) => first == second;
                        var stringOperatorEquals = ((BinaryExpression)expression.Body).Method;

                        if (binaryExpression.Method == stringOperatorEquals)
                        {
                        }
                    }
                }

                if (filterBuilder.Length == 0)
                {
                    throw new Exception("TODO");
                }*/

                TraverseFilter(predicate.Body, filterBuilder);

                return new CalendarEventCollectionContext(
                    this.graphClient,
                    this.eventsUri,
                    this.filter == null ? $"$filter={filterBuilder}" : $"{this.filter} and {filterBuilder}",
                    this.select,
                    this.top,
                    this.orderBy);
            }

            private void TraverseBinaryExpression(BinaryExpression expression, StringBuilder queryParameter)
            {
                TraverseFilter(expression.Left, queryParameter);

                if (
                    (expression.Method?.IsSpecialName == true && expression.Method?.Name == "op_Equality") // 'operator ==' overload
                    || (expression.NodeType == ExpressionType.Equal) // primitive equality provided by the compiler
                    )
                {
                    queryParameter.Append(" eq ");
                }
                else if (
                    (expression.Method?.IsSpecialName == true && expression.Method?.Name == "op_GreaterThan") // 'operator >' overload
                    || (expression.NodeType == ExpressionType.GreaterThan) // primitive comparison provided by the compiler
                    )
                {
                    queryParameter.Append(" gt ");
                }
                else if (expression.NodeType == ExpressionType.AndAlso)
                {
                    queryParameter.Append(" and ");
                }
                else if (expression.NodeType == ExpressionType.OrElse)
                {
                    queryParameter.Append(" or ");
                }
                else
                {
                    //// TODO other operators
                    throw new Exception("TODO");
                }

                TraverseFilter(expression.Right, queryParameter);
            }

            private void TraverseConstantExpression(ConstantExpression expression, StringBuilder queryParameter)
            {
                var quotes = false;
                if (expression.Type == typeof(string))
                {
                    quotes = true;
                }
                else if (expression.Type == typeof(DateTime))
                {
                    quotes = true;
                }
                else if (false)
                {
                    //// TODO other constants that need quoted here
                    throw new Exception("TODO");
                }

                var value = expression.Value;

                // handle literals
                if (expression.Type == typeof(bool))
                {
                    value = (bool)value! ? "true" : "false";
                }
                else if (false)
                {
                    //// TODO other literals here
                }

                if (quotes)
                {
                    queryParameter.Append("'");
                }

                queryParameter.Append(value);

                if (quotes)
                {
                    queryParameter.Append("'");
                }
            }

            private void TraverseMethodCallExpression(MethodCallExpression expression, StringBuilder queryParameter)
            {
                Expression<Func<DateTime>> dateTimeParseEpression = () => DateTime.Parse(string.Empty);

                var dateTimeParseMethodInfo = ((MethodCallExpression)dateTimeParseEpression.Body).Method;

                if (expression.Method == dateTimeParseMethodInfo)
                {
                    //// TODO if it doesn't equal one, that's really something hugely invalid, and we are swallowing it; is that ok?
                    if (expression.Arguments.Count == 1)
                    {
                        if (expression.Arguments[0] is ConstantExpression dateTimeConstantExpression)
                        {
                            //// TODO do you want to do a manual formatting of dateTimeConstantExpression.Value? do you want to actually call datetime.parse on it?

                            if (!(dateTimeConstantExpression.Value is string dateTimeConstantExpressionValue))
                            {
                                //// TODO do you want to just assume that it's a string?
                                throw new Exception("TODO");
                            }

                            var dateTime = DateTime.Parse(dateTimeConstantExpressionValue);

                            //// TODO use jsonpropertyname attributes to get the start and datetime strings (you should actually use brand new attributes)
                            queryParameter.Append("'");
                            queryParameter.Append(dateTimeConstantExpression.Value);
                            queryParameter.Append("'");
                        }
                        else
                        {
                            //// TODO you are oinly supporting constants for this convenience method call, right? maybe also support a closure?
                            throw new Exception("TODO");
                        }
                    }
                }
                else
                {
                    //// TODO other conveinece method calls here, like guid.parse
                    throw new Exception("tODO");
                }
            }

            private void TraverseFilter(Expression expression, StringBuilder queryParameter)
            {
                if (expression is ConstantExpression constantExpression)
                {
                    TraverseConstantExpression(constantExpression, queryParameter);
                }
                else if (expression is MemberExpression memberExpression)
                {
                    TraverseMemberExpression(memberExpression, queryParameter);
                }
                else if (expression is BinaryExpression binaryExpression)
                {
                    TraverseBinaryExpression(binaryExpression, queryParameter);
                }
                else if (expression.NodeType == ExpressionType.Convert && expression is UnaryExpression unaryExpression)
                {
                    //// TODO a "convert" expression is the result of a nullable casting
                    TraverseFilter(unaryExpression.Operand, queryParameter);
                }
                else if (expression is MethodCallExpression methodCallExpression)
                {
                    TraverseMethodCallExpression(methodCallExpression, queryParameter);
                }
                else
                {
                    throw new Exception("TODO");
                }
            }

            public IODataCollectionContext<GraphCalendarContextEvent> Select<TProperty>(Expression<Func<GraphCalendarContextEvent, TProperty>> selector)
            {
                //// TODO prevent two selects of the same property
                //// TODO do any other validations needed for the spec to be accureate
                if (selector.Body is MemberExpression memberExpression)
                {
                    var propertyPath = TraverseCalendarEventMemberExpression(memberExpression, Enumerable.Empty<MemberExpression>());
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

            private void TraverseMemberExpression(MemberExpression memberExpression, StringBuilder queryParameter)
            {
                var traversed = TraverseCalendarEventMemberExpression(memberExpression, Enumerable.Empty<MemberExpression>());
                if (traversed == null)
                {
                    TraverseClosureMemberExpression(memberExpression, queryParameter);
                }
                else
                {
                    queryParameter.Append(traversed);
                }
            }

            private void TraverseClosureMemberExpression(MemberExpression memberExpression, StringBuilder queryParameter)
            {
                if (memberExpression.Expression is ConstantExpression constantExpression) //// this is a "dynamic" closure (both instance accesses, and locally scoped variable accesses)
                {
                    //// TODO null checks
                    var fieldInfo = constantExpression.Value?.GetType().GetField(memberExpression.Member.Name);
                    var value = fieldInfo?.GetValue(constantExpression.Value);

                    if (fieldInfo?.FieldType == typeof(DateTime))
                    {
                        queryParameter.Append($"'{((DateTime)value!).ToString("yyyy-MM-ddThh:mm:ss.000000")}'");
                    }
                    else
                    {
                        //// TODO support other EDM primitives
                        throw new Exception("tODO");
                    }

                    //// TODO do you want to do a manual formatting of dateTimeConstantExpression.Value? do you want to actually call datetime.parse on it?

                    //// TODO use jsonpropertyname attributes to get the start and datetime strings (you should actually use brand new attributes)
                    ////return $"start/dateTime gt '{dateTimeValue}'";
                }
                else
                {
                    //// TODO support other kinds of closures here
                    throw new Exception("tODO");
                }
            }

            private string? TraverseCalendarEventMemberExpression(MemberExpression expression, IEnumerable<MemberExpression> previousExpressions)
            {
                if (expression.Expression?.NodeType == ExpressionType.Constant)
                {
                    //// TODO make this a "try" method instead of null here
                    return null;
                }

                //// TODO should this "replace" with a constant expression of the property path?
                if (expression.Expression?.NodeType != ExpressionType.Parameter)
                {
                    if (expression.Expression is MemberExpression memberExpression)
                    {
                        return TraverseCalendarEventMemberExpression(memberExpression, previousExpressions.Append(expression));
                    }
                }
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.Id))
                {
                    return "id"; //// TODO you should generalize at some point and pull this from a c# attribute
                }
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.Body))
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
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.Start))
                {
                    var propertyPath = new StringBuilder("start");
                    foreach (var previousExpression in previousExpressions)
                    {
                        if (previousExpression.Member.Name == nameof(TimeStructure.DateTime))
                        {
                            propertyPath.Append("/dateTime");
                        }
                        else if (previousExpression.Member.Name == nameof(TimeStructure.TimeZone))
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
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.End))
                {
                    var propertyPath = new StringBuilder("end");
                    foreach (var previousExpression in previousExpressions)
                    {
                        if (previousExpression.Member.Name == nameof(TimeStructure.DateTime))
                        {
                            propertyPath.Append("/dateTime");
                        }
                        else if (previousExpression.Member.Name == nameof(TimeStructure.TimeZone))
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
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.Subject))
                {
                    return "subject";
                }
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.ResponseStatus))
                {
                    var propertyPath = new StringBuilder("responseStatus");
                    foreach (var previousExpression in previousExpressions)
                    {
                        if (previousExpression.Member.Name == nameof(ResponseStatusStructure.Response))
                        {
                            propertyPath.Append("/response");
                        }
                        else if (previousExpression.Member.Name == nameof(ResponseStatusStructure.Time))
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
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.WebLink))
                {
                    return "webLink";
                }
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.Type))
                {
                    return "type";
                }
                else if (expression.Member.Name == nameof(GraphCalendarContextEvent.IsCancelled))
                {
                    return "isCancelled";
                }

                throw new Exception("TODO not a known member; do you really want to be strict about this? well, actually, you probably *should* be consistent about ti because even if they select a property, you won't deserialiez it if you don't know about; at the same time, though, you're efectively doing what the odata webapi stuff does and hide things; m,aybe you should expose the url somewhere, and if you do that, then it might make sense to allow properties that you're not aware of");
            }

            public IODataCollectionContext<GraphCalendarContextEvent> Top(int count)
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

            public IODataCollectionContext<GraphCalendarContextEvent> OrderBy<TProperty>(Expression<Func<GraphCalendarContextEvent, TProperty>> selector)
            {
                //// TODO validate that you only allow expressions that are supported by graph?
                if (selector.Body is MemberExpression memberExpression)
                {
                    var propertyPath = TraverseCalendarEventMemberExpression(memberExpression, Enumerable.Empty<MemberExpression>());
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

            public ODataCollection<GraphCalendarContextEvent> Values //// TODO shouldn't this return type be something odata-y? like, the properties need to be marked as selected and stuff, right?
            {
                get
                {
                    //// TODO it would be very good to make this lazy
                    var queryOptionsList = new[]
                    {
                        this.select,
                        this.filter,
                        this.orderBy,
                        this.top,
                    }.Where(option => option != null);

                    var queryOptions = string.Join("&", queryOptionsList);

                    var requestUri = string.IsNullOrEmpty(queryOptions) ?
                        this.eventsUri :
                        new Uri($"{this.eventsUri.OriginalString}?{queryOptions}", UriKind.Relative).ToRelativeUri();
                    var deserializedResponse = GetCollection<GraphCalendarContextEvent>(this.graphClient, requestUri);

                    var queryableResponse = new ODataCollection<GraphCalendarContextEvent>(
                        deserializedResponse.Elements.Select(element => new GraphCalendarContextEvent(
                            this.graphClient,
                            new Uri(this.eventsUri.OriginalString.TrimEnd('/') + $"/{element.Id}", UriKind.Relative).ToRelativeUri(),
                            element)),
                        deserializedResponse.LastRequestedPageUrl);

                    return queryableResponse;
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

        IODataCollectionContext<T> Filter(Expression<Func<GraphCalendarContextEvent, bool>> predicate);

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
