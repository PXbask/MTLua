using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Log
{
    public static void Info(string msg)
    {
        if (!APPConst.OpenLog)
            return;
        Debug.Log(msg);
    }
    public static void Warning(string msg)
    {
        if (!APPConst.OpenLog)
            return;
        Debug.LogWarning(msg);
    }
    public static void Error(string msg)
    {
        if (!APPConst.OpenLog)
            return;
        Debug.LogError(msg);
    }
}
