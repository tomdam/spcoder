using FastColoredTextBoxNS;
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

        public Log()
        {
            InitializeComponent();
            //RtLog = rtLog;
            
        }
        
        public void AppendToLog(string text)
        {
            //rtLog.Text += "[" + DateTime.Now.ToShortTimeString() + "]:" + text + "\n";
            LogText(text, infoStyle);
        }

        public void LogError(string text)
        {
            LogText(text, errorStyle);
        }

        public void LogWarning(string text)
        {
            LogText(text, warningStyle);
        }

        public void LogInfo(string text)
        {
            LogText(text, infoStyle);
        }

        private void LogText(string text, Style style)
        {
            if (fctb.InvokeRequired)
            {
                LogTextVoidDelegate d = new LogTextVoidDelegate(LogText);
                this.Invoke(d, new object[] { text, style });
            }
            else
            {
                //some stuffs for best performance
                fctb.BeginUpdate();
                fctb.Selection.BeginUpdate();
                //remember user selection
                var userSelection = fctb.Selection.Clone();
                //add text with predefined style
                fctb.AppendText(DateTime.Now.ToLongTimeString() + ": " + text + "\r\n", style);
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
        delegate void LogTextVoidDelegate(string text, Style style);

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
