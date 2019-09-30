namespace WorkTimeTracking.Abstractions
{
    internal interface IResult
    {
        string Message { get; set; }

        ExitCode Code { get; set; }
    }
}
