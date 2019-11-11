using System;
using System.Windows.Forms;
using SPCoder.Context;

namespace SPCoder.HelperWindows
{
    public partial class ContextItemRenamer : Form
    {
        public ContextItemRenamer()
        {
            InitializeComponent();
        }
        
        public delegate void DoRenameContextItem (object item, string newName);
        public event DoRenameContextItem Rename;
        public object Item { get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Rename(Item, txtRenamer.Text.Trim());
                this.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);                
            }            
        }

        private void ContextItemRenamer_Load(object sender, EventArgs e)
        {
            txtRenamer.Text = ((ContextItem) Item).Name;
            txtRenamer.SelectAll();
        }

        private void txtRenamer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, null);
            }
        }
    }
}
