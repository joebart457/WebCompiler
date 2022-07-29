namespace WebCompilerServer.Services
{
    public enum LogSeverity
    {
        INFO = 0,
        WARNING = 1,
        ERROR = 2,
        SUCCESS = 3,
        LOGS = 4,
        LIST,
    }
    public static class LoggerService
    {
        public static bool LoggingEnabled { get; set; } = true;

        public static void Log(string msg, LogSeverity severity = LogSeverity.INFO, string guid = "<none>", bool force = false)
        {
            if (LoggingEnabled || force)
            {
                msg = $"[{guid}]: {msg}";
                switch (severity)
                {
                    case LogSeverity.INFO:
                        Write(msg, ConsoleColor.Cyan);
                        break;
                    case LogSeverity.WARNING:
                        Write(msg, ConsoleColor.DarkYellow);
                        break;
                    case LogSeverity.ERROR:
                        Write(msg, ConsoleColor.Red);
                        break;
                    case LogSeverity.SUCCESS:
                        Write(msg, ConsoleColor.Green);
                        break;
                    case LogSeverity.LOGS:
                        Write(msg, ConsoleColor.Yellow);
                        break;
                    case LogSeverity.LIST:
                        Write(msg, ConsoleColor.DarkCyan);
                        break;
                    default:
                        Write(msg, ConsoleColor.Cyan);
                        break;
                }
            }
        }

        public static void Log<Ty>(List<Ty> ls, LogSeverity severity = LogSeverity.LIST, bool force = false) where Ty : class
        {
            Log(ls, new List<string>(), severity, force);
        }
        public static void Log<Ty>(List<Ty> ls, List<string> excludeFields, LogSeverity severity = LogSeverity.LIST, bool force = false) where Ty : class
        {
            if (LoggingEnabled || force)
            {
                if (ls is List<string> data)
                {
                    foreach (var str in data)
                    {
                        Log(str, severity, force: force);
                    }
                    return;
                }
                var headers = typeof(Ty).GetProperties().Where(p => !excludeFields.Contains(p.Name)).Select(p => p.Name);
                Log(string.Join(" | ", headers), severity);
                foreach (var item in ls)
                {
                    var values = typeof(Ty).GetProperties().Where(p => !excludeFields.Contains(p.Name)).Select(prop => $"{prop.GetValue(item)}");
                    Log(string.Join("   ", values), severity);
                }
            }
        }

        private static void Write(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
