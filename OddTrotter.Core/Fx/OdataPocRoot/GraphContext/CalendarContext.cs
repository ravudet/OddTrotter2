namespace Fx.OdataPocRoot.GraphContext
{
    using System;
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

        private readonly string? select;

        public CalendarContext(IGraphClient graphClient, RelativeUri calendarUri)
            : this(graphClient, calendarUri, null)
        {
        }

        private CalendarContext(IGraphClient graphClient, RelativeUri calendarUri, string? select)
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
            throw new System.NotImplementedException();
        }
    }

    public static class LinqToOdata
    {
        public static Select Select<TType, TProperty>(Expression<Func<TType, TProperty>> selector)
        {
            return new Select(Enumerable.Empty<SelectItem>());
        }
    }
}
