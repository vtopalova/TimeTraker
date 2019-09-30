using System.Collections.Generic;
using WorkTimeTracking.Domain;

namespace WorkTimeTracking.Abstractions
{
    internal interface IValidationService
    {
        IResult ValidateNoDuplicatedEmployeeDate(IList<Employee> employees);

        IResult ValidateNoOverlappedMeetings(IList<Meeting> meetings);
    }
}
