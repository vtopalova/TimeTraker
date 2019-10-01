using System;
using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Errors
{
    internal class ErrorResolver : IErrorResolver
    {
        private readonly IConsoleLogger _consoleLogger;

        public ErrorResolver(IConsoleLogger consoleLogger)
        {
            _consoleLogger = consoleLogger;
        }

        public void Resolve(IResult result)
        {
            _consoleLogger.Error(result.Message);

            Environment.Exit((int) result.Code);
        }
    }
}
