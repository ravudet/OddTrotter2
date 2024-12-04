﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;

    public sealed class CalendarEvent
    {
        public CalendarEvent(string id, string subject, string body, DateTimeOffset start, bool isCancelled)
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
            IsCancelled = isCancelled;
        }

        public string Id { get; }

        public string Subject { get; }

        public string Body { get; }

        public DateTimeOffset Start { get; }

        public bool IsCancelled { get; }
    }

    /// <summary>
    /// TODO better name
    /// </summary>
    public sealed class CalendarEventBuilder
    {
        public string? Id { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        public string? Start { get; set; }

        public bool? IsCancelled { get; set; }
    }
}
