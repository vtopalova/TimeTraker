using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTimeTracking.Errors
{
   public static class ErrorMessages
    {
        public static string InvalidStartHours = "The start office hour {0} is invalid.";
        public static string InvalidEndHours = "The end office hour {0} is invalid.";
        public static string InvalidOfficeHours = "The first line should contains company office hours, in 24 hour clock format HHmm HHmm";
        public static string InvalidDate = "Invalid date {0} in line {1}.";
        public static string InvalidMeetingDuration = "Invalid meeting's duration {0} in line {1}.";
    }
}
