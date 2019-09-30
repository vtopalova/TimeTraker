using Microsoft.Extensions.DependencyInjection;
using WorkTimeTracking.Abstractions;
using WorkTimeTracking.Domain;
using WorkTimeTracking.Errors;

namespace WorkTimeTracking
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConsoleLogger, ConsoleLogger>()
                .AddSingleton<IWorkTimeService, WorkTimeService>()
                .AddSingleton<IErrorResolver, ErrorResolver>()
                .AddSingleton<IValidationService, ValidationService>()
                .BuildServiceProvider();

            var consoleLogger = serviceProvider.GetService<IConsoleLogger>();
            consoleLogger.Info("Please add your information");

            consoleLogger.Info("Enter one or more lines of text (press CTRL+E to start process):");

            var workTimeService = serviceProvider.GetService<IWorkTimeService>();

            if (args.Length == 1)
            {
                var parsedContent = workTimeService.ParseInput(args[0]);

                workTimeService.ValidateContent(parsedContent);

                workTimeService.CreateOutput(parsedContent);
            }
        }
    }
}
