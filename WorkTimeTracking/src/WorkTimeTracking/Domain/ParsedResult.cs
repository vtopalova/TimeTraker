using System.Collections.Generic;
using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Domain
{
    internal class ParsedResult
    {
        public IList<IBookedRecords> BookedRecords { get; set; } = new List<IBookedRecords>();

        public IResult Result { get; set; } = new SuccessfulResult();
    }
}
