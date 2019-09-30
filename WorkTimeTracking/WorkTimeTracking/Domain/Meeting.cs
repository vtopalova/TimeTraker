using System;
using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Domain
{
    internal class Meeting : IBookedRecords
    {
        public DateTime Date { get; set; }

        public int Duration { get; set; }

        public DateTime End { get; }

        public int Sequence { get; set; }


        public Meeting(DateTime date, int duration, int sequence)
        {
            Date = date;
            Duration = duration;
            End = date.AddHours(duration);
            Sequence = sequence;
        }
    }
}
