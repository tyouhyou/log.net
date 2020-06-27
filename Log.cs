using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace zb
{
    /// 
    ///
    ///<remarks>
    /// TODO: 
    /// 1. outputting to multiple files
    /// </remarks>
    public class Log
    {
        public enum LogLevel
        {
            ALL = 0,
            DEBUG,
            INFO,
            WARN,
            ERROR,
            NONE,
        }

        private static LogLevel LogLv { set; get; }
        // TODO: not string but file stream?
        private static string LogFile { set; get; }

        public static void SetLogFile(string file)
        {
            // if exception occurred, let it go.
            using (var f = File.Open(file, FileMode.OpenOrCreate))
            {
                // DO NOTHING, just make sure file exists or be created
            }
            LogFile = file;
        }

        public static void SetLogLevel(LogLevel lv)
        {
            LogLv = LogLevel.NONE;
            if (Enum.IsDefined(typeof(LogLevel), lv))
            {
                LogLv = lv;
            }
        }

        private static string Output(string msg, LogLevel lv, 
            string path=null, 
            string line=null,
            string fn=null)
        {
            if (lv < LogLv) return;

            var p = null != path ? $"[{Path.GetFileName(path)}]" : string.empty;
            var f = null != fn ? $"[{fn}]" : string.empty;
            var l = null != line ? $"[{line}]" : string.empty;

            WriteLog(LogFile, $"[{lv}][{DateTime.Now}]{p}{l}{f} - {msg}");
        }

        public static void D(string msg) => Output(msg, LogLevel.DEBUG,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = ""
            );

        public static void I(string msg) => Output(msg, LogLevel.INFO);

        public static void W(string msg) => Output(msg, LogLevel.WARN);

        public static void E(string msg) => Output(msg, LogLevel.ERROR,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = ""
            );

        private static void WriteLog(string file, string msg)
        {
            // Do not check null file path. If file path not set, let exception fury.
            try
            {
                using (var f = File.AppendText(file))
                {
                    f.WriteLine(msg);
                }
            }
            catch(exception)
            {
                // DO NOTHING
            }
        }
    }
}