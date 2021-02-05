using Discord;
using System;

namespace YoshikoBot {
    public static class Logger {

        private const ConsoleColor DEFAULT_COLOR = ConsoleColor.Gray;

        public static void Log(LogSeverity severity, string message) {

            PrintSeverity();

            Console.ForegroundColor = DEFAULT_COLOR;
            Console.WriteLine(message);

            #region Local_Function

            void PrintSeverity() {
                Console.ForegroundColor = SeverityToColor(severity);
                Console.Write(Enum.GetName(typeof(LogSeverity), severity) + ": ");
            }

            #endregion
        }

        public static void Log(LogMessage logMessage) {
            string message = $"{logMessage.Message} @ {logMessage.Source}";
            Log(logMessage.Severity, message);
        }

        private static ConsoleColor SeverityToColor(LogSeverity severity) {
            switch (severity) {
                case LogSeverity.Critical:
                    return ConsoleColor.Red;
                case LogSeverity.Error:
                    return ConsoleColor.DarkRed;
                case LogSeverity.Warning:
                    return ConsoleColor.DarkYellow;
                case LogSeverity.Info:
                    return ConsoleColor.White;
                case LogSeverity.Debug:
                    return ConsoleColor.DarkBlue;
            }

            return ConsoleColor.Gray;
        }
    }
}
