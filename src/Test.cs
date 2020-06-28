using zb;

class Test
{
    public static void StaticLogs()
    {
        Log.D("Debug-");
        Log.I("Info-");
        Log.W("Warn-");
        Log.E("Error-");
    }

    public Test Logs()
    {
        Log.D("Debug--");
        Log.I("Info--");
        Log.W("Warn--");
        Log.E("Error--");

        return this;
    }

    public Test AnotherLog()
    {
        DF("Debug---");
        IF("Info---");
        WF("Warn---");
        EF("Error---");

        return this;
    }

    private void DF(string msg) => Log.D(msg, "test/another.log");
    private void IF(string msg) => Log.I(msg, "test/another.log");
    private void WF(string msg) => Log.W(msg, "test/another.log");
    private void EF(string msg) => Log.E(msg, "test/another.log");
}