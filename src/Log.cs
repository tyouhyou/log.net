using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace zb
{
    public class Log
    {
        public enum Level
        {
            DEBUG = 0,
            PERF,
            INFO,
            WARN,
            ERROR,
            NONE,
        }

        #region  Static Members

        private static string _LogFile = Assembly.GetEntryAssembly().GetName().Name + ".log";
        public static string LogFile
        {
            set { _LogFile = Path.GetFullPath(value); }
            get { return _LogFile; }
        }

#if DEBUG
        private static Log.Level _LogLevel = Log.Level.DEBUG;
#else
        private static Log.Level _LogLevel = Log.Level.INFO;
#endif

        public static Log.Level LogLevel
        {
            set { _LogLevel = value; }
            get { return _LogLevel; }
        }

        private static Dictionary<string, Log> LogList = new Dictionary<string, Log>();

        static Log()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((s, e) =>
            {
                foreach (var kv in LogList)
                {
                    kv.Value.Dispose();
                }
            });
        }

        public static void D(string msg,
            string file = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = null)
            => Output(msg, Log.Level.DEBUG, file, sourceFilePath, sourceLineNumber, memberName);

        public static void P(string msg, string file = null) => Output(msg, Log.Level.PERF, file);

        public static void I(string msg, string file = null) => Output(msg, Log.Level.INFO, file);

        public static void W(string msg, string file = null) => Output(msg, Log.Level.WARN, file);

        public static void E(string msg,
            string file = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = null)
            => Output(msg, Log.Level.ERROR, file, sourceFilePath, sourceLineNumber, memberName);

        // Output to stdout
        public static void O(string msg,
            [CallerFilePath] string file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string func = null)
        {
            Console.Out.WriteLine($"[{DateTime.Now}][{Path.GetFileName(file)}({line})::{func}] {msg}");
        }

        private static void Output(
            string msg,
            Log.Level lv,
            string file,
            string path = null,
            int line = 0,
            string fn = null)
        {
            if (lv < LogLevel) return;

            try
            {
                var p = null != path ? $"{Path.GetFileName(path)}" : string.Empty;
                var f = null != fn ? $"{fn}" : string.Empty;
                var l = 0 != line ? $"{line}" : string.Empty;
                string fi;
                
                if (null != file)
                {
                    fi = Path.GetFullPath(file).ToLowerInvariant();
                }
                else
                {
                    fi = LogFile;
                }

                Log log = null;
                if (!LogList.ContainsKey(fi))
                {
                    log = new Log(fi);
                    LogList.Add(fi, log);
                }
                else
                {
                    log = LogList[fi];
                }

                var fili = string.IsNullOrWhiteSpace(p) ? string.Empty : $"[{p}({l})::{f}]";
                log.OutputLog($"[{DateTime.Now}][{lv,-5}]{fili} {msg}");
            }
            catch(Exception){/* Do Nothing */}
        }

        #endregion  // Static Members

        #region Instance Memebers

        private string MyFile { get; set; }
#if LOG_FILE_KEEP_OPENNING
        private StreamWriter SW { get; set; }
#endif
#if LOG_LOCK
        private readonly object locker = new object();
#endif

        private Log(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException("Log file could not be null or empty.");
            }
            MyFile = file;
#if LOG_FILE_KEEP_OPENNING
            SW = new StreamWriter(MyFile, true);
#endif
        }

        ~Log()
        {
            Dispose();
        }

        private void Dispose()
        {
#if LOG_FILE_KEEP_OPENNING
            if (null != SW) SW.Dispose();
#endif
        }

        private void OutputLog(string msg)
        {
#if LOG_LOCK
            lock(locker)
            {
#endif
            if (null == msg) return;
#if !LOG_FILE_KEEP_OPENNING
                using (StreamWriter fsw = new StreamWriter(MyFile, true))
                {
                    fsw.WriteLine(msg);
                }
#else
            SW.WriteLine(msg);
            SW.Flush();
#endif

#if LOG_LOCK
            }
#endif
        }

        #endregion // Instance Members
    }

    ///
    /// Usually, using a instance of a this class is sufficient.
    /// However, there has cases that meseauring performance across
    /// functions or even classes. Using the static methods instead of
    /// instancial ones may be desired.
    ///
    public class PerfLog
    {
        #region static Members

        private static PerfLog _MyStaticWatch;

        ///
        /// Start the stopwatch
        ///
        public static void S()
        {
            if (null == _MyStaticWatch)
            {
                _MyStaticWatch = new PerfLog();
            }
            _MyStaticWatch.Start();
        }

        ///
        /// Wrap the elasped time since last wrapping.
        ///
        public static void R(string msg, string file = null) => _MyStaticWatch.Wrap(msg, file);

        ///
		/// Record elaspsd time without stop the stopwatch
		///
        public static void L(string msg, string file = null) => _MyStaticWatch.Elapsed(msg, file);

        ///
        /// Record elaspsd time and stop the stopwatch
        ///
        public static void E() => _MyStaticWatch.Stop();

        #endregion

        #region instance members

        private Stopwatch _MyWatch = new Stopwatch();
        private long _LastElaspsed = 0;

        public void Start()
        {
            _MyWatch.Restart();
        }

        public void Wrap(string msg, string file = null)
        {
            var elps = _MyWatch.ElapsedMilliseconds;
            Log.P(msg + $" ({elps - _LastElaspsed} ms)", file);
            _LastElaspsed = elps;
        }

        public void Elapsed(string msg, string file = null)
        {
            Log.P(msg + $" ({_MyWatch.ElapsedMilliseconds} ms)", file);
        }

        public void Stop() 
        {
            _MyWatch.Stop();
        }

        #endregion
    }
}