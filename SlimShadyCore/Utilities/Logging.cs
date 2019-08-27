using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SlimShadyCore.Utilities
{
    /**
     * Where should the caller class information be put in the log line (Begin/End)
     */
    public enum CallerLogPosition
    {
        Begin,
        End
    }

    public class LogType
    {
        public static readonly LogType DEBUG = new LogType("DBG");
        public static readonly LogType INFO = new LogType("INF");
        public static readonly LogType WARN = new LogType("WRN");
        public static readonly LogType ERROR = new LogType("ERR");

        public string shortName;

        private LogType(string shortName)
        {
            this.shortName = shortName;
        }
    }

    public abstract class INkLogger
    {
        public void debug(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { debug(msg, new CallerInfo(filePath, memberName, lineNumber)); }
        public void debug(Object msg, CallerInfo caller)
        { log(LogType.DEBUG, msg, caller); }
        public void info(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { info(msg, new CallerInfo(filePath, memberName, lineNumber)); }
        public void info(Object msg, CallerInfo caller)
        { log(LogType.INFO, msg, caller); }
        public void warn(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { warn(msg, new CallerInfo(filePath, memberName, lineNumber)); }
        public void warn(Object msg, CallerInfo caller)
        { log(LogType.WARN, msg, caller); }
        public void error(Object msg, Exception ex, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { error(msg, ex, new CallerInfo(filePath, memberName, lineNumber)); }
        public void error(Object msg, Exception ex, CallerInfo caller)
        { log(LogType.ERROR, msg + "; " + ex, caller); }
        public void error(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { error(msg, new CallerInfo(filePath, memberName, lineNumber)); }
        public void error(Object msg, CallerInfo caller)
        { log(LogType.ERROR, msg, caller); }
        public NkTrace dbgTrace(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { return dbgTrace(msg, new CallerInfo(filePath, memberName, lineNumber)); }
        public NkTrace dbgTrace([CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { return dbgTrace("", new CallerInfo(filePath, memberName, lineNumber)); }
        public NkTrace dbgTrace(Object msg, CallerInfo caller)
        { return new NkTrace(this, LogType.DEBUG, msg, caller); }
        public NkTrace trace(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { return trace(msg, new CallerInfo(filePath, memberName, lineNumber)); }
        public NkTrace trace([CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { return trace("", new CallerInfo(filePath, memberName, lineNumber)); }
        public NkTrace trace(Object msg, CallerInfo caller)
        { return new NkTrace(this, LogType.INFO, msg, caller); }

        public void log(LogType type, Object msg, CallerInfo caller)
        { log(type, "", msg, caller, CallerLogPosition.End); }
        public void log(LogType type, String msgPrefix, Object msg, CallerInfo caller, CallerLogPosition callerPosition)
        { log(new LogLineFormat(type, msgPrefix, msg, caller, callerPosition)); }

        public abstract void log(LogLineFormat logLine);
    }

    public class NkLLogger : INkLogger
    {
        private static readonly ILog proxy = LogManager.GetLogger(typeof(NkLogger));
        public static readonly INkLogger logger = new NkLLogger();

        public NkLLogger()
        {
        }

        public override void log(LogLineFormat logLine)
        {
            if (logLine.type == LogType.DEBUG)
                proxy.Debug(logLine);
            else
                proxy.Info(logLine);
        }
    }

    public class NkLogger
    {
        public static readonly INkLogger logger = new NkLLogger();

        private NkLogger()
        {
            throw new NotSupportedException();
        }

        public static void dbg(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { logger.debug(msg, filePath, memberName, lineNumber); }
        public static void dbg(Object msg, CallerInfo caller)
        { logger.debug(msg, caller); }
        public static void info(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { logger.info(msg, filePath, memberName, lineNumber); }
        public static void info(Object msg, CallerInfo caller)
        { logger.info(msg, caller); } 
        public static void warn(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { logger.warn(msg, filePath, memberName, lineNumber); } 
        public static void warn(Object msg, CallerInfo caller)
        { logger.warn(msg, caller); }
        public static void error(Object msg, Exception ex, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { logger.error(msg, ex, filePath, memberName, lineNumber); }
        public static void error(Object msg, Exception ex, CallerInfo caller)
        { logger.error(msg, ex, caller); }
        public static void error(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { logger.error(msg, filePath, memberName, lineNumber); }
        public static void error(Object msg, CallerInfo caller)
        { logger.error(msg, caller); }
        public static NkTrace dbgTrace(bool csharp_compiler_hax_to_select_below_method_instead_of_this_when_messsage_string_is_passed_as_parameter_do_not_remove_this_parameter = true, 
            [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { return logger.dbgTrace("", filePath, memberName, lineNumber); }
        public static NkTrace dbgTrace(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { return logger.dbgTrace(msg, filePath, memberName, lineNumber); }
        public static NkTrace dbgTrace(Object msg, CallerInfo caller)
        { return logger.dbgTrace(msg, caller); }
        public static NkTrace trace(bool csharp_compiler_hax_to_select_below_method_instead_of_this_when_messsage_string_is_passed_as_parameter_do_not_remove_this_parameter = true, 
            [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { return logger.trace("", filePath, memberName, lineNumber); }
        public static NkTrace trace(Object msg, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        { return logger.trace(msg, filePath, memberName, lineNumber); }
        public static NkTrace trace(Object msg, CallerInfo caller)
        { return logger.trace(msg, caller); }

        public static void Configure()
        {
            PatternLayout layout = new PatternLayout(@"%message%newline");

            RollingFileAppender appender1 = new RollingFileAppender()
            {
                Layout = layout,
                File = "log-DBG.txt",
                AppendToFile = true,
                MaximumFileSize = "10MB",
                DatePattern = "dd.MM.yyyy'.log'",
                MaxSizeRollBackups = 3,
                Threshold = Level.All,
            };
            appender1.ActivateOptions();

            RollingFileAppender appender2 = new RollingFileAppender()
            {
                Layout = layout,
                File = "log-INF.txt",
                AppendToFile = true,
                MaximumFileSize = "3MB",
                DatePattern = "dd.MM.yyyy'.log'",
                MaxSizeRollBackups = 3,
                Threshold = Level.Info,
            };
            appender2.ActivateOptions();

            ConsoleAppender appender3 = new ConsoleAppender()
            {
                Layout = layout,
                Threshold = Level.Info,
            };
            appender3.ActivateOptions();

            BasicConfigurator.Configure(appender1, appender2, appender3);
        }
    }

    public class NkTrace : IDisposable
    {
        private readonly INkLogger logger;
        private LogType type;
        private readonly CallerInfo caller;
        private StringBuilder exitMsg;

        public NkTrace(INkLogger logger, LogType type, Object msg, CallerInfo caller)
        {
            this.logger = logger;
            this.type = type;
            this.caller = caller;

            string prefix = ">> ";
            logger.log(type, prefix, msg, caller, CallerLogPosition.Begin);
            NkLoggerIndent.increment();
        }

        public T appendExit<T>(T msg)
        {
            if (exitMsg == null)
                exitMsg = new StringBuilder();
            else
                exitMsg.Append("; ");
            exitMsg.Append(msg);
            return msg;
        }

        public void appendErrorExit(Object msg)
        {
            type = LogType.ERROR;
            appendExit("ERR: " + msg);
        }

        public void appendWarnExit(Object msg)
        {
            type = LogType.WARN;
            appendExit("WRN: " + msg);
        }

        public void Dispose()
        {
            NkLoggerIndent.decrement();
            String prefix = "<< ";
            logger.log(type, prefix, exitMsg, caller, CallerLogPosition.Begin);
        }
    }

    public class LogLineFormat
    {
        private static readonly bool PRINT_DATE = false;

        public readonly LogType type;
        private readonly string asString;

        public LogLineFormat(LogType type, string prefix, object message, CallerInfo caller, CallerLogPosition callerPosition)
        {
            this.type = type;
            StringBuilder sb = new StringBuilder(128);

            string datetimeFormat;
            if (PRINT_DATE)
                datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            else
                datetimeFormat = "HH:mm:ss.fff";

            sb
                .Append(DateTime.Now.ToString(datetimeFormat))
                .Append(" ")
                .Append(type.shortName)
                .Append(" [" + Thread.CurrentThread.ManagedThreadId + "] ")
                .Append(NkLoggerIndent.getIndent())
                .Append(prefix);

            if (callerPosition == CallerLogPosition.Begin)
                sb.Append(caller).Append(" ");

            sb.Append(message);

            if (callerPosition == CallerLogPosition.End)
                sb.Append(" (").Append(caller).Append(")");

            asString = sb.ToString();
        }

        public override string ToString()
        {
            return asString;
        }
    }

    public class CallerInfo
    {
        public readonly string className;
        public readonly string memberName;
        public readonly int lineNumber;

        public CallerInfo([CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            this.className = Path.GetFileNameWithoutExtension(filePath);
            this.memberName = memberName;
            this.lineNumber = lineNumber;
        }

        override public string ToString()
        {
            return className + "." + memberName + ":" + lineNumber;
        }
    }

    /**
     * Indents module for NkLogger
     */
    public class NkLoggerIndent
    {
        // 3 is a prefix length (DEV/DBG/WRN/ERR/LOG).
        // This way we have really clean & readable log lines that align to the log-type
        private static int SPACES_PER_INDENT = 3;

        private static ThreadLocal<int> indentPerThread = new ThreadLocal<int>(() => 0);

        public static void increment()
        {
            indentPerThread.Value++;
        }

        public static void decrement()
        {
            indentPerThread.Value--;
        }

        public static string getIndent()
        {
            return new string(' ', indentPerThread.Value * SPACES_PER_INDENT);
        }
    }
}
