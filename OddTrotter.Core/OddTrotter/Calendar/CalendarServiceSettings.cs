using System;

namespace OddTrotter.Calendar
{
    public sealed class CalendarServiceSettings
    {
        private CalendarServiceSettings(TimeSpan lookahead)
        {
            this.LookAhead = lookahead;
        }

        public TimeSpan LookAhead { get; }

        public sealed class Builder
        {
            public TimeSpan LookAhead { get; set; } = TimeSpan.FromDays(365);

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="LookAhead"/> is not a positive value</exception>
            public CalendarServiceSettings Build()
            {
                if (this.LookAhead <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.LookAhead), $"The lookahead must be greater than 0; the provided value was '{this.LookAhead}'");
                }

                return new CalendarServiceSettings(this.LookAhead);
            }
        }
    }
}
