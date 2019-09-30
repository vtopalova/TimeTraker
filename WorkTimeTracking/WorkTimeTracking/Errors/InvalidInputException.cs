using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class InvalidInputException : ApplicationException
    {
        public InvalidInputException(string message) : base(message, ExitCode.InvalidInput)
        {
        }
    }
}
