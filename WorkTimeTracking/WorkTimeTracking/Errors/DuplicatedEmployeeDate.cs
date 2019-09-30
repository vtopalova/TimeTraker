using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class DuplicateEmployeeDate : ApplicationException
    {
        public DuplicateEmployeeDate(string message) : base(message, ExitCode.DuplicateEmployeeDate)
        {
        }
    }
}