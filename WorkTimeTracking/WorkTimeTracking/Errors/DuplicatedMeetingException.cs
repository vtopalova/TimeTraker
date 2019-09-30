using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class DuplicatedMeetingException : ApplicationException
    {
        public DuplicatedMeetingException(string message) : base(message, ExitCode.DuplicateMeeting)
        {
        }
    }
}