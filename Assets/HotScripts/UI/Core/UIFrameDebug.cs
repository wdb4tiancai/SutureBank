using System.Diagnostics;
using UnityEngine;
namespace UIFrameWork
{
    public class UIFrameDebug
    {
        [Conditional("UI_FRAME_DEBUG")]
        public static void Log(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        [Conditional("UI_FRAME_DEBUG")]
        public static void LogWarning(string logWarning)
        {
            UnityEngine.Debug.LogWarning(logWarning);
        }

        [Conditional("UI_FRAME_DEBUG")]
        public static void LogError(string logError)
        {
            UnityEngine.Debug.LogError(logError);
        }
    }
}