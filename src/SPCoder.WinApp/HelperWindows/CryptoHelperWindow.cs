using SPCoder.Utils;
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
    public partial class CryptoHelperWindow : Form
    {
        public CryptoHelperWindow()
        {
            InitializeComponent();
        }

        private void chkMaskText_CheckedChanged(object sender, EventArgs e)
        {
            this.txtClearText.UseSystemPasswordChar = this.chkMaskText.Checked;
            /*if (this.chkMaskText.Checked)
            {
                this.txtClearText.UseSystemPasswordChar = '*';
            }
            else
            {
                this.txtClearText.pas
            }*/
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            this.txtEncryptedText.Text = CryptoHelper.Encrypt(SPCoderConstants.SP_CODER_CONTAINER_KEY_NAME, txtClearText.Text);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            this.txtClearText.Text = CryptoHelper.Decrypt(SPCoderConstants.SP_CODER_CONTAINER_KEY_NAME, txtEncryptedText.Text);
        }
    }
}
