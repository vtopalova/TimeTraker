using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking
{
    internal class SuccessfulResult : IResult
    {
        public string Message { get; set; }

        public ExitCode Code { get; set; }

        public SuccessfulResult()
        {
            Code = ExitCode.Success;
        }
    }
}
