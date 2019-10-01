namespace WorkTimeTracking.Abstractions
{
    enum ExitCode
    {
        Success = 0,
        InvalidInput = 1,
        DuplicateEmployeeDate = 2,
        OverlappedMeeting = 3,
        InvalidOfficeHours = 4,
        UnknownError = 10
    }
}
