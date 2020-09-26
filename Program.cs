
using System;
using zb;

namespace Lognet
{
    class Program
    {
        static void Main(string[] args)
        {
            try 
            {
                AppDomain.CurrentDomain.UnhandledException += (o, e) =>
                {
                    Log.O(((Exception)e.ExceptionObject).ToString());
                };

                Log.LogFile = "test/result.log";

                var testcount = 1001;
                var test = new Test();

                if (null != args && args.Length > 0)
                {
                    int.TryParse(args[0], out testcount);
                }
                Log.O($"Test count: {testcount}");

                var p = new Perf();
                p.Start();

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

                p.Stop("Test#Program");
            }
            catch(Exception e)
            {
                Log.E(e.ToString());
            }
        }
    }
}
