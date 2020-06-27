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

    public void Logs()
    {
        Log.D("Debug--");
        Log.I("Info--");
        Log.W("Warn--");
        Log.E("Error--");
    }
}