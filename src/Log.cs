using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace zb.Lib.Logger
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

        public static string DefaultLogFile = Assembly.GetEntryAssembly().GetName().Name + ".log";

        private static string _LogFile = null;
        /// <sumary>
        /// If no log file is set, all logs will be writen to Trace.
        /// </sumary>
        public static string LogFile
        {
            set
            {
                _LogFile = Path.GetFullPath(value);
                MyLog = new Log(_LogFile);
            }
            get
            {
                return _LogFile;
            }
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

        private static Log MyLog { set; get; }

#if MULT_LOGS
        private static Dictionary<string, Log> LogList { set; get; }
#endif

        static Log()
        {
            MyLog = new Log(null);
#if MULT_LOGS
            LogList = new Dictionary<string, Log>();
#endif
        }

        public static void D(string msg,
            string file = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = null)
            => Output(msg, Log.Level.DEBUG, file, sourceFilePath, sourceLineNumber, memberName);

        public static void P(string msg, string file = null)
            => Output(msg, Log.Level.PERF, file);

        public static void I(string msg, string file = null)
            => Output(msg, Log.Level.INFO, file);

        public static void W(string msg, string file = null)
            => Output(msg, Log.Level.WARN, file);

        public static void E(string msg,
            string file = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = null)
            => Output(msg, Log.Level.ERROR, file, sourceFilePath, sourceLineNumber, memberName);

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
                Log log = null;

#if MULT_LOGS
                string fi;
                if (null != file)
                {
                    fi = Path.GetFullPath(file).ToLowerInvariant();
                    if (!LogList.ContainsKey(fi))
                    {
                        log = new Log(fi);
                        LogList.Add(fi, log);
                    }
                    else
                    {
                        log = LogList[fi];
                    }
                }
                else
                {
                    log = MyLog;
                }
#else
                log = MyLog;
#endif

                var fili = string.IsNullOrWhiteSpace(p) ? string.Empty : $"[{p}({l})::{f}]";
                log.OutputLog($"[{DateTime.Now}][{lv,-5}]{fili} {msg}");
            }
            catch (Exception) {/* Do Nothing */}
        }

        #endregion  // Static Members

        #region Instance Memebers

#if LOG_LOCK
        private readonly object locker = new object();
#endif

        private string MyFile { set; get; }

        private Log(string file)
        {
            MyFile = file;
        }

        private void OutputLog(string msg)
        {
#if LOG_LOCK
            lock(locker)
            {
#endif
            if (null == MyFile)
            {
                Trace.WriteLine(msg);
                return;
            }

            using (StreamWriter fsw = new StreamWriter(MyFile, true))
            {
                fsw.WriteLine(msg);
            }
#if LOG_LOCK
            }
#endif
        }

        #endregion // Instance Members
    }

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
        public static void R(string msg, string file = null)
            => _MyStaticWatch.Wrap(msg, file);

        ///
		/// Record elaspsd time without stop the stopwatch
		///
        public static void L(string msg, string file = null)
            => _MyStaticWatch.Elapsed(msg, file);

        public static double P()
            => _MyStaticWatch.Peek();

        ///
        /// Record elaspsd time and stop the stopwatch
        ///
        public static void E()
            => _MyStaticWatch.Stop();

        #endregion

        #region instance members

        private Stopwatch _MyWatch = new Stopwatch();
        private double _LastElaspsed = 0;

        public void Start()
        {
            _MyWatch.Restart();
        }

        public void Wrap(string msg, string file = null)
        {
            var elps = _MyWatch.Elapsed.TotalMilliseconds - _LastElaspsed;
            Log.P(msg + $" ({elps} ms; {elps * 1000000} ns)", file);
            _LastElaspsed = elps;
        }

        public void Elapsed(string msg, string file = null)
        {
            var elps = _MyWatch.Elapsed.TotalMilliseconds;
            Log.P(msg + $" ({elps: 0.000} ms; {elps * 1000000: 0.00} ns)", file);
        }

        public double Peek()
        {
            return _MyWatch.Elapsed.TotalMilliseconds;
        }

        public void Stop()
        {
            _MyWatch.Stop();
        }

        #endregion
    }
}