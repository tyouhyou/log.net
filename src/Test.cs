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
        Perf.S();

        Log.D("Debug--");
        Log.I("Info--");
        Log.W("Warn--");
        Log.E("Error--");

        Perf.E("Test#Logs");

        return this;
    }

    public Test AnotherLog()
    {
        Perf.S();

        DF("Debug---");
        IF("Info---");
        WF("Warn---");
        EF("Error---");

        Perf.E("Test#AnotherLog", "test/another.log");

        return this;
    }

    private void DF(string msg) => Log.D(msg, "test/another.log");
    private void IF(string msg) => Log.I(msg, "test/another.log");
    private void WF(string msg) => Log.W(msg, "test/another.log");
    private void EF(string msg) => Log.E(msg, "test/another.log");
}