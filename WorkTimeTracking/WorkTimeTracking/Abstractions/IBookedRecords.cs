using System;

namespace WorkTimeTracking.Abstractions
{
    internal interface IBookedRecords
    {
        int Sequence { get; set; }

        DateTime Date { get; set; }
    }
}
