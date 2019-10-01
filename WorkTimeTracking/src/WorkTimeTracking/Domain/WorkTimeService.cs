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

        public ParsedResult ParseInput(string filename)
        {
            return ReadFile(filename);
        }

        public void ValidateContent(ParsedResult parsedContent)
        {
            var allEmployees = parsedContent.BookedRecords.Where(d => d.GetType() == typeof(Employee)).Select(e => (Employee)e)
                .ToList();

            var allMeetings = parsedContent.BookedRecords.Where(d => d.GetType() == typeof(Meeting)).Select(e => (Meeting)e)
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

        public void CreateOutput(ParsedResult bookingContent)
        {
            var bookingRecords = bookingContent.BookedRecords.Select(d => d as IBookedRecords)
                          .ToList();

            var dateBookings = bookingRecords.GroupBy(d => d.Date.Date).Select(p => new { Date = p.Key, Bookings = p.Where(k => k.Date.Date == p.Key) });

            foreach (var date in dateBookings)
            {
                _consoleLogger.Info(date.Date.ToString("yyyy-MM-dd"));

                foreach (var records in date.Bookings)
                {
                    if (records is Employee)
                    {
                        var employeeBooking = (Employee)records;
                        _consoleLogger.Info($"{employeeBooking.Date.ToString("HH:mm:ss")}", false);
                        _consoleLogger.Info(" ", false);
                        _consoleLogger.Info($"{employeeBooking.Name}");
                    }
                    else
                    {
                        var meetingBooking = (Meeting)records;
                        _consoleLogger.Info($"{meetingBooking.Date.ToString("HH:mm")}", false);
                        _consoleLogger.Info(" ", false);
                        _consoleLogger.Info($"{meetingBooking.End.ToString("HH:mm")}");
                    }
                }
            }
        }

        private ParsedResult ReadFile(string filename)
        {
            var listInputRecords = new List<InputWorkingRecords>();

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    Console.SetIn(reader);
                    string line;
                    while ((line = Console.ReadLine()) != null)
                    {
                        listInputRecords.Add(new InputWorkingRecords { Line = line });
                    }
                }
            }
            catch (Exception ex)
            {
                _errorResolver.Resolve(new UnknownError(ex.Message));
            }

            return ParseContent(listInputRecords);
        }

        private ParsedResult ParseContent(IList<InputWorkingRecords> listRecords)
        {
            var parsedResult = new ParsedResult();

            CultureInfo provider = CultureInfo.InvariantCulture;
            int lineCounter = 1;

            foreach (var record in listRecords)
            {
                var sections = record.Line.Split(" ");

                if (lineCounter == 1)
                {
                    var parseOfficeHours = ParseOfficeHours(sections, provider);
                    if (parseOfficeHours.Code != ExitCode.Success)
                    {
                        parsedResult.Result = parseOfficeHours;
                        return parsedResult;
                    }
                }
                else
                {
                    var parsedContent = ParseBookingContent(sections, lineCounter);
                    if (parsedContent.Result.Code != ExitCode.Success)
                    {
                        return parsedContent;
                    }
                    else
                    {
                        parsedResult.BookedRecords.Add(parsedContent.BookedRecords.First());
                    }
                }
                lineCounter++;
            }

            return parsedResult;
        }

        private IResult ParseOfficeHours(string[] officeHours, CultureInfo provider)
        {
            if (officeHours.Length != 2)
            {
                var error = new InvalidOfficeHoursError(ErrorMessages.InvalidOfficeHours);
                _errorResolver.Resolve(error);

                return error;
            }

            var format = "HHmm";
            if (DateTime.TryParseExact(officeHours[0], format, provider, DateTimeStyles.None, out var startTime))
            {
                OfficeHours.Start = startTime.TimeOfDay;
            }
            else
            {
                var errorStartHours = new InvalidOfficeHoursError(string.Format(ErrorMessages.InvalidStartHours, officeHours[0]));
                _errorResolver.Resolve(errorStartHours);

                return errorStartHours;
            }
            if (DateTime.TryParseExact(officeHours[1], format, provider, DateTimeStyles.None, out var endTime))
            {
                OfficeHours.End = endTime.TimeOfDay;
            }
            else
            {
                var errorEndHours = new InvalidOfficeHoursError(string.Format(ErrorMessages.InvalidEndHours, officeHours[1]));
                _errorResolver.Resolve(errorEndHours);

                return errorEndHours;
            }

            return new SuccessfulResult();
        }

        private ParsedResult ParseBookingContent(string[] content, int lineCounter)
        {
            var result = new ParsedResult();

            var inputDate = string.Concat(content[0], " ", content[1]);

            if (!DateTime.TryParse(inputDate, out var date))
            {
                var parseError = new InvalidDate(string.Format(ErrorMessages.InvalidDate, inputDate, lineCounter));
                _errorResolver.Resolve(parseError);
                result.Result = parseError;
                return result;
            }

            string dateString = content[0] + " " + content[1];

            result = TryParseMeetingRecord(dateString, content[2], lineCounter);

            if (result.Result.Code == ExitCode.InvalidDate)
            {
                result = TryParseEmployeeRecord(dateString, content[2], lineCounter);
                if (result.Result.Code != ExitCode.Success)
                {
                    _errorResolver.Resolve(result.Result);
                }
            }

            return result;
        }

        private ParsedResult TryParseMeetingRecord(string dateString, string bookedDuration, int lineCounter)
        {
            var result = new ParsedResult();

            string formatMeeting = "yyyy-MM-dd HH:mm";

            if (DateTime.TryParseExact(dateString, formatMeeting, CultureInfo.InvariantCulture, DateTimeStyles.None, out var theDate))
            {
                var parseDuration = int.TryParse(bookedDuration, out var duration);
                if (!parseDuration)
                {
                    var errorDuration = new InvalidDuration(string.Format(ErrorMessages.InvalidMeetingDuration, bookedDuration, lineCounter));
                    _errorResolver.Resolve(errorDuration);
                    result.Result = errorDuration;
                }
                else
                {
                    result.BookedRecords.Add(new Meeting(theDate, duration, lineCounter));
                }
            }
            else
            {
                result.Result = new InvalidDate(ErrorMessages.InvalidDate);
            }

            return result;
        }

        private ParsedResult TryParseEmployeeRecord(string dateString, string employeeAcr, int lineCounter)
        {
            string formatEmployee = "yyyy-MM-dd HH:mm:ss";
            var result = new ParsedResult();

            if (!DateTime.TryParseExact(dateString, formatEmployee, CultureInfo.InvariantCulture, DateTimeStyles.None, out var theDate))
            {
                var invalidEmployeeDateError = new InvalidDate(string.Format(ErrorMessages.InvalidDate, dateString, lineCounter));
                result.Result = invalidEmployeeDateError;
            }
            else
            {
                result.BookedRecords.Add(new Employee(theDate, employeeAcr, lineCounter));
            }
            return result;
        }
    }
}
