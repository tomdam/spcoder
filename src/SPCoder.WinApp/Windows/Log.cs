using FastColoredTextBoxNS;
using SPCoder.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SPCoder.Windows
{
    public partial class Log : DockContent, ISPCoderLog
    {

        TextStyle infoStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle warningStyle = new TextStyle(Brushes.BurlyWood, null, FontStyle.Regular);
        TextStyle errorStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);

        TextStyle blueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Underline);
        public FastColoredTextBox LogBox {
            get { return this.fctb; }
        }
        public Log()
        {
            InitializeComponent();
            //RtLog = rtLog;
            //registering the logger so that other modules can write to SPCoder log window
            SPCoderLogger.Logger = this;
        }
        
        public void AppendToLog(string text, bool logTimestamp = true)
        {
            //rtLog.Text += "[" + DateTime.Now.ToShortTimeString() + "]:" + text + "\n";
            LogText(text, infoStyle, logTimestamp);
        }

        public void LogError(string text, bool logTimestamp = true)
        {
            LogText(text, errorStyle, logTimestamp);
        }

        public void LogError(Exception exception, bool logTimestamp = true)
        {
            string text = exception.Message + "\n" + exception.StackTrace;
            LogText(text, errorStyle, logTimestamp);
        }
        public void LogWarning(string text, bool logTimestamp = true)
        {
            LogText(text, warningStyle, logTimestamp);
        }

        public void LogInfo(string text, bool logTimestamp = true)
        {
            LogText(text, infoStyle, logTimestamp);
        }

        private void LogText(string text, Style style, bool logTimestamp = true)
        {
            if (fctb.InvokeRequired)
            {
                LogTextVoidDelegate d = new LogTextVoidDelegate(LogText);
                this.Invoke(d, new object[] { text, style, logTimestamp });
            }
            else
            {
                //some stuffs for best performance
                fctb.BeginUpdate();
                fctb.Selection.BeginUpdate();
                //remember user selection
                var userSelection = fctb.Selection.Clone();
                //add text with predefined style
                if (logTimestamp)
                {
                    fctb.AppendText(DateTime.Now.ToLongTimeString() + ": " + text + "\r\n", style);
                }
                else
                {
                    fctb.AppendText(text + "\r\n", style);
                }
                
                //restore user selection
                /*if (!userSelection.IsEmpty || userSelection.Start.iLine < fctb.LinesCount - 2)
                {
                    fctb.Selection.Start = userSelection.Start;
                    fctb.Selection.End = userSelection.End;
                }
                else*/
                fctb.GoEnd();//scroll to end of the text
                             //
                fctb.Selection.EndUpdate();
                fctb.EndUpdate();
            }
        }
        delegate void LogTextVoidDelegate(string text, Style style, bool logTimestamp);

        public void ClearLog()
        {
            ClearLogPrivate();
        }

        private void ClearLogPrivate()
        {
            if (fctb.InvokeRequired)
            {
                ClearLogDelegate d = new ClearLogDelegate(ClearLogPrivate);
                this.Invoke(d, new object[] {  });
            }
            else
            {
                fctb.Clear();
            }
        }
        delegate void ClearLogDelegate();

        bool CharIsHyperlink(Place place)
        {
            var mask = fctb.GetStyleIndexMask(new Style[] { blueStyle });
            if (place.iChar < fctb.GetLineLength(place.iLine))
                if ((fctb[place].style & mask) != 0)
                    return true;

            return false;
        }

        private void fctb_MouseMove(object sender, MouseEventArgs e)
        {
            var p = fctb.PointToPlace(e.Location);
            if (CharIsHyperlink(p))
                fctb.Cursor = Cursors.Hand;
            else
                fctb.Cursor = Cursors.IBeam;
        }

        private void fctb_MouseDown(object sender, MouseEventArgs e)
        {
            var p = fctb.PointToPlace(e.Location);
            if (CharIsHyperlink(p))
            {
                var url = fctb.GetRange(p, p).GetFragment(@"[\S]").Text;
                Process.Start(url);
            }
        }

        private void fctb_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(blueStyle);
            e.ChangedRange.SetStyle(blueStyle, @"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
        }

    }

    

}
