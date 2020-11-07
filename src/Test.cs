using zb;

class Test
{
    public static void StaticLogs()
    {
        Perf.S();

        Log.D("Debug-");
        Log.I("Info-");
        Log.W("Warn-");
        Log.E("Error-");

        Perf.E("Test#StaticLogs");
    }

    public Test Logs()
    {
        var p = new Perf();
        p.Start();

        Log.D("Debug--");
        Log.I("Info--");
        Log.W("Warn--");
        Log.E("Error--");

        p.Stop("Test#Logs");

        return this;
    }

    public Test AnotherLog()
    {
        var p = new Perf();
        p.Start();

        DF("Debug---");
        IF("Info---");
        WF("Warn---");
        EF("Error---");

        p.Stop("Test#AnotherLog", "test/another.log");

        return this;
    }

    private void DF(string msg) => Log.D(msg, "test/another.log");
    private void IF(string msg) => Log.I(msg, "test/another.log");
    private void WF(string msg) => Log.W(msg, "test/another.log");
    private void EF(string msg) => Log.E(msg, "test/another.log");
}