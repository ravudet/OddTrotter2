////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;

    public sealed class CalendarEvent
    {
        public CalendarEvent(string id, string subject, string body, DateTimeOffset start)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            //// TODO are subject and body nullable? i think maybe empty makes sense, but not null?

            Id = id;
            Subject = subject;
            Body = body;
            Start = start;
        }

        public string Id { get; }

        public string Subject { get; }

        public string Body { get; }

        public DateTimeOffset Start { get; }
    }

    public sealed class CalendarEventBuilder
    {
        public string? Id { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        public string? Start { get; set; }
    }

    public abstract class Either<TLeft, TRight>
    {
        private Either()
        {
        }

        public sealed class Left : Either<TLeft, TRight>
        {
            public Left(TLeft value)
            {
                Value = value;
            }

            public TLeft Value { get; }
        }

        public sealed class Right : Either<TLeft, TRight>
        {
            public Right(TRight value)
            {
                Value = value;
            }

            public TRight Value { get; }
        }
    }
}
