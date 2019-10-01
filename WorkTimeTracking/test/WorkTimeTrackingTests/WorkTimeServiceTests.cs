using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using WorkTimeTracking.Abstractions;
using WorkTimeTracking.Domain;
using WorkTimeTracking.Errors;
using Xunit;

namespace WorkTimeTrackingTests
{
    public class WorkTimeServiceTests
    {
        private IErrorResolver _errorResolver;
        private IValidationService _validationService;
        private IWorkTimeService _workTimeService;
        private string _path;

        private IList<Employee> _employees = new List<Employee>
        {
            new Employee(DateTime.Now, "EMP001", 1),
            new Employee(DateTime.Now.AddMinutes(-5), "EMP001", 2)

        };

        private IList<Meeting> _meetings = new List<Meeting>
        {
            new Meeting(DateTime.Now, 2, 1),
            new Meeting(DateTime.Now.AddHours(2), 3, 2)

        };

        public string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public WorkTimeServiceTests()
        {
            var errorResolver = Substitute.For<IErrorResolver>();
            var validationService = Substitute.For<IValidationService>();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(s => errorResolver)
                .AddSingleton(s => validationService)
                .AddSingleton<IWorkTimeService, WorkTimeService>()
                .AddSingleton<IConsoleLogger, ConsoleLogger>()
                .BuildServiceProvider();

            _errorResolver = errorResolver;
            _validationService = validationService;
            _workTimeService = serviceProvider.GetService<IWorkTimeService>();

            _path = GetApplicationRoot();
        }

        [Fact]
        public void ValidateContentInvokesValidateNoDuplicatedEmployeeDate()
        {
            IList<IBookedRecords> content = new List<IBookedRecords> { _employees.First()};
            var parsedResult = new ParsedResult() { BookedRecords = content };

            _workTimeService.ValidateContent(parsedResult);

            _validationService.Received().ValidateNoDuplicatedEmployeeDate(Arg.Any<IList<Employee>>());

            _errorResolver.DidNotReceive().Resolve(Arg.Any<IResult>());
        }

        [Fact]
        public void ValidateContentInvokesValidateNoOverlappedMeetings()
        {
            IList<IBookedRecords> content = new List<IBookedRecords> { _meetings.First() };
            var parsedResult = new ParsedResult { BookedRecords = content };

            _workTimeService.ValidateContent(parsedResult);

            _validationService.Received().ValidateNoOverlappedMeetings(Arg.Any<IList<Meeting>>());

            _errorResolver.DidNotReceive().Resolve(Arg.Any<IResult>());
        }

        [Fact]
        public void ParseContentInvalidOfficeHours()
        {
            _workTimeService.ParseInput($"{_path}\\Files\\WrongOfficeHours");

            var error = new InvalidOfficeHoursError(ErrorMessages.InvalidOfficeHours);

            _errorResolver.Received().Resolve(Arg.Is<InvalidOfficeHoursError>(x => x.Code == ExitCode.InvalidOfficeHours && x.Message == error.Message));
        }

        [Fact]
        public void ParseContentInvalidStartOfficeHours()
        {
            _workTimeService.ParseInput($"{_path}\\Files\\InvalidStartOfficeHours");

            var error = new InvalidOfficeHoursError(string.Format(ErrorMessages.InvalidStartHours, "090F"));

            _errorResolver.Received().Resolve(Arg.Is<InvalidOfficeHoursError>(x => x.Code == ExitCode.InvalidOfficeHours && x.Message == error.Message));
        }

        [Fact]
        public void ParseContentInvalidEndOfficeHours()
        {
            _workTimeService.ParseInput($"{_path}\\Files\\InvalidEndOfficeHours");

            var error = new InvalidOfficeHoursError(string.Format(ErrorMessages.InvalidEndHours, "17F0"));

            _errorResolver.Received().Resolve(Arg.Is<InvalidOfficeHoursError>(x => x.Code == ExitCode.InvalidOfficeHours && x.Message == error.Message));
        }

        [Fact]
        public void ParseContentInvalidMeetingDuration()
        {
            _workTimeService.ParseInput($"{_path}\\Files\\InvalidMeetingDuration");

            var error = new InvalidDuration(string.Format(ErrorMessages.InvalidMeetingDuration, "k", 3));

            _errorResolver.Received().Resolve(Arg.Is<InvalidDuration>(x => x.Code == ExitCode.InvalidDuration && x.Message == error.Message));
        }

        [Fact]
        public void ParseContentInvalidDate()
        {
            _workTimeService.ParseInput($"{_path}\\Files\\InvalidDate");

            var error = new InvalidDate(string.Format(ErrorMessages.InvalidDate, "2011-03-k 09:00", 3));

            _errorResolver.Received().Resolve(Arg.Is<InvalidDate>(x => x.Code == ExitCode.InvalidDate && x.Message == error.Message));
        }
    }
}
