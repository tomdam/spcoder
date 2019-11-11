using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPCoder.HelperWindows
{
    public partial class LoginWindow : Form
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        public string PortalUrl
        {
            get { return this.txtPortalUrl.Text; }
            set { this.txtPortalUrl.Text = value; }
        }

        public string Password
        {
            get { return this.txtPassword.Text; }
            set { this.txtPassword.Text = value; }
        }

        public string Username
        {
            get { return this.txtUsername.Text; }
            set { this.txtUsername.Text = value; }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //this.DialogResult = new SPLoginDialogResult();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
