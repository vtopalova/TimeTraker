using System;
using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class ApplicationException : Exception
    {
        public ExitCode Code { get; set; }

        public ApplicationException(string message, ExitCode code) : base(message)
        {
            Code = code;
        }
    }
}
