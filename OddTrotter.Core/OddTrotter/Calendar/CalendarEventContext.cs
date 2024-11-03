////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;

    using OddTrotter.GraphClient;

    public sealed class CalendarEventContext : IQueryContext<Either<CalendarEvent, CalendarEventBuilder>, object>
    {
        private readonly IGraphClient graphClient;
        private readonly RelativeUri calendarUri;
        private readonly DateTime startTime;
        private readonly DateTime endTime;
        private readonly int pageSize;

        public CalendarEventContext(IGraphClient graphClient, RelativeUri calendarUri, DateTime startTime, DateTime endTime, CalendarEventContextSettings settings)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri;
            this.startTime = startTime;
            this.endTime = endTime;
            this.pageSize = settings.PageSize;
        }

        public QueryResult<Either<CalendarEvent, CalendarEventBuilder>, object> Evaluate()
        {
            throw new System.NotImplementedException();
        }
    }
}
