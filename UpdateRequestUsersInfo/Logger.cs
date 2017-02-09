using System;

namespace UpdateUsersLogins
{
    public class ConsoleLogger: ILogger
    {
        public void Log(string message, LogLevel logLevel)
        {
            var foregroundColor = Console.ForegroundColor;
            switch (logLevel)
            {
                case LogLevel.Notice:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("logLevel", logLevel, null);
            }
            Console.WriteLine(message);
            Console.ForegroundColor = foregroundColor;
        }
    }
}
