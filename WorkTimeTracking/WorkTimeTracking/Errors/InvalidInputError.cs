using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class InvalidInputError : IResult
    {
        public string Message { get; set; }

        public ExitCode Code { get; set; }

        public InvalidInputError(string message)
        {
            Message = message;
            Code = ExitCode.InvalidInput;
        }
    }
}
