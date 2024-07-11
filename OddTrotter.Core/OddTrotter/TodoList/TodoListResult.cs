namespace OddTrotter.TodoList
{
    using System;
    using System.Collections.Generic;

    public sealed class TodoListResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="todoList"></param>
        /// <param name="brokenNextLink"></param>
        /// <param name="eventsWithoutStarts"></param>
        /// <param name="eventsWithStartParseFailures"></param>
        /// <param name="eventsWithoutBodies"></param>
        /// <param name="eventsWithBodyParseFailures"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="todoList"/> or <paramref name="eventsWithoutStarts"/> or <paramref name="eventsWithStartParseFailures"/> or
        /// <paramref name="eventsWithoutBodies"/> or <paramref name="eventsWithBodyParseFailures"/> is <see langword="null"/>
        /// </exception>
        public TodoListResult(
            string todoList,
            DateTime startTimestamp,
            DateTime endTimestamp,
            string? brokenNextLink,
            IEnumerable<CalendarEvent> eventsWithoutStarts,
            IEnumerable<(CalendarEvent, Exception)> eventsWithStartParseFailures,
            IEnumerable<CalendarEvent> eventsWithoutBodies,
            IEnumerable<(CalendarEvent, Exception)> eventsWithBodyParseFailures)
        {
            if (todoList == null)
            {
                throw new ArgumentNullException(nameof(todoList));
            }

            if (eventsWithoutStarts == null)
            {
                throw new ArgumentNullException(nameof(eventsWithoutStarts));
            }

            if (eventsWithStartParseFailures == null)
            {
                throw new ArgumentNullException(nameof(eventsWithStartParseFailures));
            }

            if (eventsWithoutBodies == null)
            {
                throw new ArgumentNullException(nameof(eventsWithoutBodies));
            }

            if (eventsWithBodyParseFailures == null)
            {
                throw new ArgumentNullException(nameof(eventsWithBodyParseFailures));
            }

            this.TodoList = todoList;
            this.StartTimestamp = startTimestamp;
            this.EndTimestamp = endTimestamp;
            this.BrokenNextLink = brokenNextLink;
            EventsWithoutStarts = eventsWithoutStarts;
            EventsWithStartParseFailures = eventsWithStartParseFailures;
            EventsWithoutBodies = eventsWithoutBodies;
            EventsWithBodyParseFailures = eventsWithBodyParseFailures;
        }

        public string TodoList { get; }

        public DateTime StartTimestamp { get; }

        public DateTime EndTimestamp { get; }

        /// <summary>
        /// Gets the URI of one of three values:
        /// 1. <see langword="null"/> if no errors occurred retrieve any of the data
        /// 2. The URL of series master entity for which an error occurred while retrieving the instance events
        /// 3. The URL of the nextLink for which an error occurred while retrieving the that URL's page
        /// </summary>
        public string? BrokenNextLink { get; }

        public IEnumerable<CalendarEvent> EventsWithoutStarts { get; }

        public IEnumerable<(CalendarEvent, Exception)> EventsWithStartParseFailures { get; }

        public IEnumerable<CalendarEvent> EventsWithoutBodies { get; }

        public IEnumerable<(CalendarEvent, Exception)> EventsWithBodyParseFailures { get; }
    }
}