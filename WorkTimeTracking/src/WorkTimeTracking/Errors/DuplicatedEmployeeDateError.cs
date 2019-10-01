using System;
using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class DuplicateEmployeeDateError : IResult
    {
        public string Message { get; set; }

        public ExitCode Code { get; set; }

        public DuplicateEmployeeDateError(DateTime duplicatedDate)
        {
            Message = $"The date {duplicatedDate} is duplicated";
            Code = ExitCode.DuplicateEmployeeDate;
        }
    }
}