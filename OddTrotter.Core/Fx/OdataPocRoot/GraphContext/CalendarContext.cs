namespace Fx.OdataPocRoot.GraphContext
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Fx.OdataPocRoot.Graph;
    using Fx.OdataPocRoot.Odata;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;
    using OddTrotter.GraphClient;

    public sealed class CalendarContext : IInstanceContext<Calendar>
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri calendarUri;

        private readonly Select? select;

        public CalendarContext(IGraphClient graphClient, RelativeUri calendarUri)
            : this(graphClient, calendarUri, null)
        {
        }

        private CalendarContext(IGraphClient graphClient, RelativeUri calendarUri, Select? select)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri;
            this.select = select;
        }

        public async Task<Calendar> Evaluate()
        {
            using (var httpResponseMessage = await this.graphClient.GetAsync(this.calendarUri).ConfigureAwait(false))
            {
                httpResponseMessage.EnsureSuccessStatusCode();
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                var calendar = JsonSerializer.Deserialize<Calendar>(httpResponseContent);
                if (calendar ==null)
                {
                    throw new Exception("TODO null calendar");
                }

                return calendar;
            }
        }

        public IInstanceContext<Calendar> Select<TProperty>(Expression<Func<Calendar, TProperty>> selector)
        {
            var select = LinqToOdata.Select(selector);
            if (this.select != null)
            {
                select = new Select(this.select.SelectItems.Concat(select.SelectItems));
            }

            return new CalendarContext(this.graphClient, this.calendarUri, select);
        }
    }

    public static class LinqToOdata
    {
        public static Select Select<TType, TProperty>(Expression<Func<TType, TProperty>> selector)
        {
            if (selector.Body is MemberExpression memberExpression)
            {
                return TraverseSelect<TType>(memberExpression);
            }
            else
            {
                throw new Exception("TODO only member expressions are allowed");
            }
        }

        private static Select TraverseSelect<TType>(MemberExpression expression, IEnumerable<MemberExpression> previousExpressions)
        {
            if (expression.Expression?.NodeType != ExpressionType.Parameter)
            {
                if (expression.Expression is MemberExpression memberExpression)
                {
                    return TraverseSelect<TType>(memberExpression, previousExpressions.Append(expression));
                }
                else
                {
                    throw new Exception("TODO i don't think you can actually get here");
                }
            }
            else
            {
                var propertyNames = GetPropertyNames<TType>();
                if (propertyNames.Contains(expression.Member.Name))
                {
                }
                else
                {
                    throw new Exception("TODO property name not found; you shouldn't be able to get here from the expression<Func>, but you could get here if the memberexpression was manually instantiated");
                }
            }
        }

        private static IEnumerable<string> GetPropertyNames<TType>()
        {
            if (typeof(TType) == typeof(Calendar))
            {
                yield return nameof(Calendar.Id);
                yield return nameof(Calendar.Events);
            }
            else if (typeof(TType) == typeof(Event))
            {
                yield return nameof(Event.Id);
                yield return nameof(Event.Body);
                yield return nameof(Event.End);
                yield return nameof(Event.IsCancelled);
                yield return nameof(Event.ResponseStatus);
                yield return nameof(Event.Start);
                yield return nameof(Event.Subject);
                yield return nameof(Event.Type);
                yield return nameof(Event.WebLink);
            }
            else if (typeof(TType) == typeof(ItemBody))
            {
                yield return nameof(ItemBody.Content);
            }
            else if (typeof(TType) == typeof(DateTimeTimeZone))
            {
                yield return nameof(DateTimeTimeZone.DateTime);
                yield return nameof(DateTimeTimeZone.TimeZone);
            }
            else if (typeof(TType) == typeof(ResponseStatus))
            {
                yield return nameof(ResponseStatus.Response);
                yield return nameof(ResponseStatus.Time);
            }
            else
            {
                throw new Exception("TODO actually implement this in a general way");
            }
        }
    }
}
