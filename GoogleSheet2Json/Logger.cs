using System;
using System.Collections.Generic;

namespace GoogleSheet2Json
{
    public static class Logger
    {
        public enum LogType
        {
            Info,
            Warning,
            Error
        }
        
        private static readonly Dictionary<LogType, ConsoleColor> logColors = new Dictionary<LogType, ConsoleColor>()
        {
            { LogType.Info,    ConsoleColor.White },
            { LogType.Warning, ConsoleColor.Yellow },
            { LogType.Error,   ConsoleColor.Red }
        };
        
        public static void Log(string message, LogType logType = LogType.Info)
        {
            WriteToConsole(message, logType);
        }
        
        public static void LogLine(string message, LogType logType = LogType.Info)
        {
            WriteLineToConsole(message, logType);
        }
        
        public static void DebugLog(string message, LogType logType = LogType.Info)
        {
            #if DEBUG
            WriteToConsole(message, logType);      
            #endif
        }
        
        public static void DebugLogLine(string message, LogType logType = LogType.Info)
        {
            #if DEBUG
            WriteLineToConsole(message, logType);
            #endif
        }

        private static void WriteLineToConsole(string message, LogType logType)
        {
            ConsoleColor logColor;
            logColors.TryGetValue(logType, out logColor);
            Console.ForegroundColor = logColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        
        private static void WriteToConsole(string message, LogType logType)
        {
            ConsoleColor logColor;
            logColors.TryGetValue(logType, out logColor);
            Console.ForegroundColor = logColor;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}