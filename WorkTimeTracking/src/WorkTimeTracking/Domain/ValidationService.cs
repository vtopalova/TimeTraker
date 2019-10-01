using System;
using System.Collections.Generic;
using System.Linq;
using WorkTimeTracking.Abstractions;
using WorkTimeTracking.Errors;

namespace WorkTimeTracking.Domain
{
    internal class ValidationService : IValidationService
    {
        private readonly IErrorResolver _errorResolver;

        public ValidationService(IErrorResolver errorResolver)
        {
            _errorResolver = errorResolver;
        }

        public IResult ValidateNoDuplicatedEmployeeDate(IList<Employee> employees)
        {
            var duplicateEmployee = employees.GroupBy(e => e.Date).Where(group => group.Count() > 1).ToList();
            if (duplicateEmployee.Any())
            {
                var error = new DuplicateEmployeeDateError(duplicateEmployee.First().Key);
                _errorResolver.Resolve(error);
                return error;
            }

            return new SuccessfulResult();
        }

        public IResult ValidateNoOverlappedMeetings(IList<Meeting> meetings)
        {
            //grouping the meetings by date.
            var query = meetings.GroupBy(m => m.Date.Date);

            //going throw all grouped dates.
            foreach (var date in query)
            {
                //getting all meetings for certain date and order them by time.
                var dateMeetings = meetings.Where(d => d.Date.Date == date.Key).OrderBy(p => p.Date).ToList();

                //setting the next available time based on the end time of the first meeting of the day.
                var nextAvailableTime = dateMeetings.First().End;

                int meetingCounter = 1;

                //going throw all meetings for the day.
                foreach (var meeting in dateMeetings)
                {
                    //Skipping the first meeting of the day.
                    if (meetingCounter > 1)
                    {
                        if (meeting.Date < nextAvailableTime)
                        {
                            var error = new OverlappedMeetingError(meeting.Date);
                            _errorResolver.Resolve(error);

                            return error;
                        }
                        //Set the end of the current meeting as a value for the next available time.
                        else
                        {
                            nextAvailableTime = meeting.End;
                        }
                    }

                    meetingCounter++;
                }
            }

            return new SuccessfulResult();
        }
    }
}
