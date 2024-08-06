namespace OddTrotter.Calendar
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("start")]
        public TimeStructure? Start { get; set; }

        //// TODO add *all* of the properties here
    }

    public sealed class TimeStructure
    {
        [JsonPropertyName("dateTime")]
        public DateTime? DateTime { get; set; } //// TODO can you trust this will be parsed correctly?

        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }
    }

    public sealed class GraphCalendar : IV2Queryable<GraphCalendarEvent>, IWhereQueryable<GraphCalendarEvent>
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
            return GetEvents(calendarUri).ToBlockingEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IV2Queryable<GraphCalendarEvent> Where(Expression<Func<GraphCalendarEvent, bool>> predicate)
        {
            return new Whered(this.graphClient, this.calendarUri, predicate);
        }

        private sealed class Whered : IV2Queryable<GraphCalendarEvent>
        {
            private readonly IGraphClient graphClient;

            private readonly RelativeUri calendarUri;

            private readonly string? filter;

            private readonly Func<GraphCalendarEvent, bool> predicate;

            public Whered(IGraphClient graphClient, RelativeUri calendarUri, Expression<Func<GraphCalendarEvent, bool>> predicate)
            {
                this.graphClient = graphClient;
                this.calendarUri = calendarUri;

                this.filter = ParsePredicateExpression(predicate);
                if (this.filter == null)
                {
                    this.predicate = predicate.Compile();
                }
                else
                {
                    this.predicate = _ => true; //// TODO singleton
                }
            }

            private static string? ParsePredicateExpression(Expression<Func<GraphCalendarEvent, bool>> predicate)
            {
                if (predicate.Body is BinaryExpression binaryExpression)
                {
                    //// TODO we actually "know" it's binary here, right? because it's a func<t,bool>?

                    //// TODO which binary operation?
                    if (binaryExpression.Left is MemberExpression leftMemberExpression)
                    {
                        var leftMember = leftMemberExpression.Member;
                        if (string.Equals(leftMember.Name, nameof(TimeStructure.DateTime), StringComparison.Ordinal))
                        {
                            if (leftMemberExpression.Expression is MemberExpression dateTimeMemberExpression)
                            {
                                var dateTimeMember = dateTimeMemberExpression.Member;
                                if (string.Equals(dateTimeMember.Name, nameof(CalendarEvent.Start), StringComparison.Ordinal))
                                {
                                    //// left side is the start, how about the right side?
                                    //// TODO the only binary operations we are supporting will make sure that the return type of the right side is a datetime, but is this true for non-graph calendar event cases with other binary operations?
                                    
                                    if (binaryExpression.Right is UnaryExpression rightConvertExpression)
                                    {
                                        //// this is a datetime.parse
                                        if (rightConvertExpression.Operand is MethodCallExpression rightDateTimeParseExpression)
                                        {
                                            var dateTimeParseMethod = rightDateTimeParseExpression.Method;
                                            if (string.Equals(dateTimeParseMethod.Name, nameof(DateTime.Parse), StringComparison.Ordinal) && typeof(DateTime).GetMethods().Contains(dateTimeParseMethod))
                                            {
                                                //// TODO can you rely on a reference equality check? you can, surprisingly...i wonder if that's *actually* reliable

                                                //// TODO if it doesn't equal one, that's really something hugely invalid, and we are swallowing it; is that ok?
                                                if (rightDateTimeParseExpression.Arguments.Count == 1)
                                                {
                                                    if (rightDateTimeParseExpression.Arguments[0] is ConstantExpression dateTimeConstantExpression)
                                                    {
                                                        //// TODO do you want to do a manual formatting of dateTimeConstantExpression.Value? do you want to actually call datetime.parse on it?

                                                        //// TODO use jsonpropertyname attributes to get the start and datetime strings (you should actually use brand new attributes)
                                                        return $"start/dateTime gt '{dateTimeConstantExpression.Value}'";
                                                    }
                                                }
                                            }
                                        }
                                        else if (rightConvertExpression.Operand is MemberExpression closureMemberExpression) //// this is a closure access
                                        {
                                            var rightMember = closureMemberExpression.Member;
                                            if (closureMemberExpression.Expression == null) //// this is a "static" closure
                                            {
                                                var fieldInfo = rightMember.DeclaringType?.GetField(rightMember.Name);
                                                var dateTimeValue = fieldInfo?.GetValue(null);
                                                //// TODO what about property accesses instead of field accesses? can you use getmember for that?
                                                //// TODO can you combine this branch with the below branch?

                                                //// TODO do you want to do a manual formatting of dateTimeConstantExpression.Value? do you want to actually call datetime.parse on it?

                                                //// TODO use jsonpropertyname attributes to get the start and datetime strings (you should actually use brand new attributes)
                                                return $"start/dateTime gt '{dateTimeValue}'";
                                            }
                                            else if (closureMemberExpression.Expression is ConstantExpression dateTimeConstantExpression) //// this is a "dynamic" closure (both instance accesses, and locally scoped variable accesses)
                                            {
                                                //// TODO null checks
                                                var fieldInfo = dateTimeConstantExpression.Value?.GetType().GetField(rightMember.Name);
                                                var dateTimeValue = fieldInfo?.GetValue(dateTimeConstantExpression.Value);



                                                //// TODO do you want to do a manual formatting of dateTimeConstantExpression.Value? do you want to actually call datetime.parse on it?

                                                //// TODO use jsonpropertyname attributes to get the start and datetime strings (you should actually use brand new attributes)
                                                return $"start/dateTime gt '{dateTimeValue}'";
                                            }
                                        }
                                    }
                                }

                                //// TODO you can implement checks for 'end' property here
                            }
                        }
                    }
                }

                if (predicate.Body is MemberExpression memberExpression)
                {
                    var member = memberExpression.Member;
                    if (string.Equals(member.Name, "start", StringComparison.Ordinal))
                    {

                    }
                }

                return null;
            }

            public IEnumerator<GraphCalendarEvent> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private IAsyncEnumerable<GraphCalendarEvent> GetEvents(RelativeUri calendarUri)
        {
            var instanceEvents = GetInstanceEvents(calendarUri); //// TODO configureawait somehow?
            //// TODO also get series event instances and merge them
            return instanceEvents;
        }

        private async IAsyncEnumerable<GraphCalendarEvent> GetInstanceEvents(RelativeUri calendarUri)
        {
            var instanceEventsUri = new Uri(calendarUri, "?$filter=type eq 'singleInstance'").ToRelativeUri();
            await foreach (var instanceEvent in GetCollection<GraphCalendarEvent>(instanceEventsUri).ConfigureAwait(false))
            {
                yield return instanceEvent;
            }
        }

        /*private async IAsyncEnumerable<GraphCalendarEvent> GetSeriesEvents(RelativeUri calendarUri)
        {
            var seriesEventsUri = new Uri(calendarUri, "?$filter=type eq 'seriesMaster'").ToRelativeUri();
            await foreach (var seriesEvent in GetCollection<GraphCalendarEvent>(seriesEventsUri).ConfigureAwait(false))
            {
            }
        }*/

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

    public static class Driver
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static void DoWork()
        {
            var graphCalendar = new GraphCalendar(null, null, null);

            graphCalendar.Where(calendarEvent => calendarEvent.Id == "Asdf");

            var events = graphCalendar.AsV2Enumerable().Where(calendarEvent =>
            {
                var value = calendarEvent.Id == "asdf";
                return value;
            });
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    public interface IV2Queryable<out T> : IV2Enumerable<T>
    {
    }

    public interface IWhereQueryable<TSource> : IV2Queryable<TSource>
    {
        public IV2Queryable<TSource> Where(Expression<Func<TSource, bool>> predicate)
        {
            return this.WhereDefault(predicate);
        }
    }

    public static class QueryableExtensions
    {
        public static IV2Queryable<TSource> Where<TSource>(this IV2Queryable<TSource> queryable, Expression<Func<TSource, bool>> predicate)
        {
            if (queryable is IWhereQueryable<TSource> where)
            {
                return where.Where(predicate);
            }

            return queryable.WhereDefault(predicate);
        }

        internal static IV2Queryable<TSource> WhereDefault<TSource>(this IV2Queryable<TSource> queryable, Expression<Func<TSource, bool>> predicate)
        {
            //// TODO monad check

            return new QueryableAdapter<TSource>(queryable.AsV2Enumerable().Where(predicate.Compile()));
        }

        private sealed class QueryableAdapter<T> : IV2Queryable<T>
        {
            private readonly IV2Enumerable<T> enumerable;

            public QueryableAdapter(IV2Enumerable<T> enumerable)
            {
                this.enumerable = enumerable;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this.enumerable.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
