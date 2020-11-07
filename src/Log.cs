//#define LOG_CLOSE_FILE

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
            set { _LogFile = value; }
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
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((s,e)=>{
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
            Console.Out.WriteLine($"[{DateTime.Now}][{Path.GetFileName(file)}][line][func] - {msg}");
        }

        private static void Output(
            string msg, 
            Log.Level lv, 
            string file,
            string path=null, 
            int line=0,
            string fn=null)
        {
            if (lv < LogLevel) return;

            var p = null != path ? $"[{Path.GetFileName(path)}]" : string.Empty;
            var f = null != fn ? $"[{fn}]" : string.Empty;
            var l = 0 != line ? $"[{line}]" : string.Empty;
            var fi = Path.GetFullPath(null != file ? file : LogFile).ToLowerInvariant();

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
            
            log.OutputLog($"[{lv,-5}][{DateTime.Now}]{p}{l}{f} - {msg}");
        }

#endregion  // Static Members

#region Instance Memebers

        private string MyFile { get; set; }
#if !LOG_CLOSE_FILE
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
#if !LOG_CLOSE_FILE
            SW = new StreamWriter(MyFile, true);
#endif
        }

        ~Log()
        {
            Dispose();
        }

        private void Dispose()
        {
#if !LOG_CLOSE_FILE
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
#if LOG_CLOSE_FILE
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

    public class Perf
    {
#region static Members

        private static Stopwatch _MyStaticWatch;

        ///
        /// Start the stopwatch
        ///
        public static void S()
        {
            if (null == _MyStaticWatch)
            {
                _MyStaticWatch = new Stopwatch();
            }
            _S(_MyStaticWatch);
        }

        ///
		/// Record elaspsd time without stop the stopwatch
		///
        public static void R(string msg, string file = null) => _R(_MyStaticWatch, msg, file);

        ///
        /// Record elaspsd time and stop the stopwatch
        ///
        public static void E(string msg, string file = null) => _E(_MyStaticWatch, msg, file);

        ///
		/// Destroy the stopwatch
        ///
        public static void D()
        {
            _MyStaticWatch = null;
        }

        private static void _S(Stopwatch sw)
        {
            sw.Restart();
        }
	
		private static void _R(Stopwatch sw, string msg, string file = null)
		{
			var ns = sw.Elapsed.TotalMilliseconds * 1000000;
            Log.P(msg + $" ([{sw.ElapsedMilliseconds} ms] or [{ns} ns])", file);
		}

        private static void _E(Stopwatch sw, string msg, string file = null)
        {
            sw.Stop();
            _R(sw, msg, file);
        }

#endregion
        
#region instance members

        private Stopwatch _MyWatch = new Stopwatch();

        public void Start() => Perf._S(_MyWatch);

        public void Record(string msg, string file = null) => Perf._R(_MyWatch, msg, file); 

        public void Stop(string msg, string file = null) => Perf._E(_MyWatch, msg, file);

#endregion
    }
}