using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SPCoder.Windows
{
    public partial class Output : DockContent
    {
        public Output()
        {
            InitializeComponent();
            RtOutput = rtOutput;
            cw = new ControlWriter(RtOutput);
            LoadDefaultSettings();
            cw.ClearBeforeExecution = cbClear.Checked;
            Console.SetOut(cw);
        }

        private ControlWriter cw = null;
        public ControlWriter ConsoleWindow { get { return cw; } }
        public void ClearOutput()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    rtOutput.Clear();
                });
            }
            else
            {
                rtOutput.Clear();
            }
        }

        public void ClearOutputIfChecked()
        {
            if (cbClear.Checked) ClearOutput(); // rtOutput.Clear();
        }

        public RichTextBox RtOutput { get; private set; }

        private void LoadDefaultSettings()
        {
            if (SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_OUTPUT] != null)
            {
                var outputSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_OUTPUT];
                if (outputSettings[SPCoderConstants.SP_SETTINGS_CLEAR_BEFORE_NEXT_EXECUTION] != null)
                {
                    string value = outputSettings[SPCoderConstants.SP_SETTINGS_CLEAR_BEFORE_NEXT_EXECUTION].ToString();
                    bool isChecked = bool.Parse(value);
                    cbClear.Checked = isChecked;
                }
            }
        }

        private void SaveSettings()
        {
            if (SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_OUTPUT] != null)
            {
                var outputSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_OUTPUT];
                outputSettings[SPCoderConstants.SP_SETTINGS_CLEAR_BEFORE_NEXT_EXECUTION] = cbClear.Checked;
            }
        }

        public static string GetRtfTable(DataTable pDt)
        {
            string rtf = "";
            // Width will be in TWIPS
            int LeftColWidth = 4000, OtherColumnsWidth = 2000;
            int ColumnPosition;

            DataRow dr;
            for (int i = 0; i < pDt.Rows.Count; i++)
            {
                dr = pDt.Rows[i];
                // STARTING TABLE ROW
                rtf += @"{\trowd\trleft0\trgaph-0\trbrdrt\brdrnone\trbrdrb\brdrnone\trbrdrr\brdrnone\trbrdrl\brdrnone\trbrdrv\brdrnone\trbrdrh\brdrnone\trftsWidth1\trftsWidthB3\trpaddl10\trpaddr10\trpaddb10\trpaddt10\trpaddfl3\trpaddfr3\trpaddft3\trpaddfb3\trql\ltrrow";
                //rtf += @"{\trowd\trleft0\trgaph-0\trbrdrt\brdrs\brdrw15\brdrcf0\trbrdrb\brdrs\brdrw15\brdrcf0\trbrdrr\brdrs\brdrw15\brdrcf0\trbrdrl\brdrs\brdrw15\brdrcf0\trbrdrv\brdrs\brdrw15\brdrcf0\trbrdrh\brdrs\brdrw15\brdrcf0\trftsWidth1\trftsWidthB3\trpaddl10\trpaddr10\trpaddb10\trpaddt10\trpaddfl3\trpaddfr3\trpaddft3\trpaddfb3\trql\ltrrow";
                // LEFT COLUMN DEFINITION
                ColumnPosition = LeftColWidth;
                rtf += @"\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\clftsWidth3\clwWidth" + LeftColWidth.ToString() + @"\cellx" + ColumnPosition.ToString();
                // OTHER COLUMNS DEFINITION
                for (int j = 1; j < pDt.Columns.Count; j++)
                {
                    ColumnPosition += OtherColumnsWidth;
                    rtf += @"\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\clftsWidth3\clwWidth" + OtherColumnsWidth.ToString() + @"\cellx" + ColumnPosition.ToString();
                }

                rtf += "{";
                // PLACING DATA
                for (int j = 0; j < pDt.Columns.Count; j++)
                {
                    if (i == 0)
                        rtf += @"{\fs22\f3\b\intbl {\ul\ltrch " + dr[j].ToString() + @"}\li0\ri0\sa0\sb0\fi0\ql\sl15\slmult0\cell}";
                    else
                        rtf += @"{\fs22\f3\intbl {\ltrch " + dr[j].ToString() + @"}\li0\ri0\sa0\sb0\fi0\ql\sl15\slmult0\cell}";
                }

                rtf += "}";
                // ENDING TABLE ROW
                rtf += @"{\trowd\trleft0\trgaph-0\trbrdrt\brdrnone\trbrdrb\brdrnone\trbrdrr\brdrnone\trbrdrl\brdrnone\trbrdrv\brdrnone\trbrdrh\brdrnone\trftsWidth1\trftsWidthB3\trpaddl10\trpaddr10\trpaddb10\trpaddt10\trpaddfl3\trpaddfr3\trpaddft3\trpaddfb3\trql\ltrrow";
                //rtf += @"{\trowd\trleft0\trgaph-0\trbrdrt\brdrs\brdrw15\brdrcf0\trbrdrb\brdrs\brdrw15\brdrcf0\trbrdrr\brdrs\brdrw15\brdrcf0\trbrdrl\brdrs\brdrw15\brdrcf0\trbrdrv\brdrs\brdrw15\brdrcf0\trbrdrh\brdrs\brdrw15\brdrcf0\trftsWidth1\trftsWidthB3\trpaddl10\trpaddr10\trpaddb10\trpaddt10\trpaddfl3\trpaddfr3\trpaddft3\trpaddfb3\trql\ltrrow";
                // LEFT COLUMN
                ColumnPosition = LeftColWidth;
                rtf += @"\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\clftsWidth3\clwWidth" + LeftColWidth.ToString() + @"\cellx" + ColumnPosition.ToString();
                // OTHER COLUMNS
                for (int j = 1; j < pDt.Columns.Count; j++)
                {
                    ColumnPosition += OtherColumnsWidth;
                    rtf += @"\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\clftsWidth3\clwWidth" + OtherColumnsWidth.ToString() + @"\cellx" + ColumnPosition.ToString();
                }
                rtf += @"\row}}";
            }
            return rtf;
        }

        private void cbClear_CheckedChanged(object sender, EventArgs e)
        {
            cw.ClearBeforeExecution = cbClear.Checked;
            SaveSettings();
        }
    }

    public class ControlWriter : TextWriter
    {
        private RichTextBox textbox;

        public bool ClearBeforeExecution { get; set; }
        public ControlWriter(Control textbox)
        {
            this.textbox = (RichTextBox)textbox;
        }
        
        public override void Write(char value)
        {            
            //textbox.Text += value;
            AppendChar(value);
        }

        public override void Write(string value)
        {            
            //textbox.Text += value;
            AppendText(value);
        }

        public override void WriteLine(string value)
        {            
            //textbox.Text += value + Environment.NewLine;
            AppendText(value + Environment.NewLine);
        }

        public override void WriteLine(int value)
        {
            //textbox.Text += value + Environment.NewLine;
            AppendText(value + Environment.NewLine);
        }

        private void AppendText(string text)
        {            
            if (this.textbox.InvokeRequired)
            {
                this.textbox.BeginInvoke((MethodInvoker)delegate ()
                {
                    textbox.Text += text;
                });
            }
            else
            {
                textbox.Text += text;
            }
        }

        private void AppendChar(char text)
        {
            if (this.textbox.InvokeRequired)
            {
                this.textbox.BeginInvoke((MethodInvoker)delegate ()
                {
                    textbox.Text += text;
                });
            }
            else
            {
                textbox.Text += text;
            }
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

}
