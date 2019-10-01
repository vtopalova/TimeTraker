using System;
using WorkTimeTracking.Abstractions;

namespace WorkTimeTracking.Domain
{
    internal class ConsoleLogger : IConsoleLogger
    {
        private string delimiter = string.Empty.PadRight(100, '-');
        public void Error(string message)
        {
            Console.WriteLine(delimiter);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {message}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Info(string message)
        {
            Console.WriteLine(delimiter);
            Console.WriteLine(message);
        }

        public void Warning(string message)
        {
            Console.WriteLine(delimiter);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: {message}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
