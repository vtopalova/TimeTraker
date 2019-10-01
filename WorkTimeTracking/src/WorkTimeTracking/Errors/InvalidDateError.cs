using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class InvalidDate : IResult
    {
        public string Message { get; set; }

        public ExitCode Code { get; set; }

        public InvalidDate(string message)
        {
            Message = message;
            Code = ExitCode.InvalidDate;
        }
    }
}
