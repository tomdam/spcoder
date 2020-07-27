using System;

namespace SPCoder.Core.Utils
{
    public interface ISPCoderLog
    {
        void AppendToLog(string text, bool logTimestamp = true);
        void LogError(string text, bool logTimestamp = true);
        void LogError(Exception exception, bool logTimestamp = true);
        void LogWarning(string text, bool logTimestamp = true);
        void LogInfo(string text, bool logTimestamp = true);
    }
}