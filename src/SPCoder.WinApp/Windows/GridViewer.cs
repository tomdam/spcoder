using SPCoder.Context;
using SPCoder.Core.Utils;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SPCoder.Windows
{
    public partial class GridViewer : DockContent
    {
        public GridViewer()
        {
            InitializeComponent();
            LoadDefaultExpression();
        }

        public object GridSource { get; set; }

        private bool gridAddedToContext = false;

        private void LoadDefaultExpression()
        {
            if (SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_GRID_VIEWER] != null)
            {
                var gridSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_GRID_VIEWER];
                if (gridSettings[SPCoderConstants.SP_SETTINGS_EXPRESSION] != null)
                {
                    string expression = gridSettings[SPCoderConstants.SP_SETTINGS_EXPRESSION].ToString();
                    txtCode.Text = expression;
                }
            }
        }

        private void SetExpression(string expression)
        {
            if (SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_GRID_VIEWER] != null)
            {
                var gridSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_GRID_VIEWER];
                txtCode.Text = expression;
                gridSettings[SPCoderConstants.SP_SETTINGS_EXPRESSION] = expression;
            }
        }

        public void ShowExpressionInGrid(string expression)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    txtCode.Text = expression;
                    showDataInGrid();
                });
            }
            else
            {
                txtCode.Text = expression;
                showDataInGrid();
            }
        }

        private void showDataInGrid()
        {
            try
            {
                if (!gridAddedToContext)
                {
                    SPCoderForm.MainForm.MyContext.AddItem(new ContextItem { Data = this, Name = "myGridViewer", Type = this.GetType().ToString() });
                    gridAddedToContext = true;
                }

                //SPCoderForm.ironPythonEngine.SetVariable("myGridViewer", this);

                string code = "myGridViewer.GridSource = " + txtCode.Text + "";
                //SPCoderForm.ironPythonEngine.Execute(code);
                SPCoderForm.MainForm.ExecuteScriptCSharp(code);
                grid.AutoGenerateColumns = true;

                var source = new BindingSource();
                source.DataSource = GridSource;
                grid.DataSource = source;

                //grid.DataSource = this.GridSource;
                

            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);

                string stackTrace = (exc.StackTrace.Contains("ExecuteScriptCSharp") ? "" : exc.StackTrace);
                SPCoderForm.MainForm.LogError(exc.Message + "\n" + stackTrace);
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {            
            showDataInGrid();
            SetExpression(txtCode.Text);
        }

        private void grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void Paste()
        {
            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.StringFormat))
            {
                string[] pastedRows = Regex.Split(o.GetData(DataFormats.StringFormat).ToString().TrimEnd("\r\n".ToCharArray()), "\r");
                int j = 0;
                try { j = grid.CurrentRow.Index; } catch { }
                foreach (string pastedRow in pastedRows)
                {
                    DataGridViewRow r = new DataGridViewRow();
                    r.CreateCells(grid, pastedRow.Split(new char[] { '\t' }));
                    grid.Rows.Insert(j, r);
                    j++;
                }
            }
        }

        private void grid_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Control && e.KeyCode == Keys.V)
            {
                Paste();
            }*/
        }

        private void btnView_KeyPress(object sender, KeyPressEventArgs e)
        {
            //If space is pressed, show the grid
            if (e.KeyChar == (char)Keys.Space)
            {
                btnView_Click(sender, e);
            }
        }
    }
}
