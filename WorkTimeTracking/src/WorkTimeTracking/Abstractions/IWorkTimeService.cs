using System.Collections.Generic;
using WorkTimeTracking.Domain;

namespace WorkTimeTracking.Abstractions
{
    internal interface IWorkTimeService
    {
        ParsedResult ParseInput(string inputFile);

        void ValidateContent(ParsedResult parsedContent);

        void CreateOutput(ParsedResult parsedContent);
    }
}
