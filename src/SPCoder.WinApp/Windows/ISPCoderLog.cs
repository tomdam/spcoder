using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPCoder.Windows
{
    public interface ISPCoderLog
    {
        void AppendToLog(string text, bool logTimestamp = true);
        void LogError(string text, bool logTimestamp = true);
        void LogWarning(string text, bool logTimestamp = true);
        void LogInfo(string text, bool logTimestamp = true);
    }
}
