using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPCoder.Windows
{
    public interface ISPCoderLog
    {
        void AppendToLog(string text);
        void LogError(string text);
        void LogWarning(string text);
        void LogInfo(string text);
    }
}
