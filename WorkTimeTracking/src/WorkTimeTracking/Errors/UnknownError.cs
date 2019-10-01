using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class UnknownError : IResult
    {
        public string Message { get; set; }

        public ExitCode Code { get; set; }

        public UnknownError(string message)
        {
            Message = message;
            Code = ExitCode.UnknownError;
        }
    }
}
