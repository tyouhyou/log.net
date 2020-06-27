using System;
using zb;

namespace Lognet
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.SetLogFile("result/mylog.txt");
            Log.SetLogLevel(Log.LogLevel.ERROR);
            Log.D("Hello debug");
            Log.I("For your information");
            Log.W("Warn, the sky is falling.");
            Log.E("Oh what's wrong, the sky fallen.");
            
            Test.StaticLogs();
            new Test().Logs();
        }
    }
}
