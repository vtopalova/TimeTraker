using System;
using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class OverlappedMeetingError : IResult
    {
        public string Message { get; set; }

        public ExitCode Code { get; set; }

        public OverlappedMeetingError(DateTime meetingDate)
        {
            Message = $"The meeting on {meetingDate} is overlapping another meeting.";
            Code = ExitCode.OverlappedMeeting;
        }
    }
}
