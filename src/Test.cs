using zb;

class Test
{
    public static void StaticLogs()
    {
        PerfLog.S();

        Log.D("Debug-");
        Log.I("Info-");
        Log.W("Warn-");
        Log.E("Error-");

        PerfLog.L("Test#StaticLogs");
        PerfLog.E();
    }

    public Test Logs()
    {
        var p = new PerfLog();
        p.Start();

        Log.D("Debug--");
        Log.I("Info--");
        Log.W("Warn--");
        Log.E("Error--");

        p.Elapsed("Test#Logs");
        p.Stop();

        return this;
    }

    public Test AnotherLog()
    {
        var p = new PerfLog();
        p.Start();

        DF("Debug---");
        IF("Info---");
        WF("Warn---");
        EF("Error---");

        p.Wrap("Test#AnotherLog", "test/another.log");
        p.Stop();

        return this;
    }

    private void DF(string msg) => Log.D(msg, "test/another.log");
    private void IF(string msg) => Log.I(msg, "test/another.log");
    private void WF(string msg) => Log.W(msg, "test/another.log");
    private void EF(string msg) => Log.E(msg, "test/another.log");
}