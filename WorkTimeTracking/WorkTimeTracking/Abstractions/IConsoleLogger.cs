namespace WorkTimeTracking.Abstractions
{
    internal interface IConsoleLogger
    {
        void Error(string message);

        void Info(string message);

        void Warning(string message);
    }
}
