using UnityEngine;
using System.Collections;

namespace Diggames.Utilities
{
    public static class DebugLogger
    {
        public enum LogType {NORMAL, WARNING, ERROR, };
        public static bool LogDebugStatements = false;

        public static void LogMessage(string message, LogType logType = LogType.NORMAL)
        {
            if(LogDebugStatements)
            {
                switch(logType)
                {
                    case LogType.NORMAL :
                        Debug.Log(message);
                        break;

                    case LogType.WARNING :
                        Debug.LogWarning(message);
                        break;

                    case LogType.ERROR :
                        Debug.LogError(message);
                        break;
                }            
            }            
        }
    }
}
