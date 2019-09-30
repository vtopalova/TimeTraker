using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class OverlappedMeetingException : ApplicationException
    {
        public OverlappedMeetingException(string message) : base(message, ExitCode.OverlappedMeeting)
        {
        }
    }
}
