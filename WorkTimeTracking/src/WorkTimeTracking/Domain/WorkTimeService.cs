using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WorkTimeTracking.Abstractions;
using WorkTimeTracking.Errors;

namespace WorkTimeTracking.Domain
{
    internal class WorkTimeService : IWorkTimeService
    {
        private readonly IConsoleLogger _consoleLogger;
        private readonly IValidationService _validationService;
        private readonly IErrorResolver _errorResolver;

        public WorkTimeService(IConsoleLogger consoleLogger, IValidationService validationService,
            IErrorResolver errorResolver)
        {
            _consoleLogger = consoleLogger;
            _validationService = validationService;
            _errorResolver = errorResolver;
        }

        public IList<object> ParseInput(string filename)
        {
            return ReadFile(filename);
        }

        public void ValidateContent(IList<object> parsedContent)
        {
            var allEmployees = parsedContent.Where(d => d.GetType() == typeof(Employee)).Select(e => (Employee)e)
                .ToList();

            var allMeetings = parsedContent.Where(d => d.GetType() == typeof(Meeting)).Select(e => (Meeting)e)
                .ToList();

            if (allEmployees.Any())
            {
                _validationService.ValidateNoDuplicatedEmployeeDate(allEmployees);
            }

            if (allMeetings.Any())
            {
                _validationService.ValidateNoOverlappedMeetings(allMeetings);
            }

        }

        public IList<BookingContent> CreateOutput(IList<object> bookingContent, string outputFile)
        {
            var allEmployees = bookingContent.Where(d => d.GetType() == typeof(IBookedRecords)).Select(e => (Employee)e)
                          .ToList();

            return new List<BookingContent>();
        }
        private IList<object> ReadFile(string filename)
        {
            var listInputRecords = new List<InputWorkingRecords>();

            using (var reader = new StreamReader(filename))
            {
                Console.SetIn(reader);
                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    listInputRecords.Add(new InputWorkingRecords { Line = line });
                }
            }

            return ParseContent(listInputRecords);
        }

        private IList<object> ParseContent(IList<InputWorkingRecords> listRecords)
        {
            var parsedResult = new List<object>();

            CultureInfo provider = CultureInfo.InvariantCulture;
            int lineCounter = 1;

            foreach (var record in listRecords)
            {
                var sections = record.Line.Split(" ");

                if (lineCounter == 1)
                {
                    ParseOfficeHours(sections, provider);
                }
                else
                {
                    parsedResult.AddRange(ParseBookingContent(sections, lineCounter));
                }

                lineCounter++;
            }

            return parsedResult;
        }

        private void ParseOfficeHours(string[] officeHours, CultureInfo provider)
        {
            if (officeHours.Length != 2)
            {
                _errorResolver.Resolve(new InvalidInputError(
                    "The first line should contains company office hours, in 24 hour clock format HHmm HHmm"));
            }

            var format = "HHmm";
            if (DateTime.TryParseExact(officeHours[0], format, provider, DateTimeStyles.None, out var startTime))
            {
                OfficeHours.Start = startTime.TimeOfDay;
            }
            else
            {
                _errorResolver.Resolve(new InvalidOfficeHoursError($"The start office hours {startTime} are invalid. "));
            }
            if (DateTime.TryParseExact(officeHours[1], format, provider, DateTimeStyles.None, out var endTime))
            {
                OfficeHours.End = endTime.TimeOfDay;
            }
            else
            {
                _errorResolver.Resolve(new InvalidOfficeHoursError($"The end office hours {endTime} are invalid. "));
            }
        }

        private IList<object> ParseBookingContent(string[] content, int lineCounter)
        {
            IList<object> parsedResult = new List<object>();

            var inputDate = string.Concat(content[0], " ", content[1]);

            if (!DateTime.TryParse(inputDate, out var date))
            {
                _consoleLogger.Error($"Invalid date {inputDate} in line {lineCounter}.");
            }

            parsedResult.Add(new BookingContent
            {
                Date = date,
                Record = content[2],

            });

            string dateString = content[0] + " " + content[1];
            string formatMeeting = "yyyy-MM-dd HH:mm";
            string formatEmployee = "yyyy-MM-dd HH:mm:ss";

            if (DateTime.TryParseExact(
                dateString,
                formatMeeting,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var theDate))
            {
                var parseDuration = int.TryParse(content[2], out var duration);
                if (!parseDuration)
                {
                    _errorResolver.Resolve(
                        new InvalidInputError(
                            $"Invalid meeting's duration {content[2]} in line {lineCounter}"));
                }

                parsedResult.Add(new Meeting(theDate, duration, lineCounter));
            }
            else
            {
                if (!DateTime.TryParseExact(
                    dateString,
                    formatEmployee,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out theDate))
                {
                    _errorResolver.Resolve(
                        new InvalidInputError($"Invalid date {dateString} in line {lineCounter}"));
                }

                parsedResult.Add(new Employee(theDate, content[2], lineCounter));
            }

            return parsedResult;
        }
    }
}
