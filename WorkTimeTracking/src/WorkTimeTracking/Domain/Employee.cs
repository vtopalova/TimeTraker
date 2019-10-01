using System;
using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Domain
{
    public class Employee : IBookedRecords
    {
        public string Name { get; set; }
        public int Sequence { get; set; }
        public DateTime Date { get; set; }

        public Employee(DateTime date, string name, int sequence)
        {
            Date = date;
            Name = name;
            Sequence = sequence;
        }
    }
}
