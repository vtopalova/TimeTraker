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
            var allEmployees = parsedContent.Where(d => d.GetType() == typeof(Employee)).Select(e => (Employee) e)
                .ToList();

            var allMeetings = parsedContent.Where(d => d.GetType() == typeof(Meeting)).Select(e => (Meeting) e)
                .ToList();

            _validationService.ValidateNoDuplicatedEmployeeDate(allEmployees);

            _validationService.ValidateNoOverlappedMeetings(allMeetings);

        }

        public IList<BookingContent> CreateOutput(IList<object> bookingContent)
        {
            return new List<BookingContent>();
            //var sortedList = bookingContent.GroupBy(d => new { d.Date.Day, d.Date.Month, d.Date.Year }).OrderBy(group => group.Min(k => k.Date));

            //foreach (var element in sortedList)
            //{
            //    var date = element.Key;

            //    _consoleLogger.Info($"{date.Year}-{date.Month.ToString("00")}-{date.Day.ToString("00")}");
            //    foreach (var records in element)
            //    {



            //    }

            //}

            //return bookingContent;
        }

        private void ReadConsoleStream()
        {
            var listInputRecords = new List<InputWorkingRecords>();

            string line;
            do
            {
                line = Console.ReadLine();

                if (line != null)
                {
                    listInputRecords.Add(new InputWorkingRecords {Line = line});
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.E);

            ParseContent(listInputRecords);
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
                    listInputRecords.Add(new InputWorkingRecords {Line = line});
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
                    if (sections.Length != 2)
                    {
                        _errorResolver.Resolve(new InvalidInputError(
                            "The first line should contains company office hours, in 24 hour clock format HHmm HHmm"));
                    }

                    var format = "HHmm";
                    var startTime = DateTime.ParseExact(sections[0], format, provider).TimeOfDay;
                    _consoleLogger.Info($"The company start time is {startTime}");

                    var endTime = DateTime.ParseExact(sections[1], format, provider).TimeOfDay;
                    _consoleLogger.Info($"The company end time is {endTime}");

                    var officeHours = endTime - startTime;
                    _consoleLogger.Info($"The company office hours are {officeHours}");
                }
                else
                {
                    var inputDate = string.Concat(sections[0], " ", sections[1]);

                    if (!DateTime.TryParse(inputDate, out var date))
                    {
                        _consoleLogger.Error($"Invalid date {inputDate} in line {lineCounter}.");
                    }

                    parsedResult.Add(new BookingContent
                    {
                        Date = date,
                        Record = sections[2],

                    });

                    string dateString = sections[0] + " " + sections[1]; //"1955-11-05 09:00";
                    string formatMeeting = "yyyy-MM-dd HH:mm";
                    string formatEmployee = "yyyy-MM-dd HH:mm:ss";

                    if (DateTime.TryParseExact(
                        dateString,
                        formatMeeting,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var theDate))
                    {
                        var parseDuration = int.TryParse(sections[2], out var duration);
                        if (!parseDuration)
                        {
                            _errorResolver.Resolve(
                                new InvalidInputError(
                                    $"Invalid meeting's duration {sections[2]} in line {lineCounter}"));
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

                        parsedResult.Add(new Employee(theDate, sections[2], lineCounter));
                    }
                }

                lineCounter++;
            }

            return parsedResult;
        }
    }
}
