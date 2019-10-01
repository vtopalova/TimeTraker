using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class InvalidOfficeHoursError : IResult
    {
        public string Message { get; set; }

        public ExitCode Code { get; set; }

        public InvalidOfficeHoursError(string message)
        {
            Message = message;
            Code = ExitCode.InvalidOfficeHours;
        }
    }
}
