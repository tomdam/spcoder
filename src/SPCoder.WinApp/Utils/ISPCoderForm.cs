using FastColoredTextBoxNS;
using SPCoder.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Utils
{
    public interface ISPCoderForm
    {
        void LogException(Exception ex);
        void LogError(string err);
        void AppendToLog(string text);
        void AddToContext(object item);
        FastColoredTextBox SourceCodeBox { get; }
        CSharpCode ActiveDocument { get; }
        Log SpLog { get; set; }
        Output SpOutput { get; set; }
        GridViewer SpGrid { get; set; }

    }
}
