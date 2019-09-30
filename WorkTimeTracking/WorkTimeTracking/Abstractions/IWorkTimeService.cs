using System.Collections.Generic;
using WorkTimeTracking.Domain;

namespace WorkTimeTracking.Abstractions
{
    internal interface  IWorkTimeService
    {
        IList<object> ParseInput(string filename);

        void ValidateContent(IList<object> parsedContent);

        IList<BookingContent> CreateOutput(IList<object> parsedContent);
    }
}
