using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTimeTracking.Abstractions
{
    internal interface IError
    {
        string Message { get; set; }
        ExitCode Code { get; set; }
    }
}
