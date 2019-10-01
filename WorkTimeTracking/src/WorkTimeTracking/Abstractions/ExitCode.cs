namespace WorkTimeTracking.Abstractions
{
    enum ExitCode
    {
        Success = 0,
        InvalidDuration = 1,
        InvalidDate = 2,
        DuplicateEmployeeDate = 3,
        OverlappedMeeting = 4,
        InvalidOfficeHours = 5,
        UnknownError = 500
    }
}
