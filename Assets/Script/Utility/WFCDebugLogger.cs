using UnityEngine;

public class WFCDebugLogger 
{
    public static void log(string _logChannel, string _s)
    {
        Debug.Log("LOG:  " + _logChannel + ":     " + _s);
    }


    public static void logError(string _logChannel, string _s)
    {
        Debug.LogError("ERROR LOG:  " + _logChannel + ":     " + _s);
    }
}
