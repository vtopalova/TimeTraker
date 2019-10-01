using Microsoft.Extensions.DependencyInjection;
using System;
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
            var workTimeService = serviceProvider.GetService<IWorkTimeService>();
            var errorResolver = serviceProvider.GetService<IErrorResolver>();

            try
            {
                if (args.Length == 1)
                {
                    var parsedContent = workTimeService.ParseInput(args[0]);

                    if (parsedContent != null)
                    {
                        workTimeService.ValidateContent(parsedContent);

                        workTimeService.CreateOutput(parsedContent);
                    }
                }
                else
                {
                    consoleLogger.Info(string.Empty.PadRight(100, '-'));
                    consoleLogger.Info("Please run the application with the path to your input file as a parameter: dotnet run {inputFile}");
                    consoleLogger.Info(" ");
                    consoleLogger.Info("The Result will be presented on the console.");
                    consoleLogger.Info(" ");
                    consoleLogger.Info("If you want the result in a file, please run the application with the output file's path as a parameter: dotnet run {inputFile} > {outputFile} ");
                    consoleLogger.Info(string.Empty.PadRight(100, '-'));
                }
            }
            catch (Exception ex)
            {
                errorResolver.Resolve(new UnknownError(ex.Message));
            }
        }
    }
}
