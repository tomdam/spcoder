using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SPCoder.Windows
{
    public partial class SPCoderInnerWindow : DockContent
    {
        public SPCoderInnerWindow()
        {
            InitializeComponent();
            TabPageContextMenuStrip = this.contextMenuStrip1;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = this.GetFilePath();
            if (path != null)
            {
                path = Path.GetDirectoryName(path);
                Process.Start(path);
            }
        }

        private void openInExternalAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = this.GetFilePath();
            if (path != null)
            {
                Process.Start(path);
            }
        }

        public virtual string GetFilePath()
        {
            string path = Directory.GetCurrentDirectory();
            return path;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            string path = this.GetFilePath();
            if (path == null)
            {
                openDirectoryToolStripMenuItem.Enabled = false;
                openInExternalAppToolStripMenuItem.Enabled = false;
            }
            else
            {
                openDirectoryToolStripMenuItem.Enabled = true;
                openInExternalAppToolStripMenuItem.Enabled = true;
            }
        }
    }
}
