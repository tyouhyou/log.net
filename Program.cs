using System.Diagnostics;
using zb;

namespace Lognet
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.LogFile = "test/result.log";

            var testcount = 1001;
            var test = new Test();

            if (null != args && args.Length > 0)
            {
                int.TryParse(args[0], out testcount);
            }
            Log.O($"Test count: {testcount}");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for(var i = 0; i < testcount; i++)
            {
                Log.LogLevel = Log.Level.DEBUG;
                Log.D("Hello bugs");
                Log.I("For your information");
                Log.W("Caution, the sky is falling.");
                Log.E("Oh what's wrong, the sky fell.");

                Test.StaticLogs();
                test.Logs().AnotherLog();
            }

            sw.Stop();
            Log.O($"Elaspsed: {sw.Elapsed.TotalMilliseconds} ms.");
        }
    }
}
