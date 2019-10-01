namespace WorkTimeTracking.Abstractions
{
    internal interface IConsoleLogger
    {
        void Error(string message);

        void Info(string message, bool isNewLine = true);

        void Warning(string message);
    }
}
