using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class InvalidDuration : IResult
    {
        public string Message { get; set; }

        public ExitCode Code { get; set; }

        public InvalidDuration(string message)
        {
            Message = message;
            Code = ExitCode.InvalidDuration;
        }
    }
}
