using System;
using System.Windows.Forms;
using SPCoder.Config;
using WeifenLuo.WinFormsUI.Docking;

namespace SPCoder.Autorun
{
    /// <summary>
    /// Code file for the AutoRun scripts form!
    /// </summary>
    /// <author>Damjan Tomic</author>
    public partial class AutoRunScriptsForm : DockContent
    {
        public AutoRunScriptsForm()
        {
            InitializeComponent();
        }

        private AutorunScriptConfig CurrentConfig;
        
        private void AutoRunScriptsForm_Load(object sender, System.EventArgs e)
        {
            CurrentConfig = AutorunScriptUtils.GetAutorunConfig();
            
            dataGridView1.DataSource = CurrentConfig.AutoRunScripts;
            dataGridView1.Refresh();
        }

        public void RefreshView()
        {
            CurrentConfig = AutorunScriptUtils.GetAutorunConfig();

            dataGridView1.DataSource = CurrentConfig.AutoRunScripts;
            dataGridView1.Refresh();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            //Move script up
            var rows = dataGridView1.SelectedRows;
            if (rows != null && rows.Count == 1)
            {                
                if (CurrentConfig.AutoRunScripts.Count > 1)
                {
                    AutorunScriptConfigItem item = (AutorunScriptConfigItem) rows[0].DataBoundItem;
                    int ind = CurrentConfig.AutoRunScripts.IndexOf(item);
                    if (ind > 0)
                    {
                        dataGridView1.DataSource = null;
                        item.Order = item.Order - 1;
                        CurrentConfig.AutoRunScripts[ind - 1].Order = CurrentConfig.AutoRunScripts[ind - 1].Order + 1;

                        CurrentConfig.AutoRunScripts.Sort((a, b) => a.Order.CompareTo(b.Order));

                        dataGridView1.DataSource = CurrentConfig.AutoRunScripts;
                        dataGridView1.Refresh();
                    }                    
                }
            }
        }

        private void btnMoveDown_Click(object sender, System.EventArgs e)
        {
            //Move script down
            var rows = dataGridView1.SelectedRows;
            if (rows != null && rows.Count == 1)
            {
                if (CurrentConfig.AutoRunScripts.Count > 1)
                {
                    AutorunScriptConfigItem item = (AutorunScriptConfigItem)rows[0].DataBoundItem;
                    int ind = CurrentConfig.AutoRunScripts.IndexOf(item);
                    int count = CurrentConfig.AutoRunScripts.Count;
                    if (ind < count - 1)
                    {
                        dataGridView1.DataSource = null;
                        item.Order = item.Order + 1;
                        CurrentConfig.AutoRunScripts[ind + 1].Order = CurrentConfig.AutoRunScripts[ind + 1].Order - 1;

                        CurrentConfig.AutoRunScripts.Sort((a, b) => a.Order.CompareTo(b.Order));

                        dataGridView1.DataSource = CurrentConfig.AutoRunScripts;
                        dataGridView1.Refresh();
                    }
                }
            }
        }

        private void btnSaveChanges_Click(object sender, System.EventArgs e)
        {
            //Save Autorun Config.
            AutorunScriptUtils.SaveConfig(CurrentConfig);
            MessageBox.Show("Config saved successfully!");
            SPCoderForm.MainForm.AppendToLog("Autorun config saved successfully!");
        }

        private void btnView_Click(object sender, System.EventArgs e)
        {
            //Open the Config file in the New Script window on main Form
            var rows = dataGridView1.SelectedRows;
            //TODO: Here check if any cell is selected, and not just the whole row
            if (rows != null && rows.Count == 1)
            {
                //Get the row, calculate absolute path of the script and open it in main window
                AutorunScriptConfigItem item = (AutorunScriptConfigItem) rows[0].DataBoundItem;
                string fullPath = ConfigUtils.GetFullPathToConfigFile(item.Path);
                SPCoderForm.MainForm.GenerateNewSourceTab(item.Title, item.Source, fullPath);
            }
        }

        private void btnDelete_Click(object sender, System.EventArgs e)
        {
            var rows = dataGridView1.SelectedRows;
            if (rows != null && rows.Count == 1)
            {
                AutorunScriptConfigItem item = (AutorunScriptConfigItem) rows[0].DataBoundItem;
                var result = MessageBox.Show(string.Format("Are you sure that you want to delete '{0}' autorun script?", item.Title)
                    ,"Delete autorun script",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dataGridView1.DataSource = null;
                    CurrentConfig.AutoRunScripts.Remove(item);
                    ReorderItems();
                    
                    //Refresh data in grid
                    dataGridView1.DataSource = CurrentConfig.AutoRunScripts;
                    dataGridView1.Refresh();
                }
            }
        }

        private void ReorderItems()
        {
            if (CurrentConfig.AutoRunScripts.Count > 0)
            {
                CurrentConfig.AutoRunScripts[0].Order = 1;
                CurrentConfig.AutoRunScripts.Sort((a, b) => a.Order.CompareTo(b.Order));
                for (int i = 0; i < CurrentConfig.AutoRunScripts.Count; i++)
                {
                    var item = CurrentConfig.AutoRunScripts[i];
                    if (i < CurrentConfig.AutoRunScripts.Count - 1)
                    {
                        var nextItem = CurrentConfig.AutoRunScripts[i + 1];
                        if (nextItem.Order - item.Order > 1)
                        {
                            nextItem.Order = item.Order + 1;
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Autorun scripts must be added from main code window by selecting 'Save As Autorun' button!");
        }
    }
}