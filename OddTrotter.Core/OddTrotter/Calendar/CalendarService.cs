namespace OddTrotter.Calendar
{
    using System;
    using System.Threading.Tasks;

    using OddTrotter.GraphClient;

    public sealed class CalendarService
    {
        private readonly IGraphClient graphClient;

        public CalendarService(IGraphClient graphClient)
        {
            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            this.graphClient = graphClient;
        }

        public async Task<TentativeCalendarResult> RetrieveTentativeCalendar()
        {

        }
    }
}
