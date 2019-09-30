using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using WorkTimeTracking.Abstractions;
using WorkTimeTracking.Domain;
using WorkTimeTracking.Errors;
using Xunit;

namespace WorkTimeTrackingTests
{
    public class ValidationServiceTests
    {
        private IErrorResolver _errorResolver;
        private IValidationService _validationService;

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

        public ValidationServiceTests()
        {
            var errorResolver = Substitute.For<IErrorResolver>();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(s => errorResolver)
                .AddSingleton<IValidationService, ValidationService>()
                .BuildServiceProvider();

            _errorResolver = errorResolver;
            _validationService = serviceProvider.GetService<IValidationService>();
        }

        [Fact]
        public void ValidateNoDuplicatedEmployeeDateSuccess()
        {
            _validationService.ValidateNoDuplicatedEmployeeDate(_employees);

            _errorResolver.DidNotReceive().Resolve(Arg.Any<IResult>());
        }

        [Fact]
        public void DuplicateEmployeeRecordsCallsErrorResolver()
        {
            var duplicatedDate = DateTime.Now;

            _employees.Add(new Employee(duplicatedDate, "EMP001", 1));
            _employees.Add(new Employee(duplicatedDate, "EMP001", 2));

            _validationService.ValidateNoDuplicatedEmployeeDate(_employees);

            _errorResolver.Received().Resolve(Arg.Any<IResult>());
        }

        [Fact]
        public void DuplicateEmployeeRecordsReturnsError()
        {
            var duplicatedDate = DateTime.Now;

            _employees.Add(new Employee(duplicatedDate, "EMP001", 1));
            _employees.Add(new Employee(duplicatedDate, "EMP001", 2));

            var expectedError = new DuplicateEmployeeDateError(duplicatedDate);

            var result = _validationService.ValidateNoDuplicatedEmployeeDate(_employees);

            Assert.Equal(expectedError.Code, result.Code);
            Assert.Equal(expectedError.Message, result.Message);
        }

        [Fact]
        public void DuplicateEmployeeRecordsDateCallsErrorResolver()
        {
            var duplicatedDate = DateTime.Now;

            _employees.Add(new Employee(duplicatedDate, "EMP001", 1));
            _employees.Add(new Employee(duplicatedDate, "EMP002", 2));

            _validationService.ValidateNoDuplicatedEmployeeDate(_employees);

            _errorResolver.Received().Resolve(Arg.Any<IResult>());
        }

        [Fact]
        public void DuplicateEmployeeRecordsDateReturnsError()
        {
            var duplicatedDate = DateTime.Now;

            _employees.Add(new Employee(duplicatedDate, "EMP001", 1));
            _employees.Add(new Employee(duplicatedDate, "EMP002", 2));

            var expectedError = new DuplicateEmployeeDateError(duplicatedDate);

            var result = _validationService.ValidateNoDuplicatedEmployeeDate(_employees);

            Assert.Equal(expectedError.Code, result.Code);
            Assert.Equal(expectedError.Message, result.Message);
        }

        [Fact]
        public void ValidateNoOverlappedMeetingsSuccess()
        {
            _validationService.ValidateNoOverlappedMeetings(_meetings);

            _errorResolver.DidNotReceive().Resolve(Arg.Any<IResult>());
        }

        [Fact]
        public void DuplicateMeetingsCallsErrorResolver()
        {
            var duplicatedDate = DateTime.Now;

            _meetings.Add(new Meeting(duplicatedDate, 3, 1));
            _meetings.Add(new Meeting(duplicatedDate, 3, 2));

            _validationService.ValidateNoOverlappedMeetings(_meetings);

            _errorResolver.Received().Resolve(Arg.Any<IResult>());
        }

        [Fact]
        public void DuplicateMeetingsReturnsError()
        {
            var duplicatedDate = DateTime.Now;

            _meetings.Add(new Meeting(duplicatedDate, 3, 1));
            _meetings.Add(new Meeting(duplicatedDate, 3, 2));
          
            var expectedError = new OverlappedMeetingError(duplicatedDate);

            var result = _validationService.ValidateNoOverlappedMeetings(_meetings);

            Assert.Equal(expectedError.Code, result.Code);
            Assert.Equal(expectedError.Message, result.Message);
        }

        [Fact]
        public void OverlappedMeetingCallsErrorResolver()
        {
            var duplicatedDate = DateTime.Now;

            _meetings.Add(new Meeting(duplicatedDate, 3, 1));
            _meetings.Add(new Meeting(duplicatedDate.AddHours(1), 3, 2));

            _validationService.ValidateNoOverlappedMeetings(_meetings);

            _errorResolver.Received().Resolve(Arg.Any<IResult>());
        }

        [Fact]
        public void OverlappedMeetingReturnsError()
        {
            var duplicatedDate = DateTime.Now.AddDays(1);
            
            var overlappingMeeting = new Meeting(duplicatedDate.AddHours(1), 3, 2);

            _meetings.Add(overlappingMeeting);
            _meetings.Add(new Meeting(duplicatedDate, 3, 1));

            var expectedError = new OverlappedMeetingError(overlappingMeeting.Date);

            var result = _validationService.ValidateNoOverlappedMeetings(_meetings);

            Assert.Equal(expectedError.Code, result.Code);
            Assert.Equal(expectedError.Message, result.Message);
        }
    }
}
