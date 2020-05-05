namespace SPCoder.HelperWindows
{
    partial class CryptoHelperWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CryptoHelperWindow));
            this.lblText = new System.Windows.Forms.Label();
            this.lblEncrypted = new System.Windows.Forms.Label();
            this.txtClearText = new System.Windows.Forms.TextBox();
            this.txtEncryptedText = new System.Windows.Forms.TextBox();
            this.chkMaskText = new System.Windows.Forms.CheckBox();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(30, 44);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(51, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Clear text";
            // 
            // lblEncrypted
            // 
            this.lblEncrypted.AutoSize = true;
            this.lblEncrypted.Location = new System.Drawing.Point(30, 76);
            this.lblEncrypted.Name = "lblEncrypted";
            this.lblEncrypted.Size = new System.Drawing.Size(75, 13);
            this.lblEncrypted.TabIndex = 1;
            this.lblEncrypted.Text = "Encrypted text";
            // 
            // txtClearText
            // 
            this.txtClearText.Location = new System.Drawing.Point(139, 41);
            this.txtClearText.Name = "txtClearText";
            this.txtClearText.Size = new System.Drawing.Size(300, 20);
            this.txtClearText.TabIndex = 2;
            this.txtClearText.UseSystemPasswordChar = true;
            // 
            // txtEncryptedText
            // 
            this.txtEncryptedText.Location = new System.Drawing.Point(139, 76);
            this.txtEncryptedText.Multiline = true;
            this.txtEncryptedText.Name = "txtEncryptedText";
            this.txtEncryptedText.Size = new System.Drawing.Size(300, 47);
            this.txtEncryptedText.TabIndex = 3;
            // 
            // chkMaskText
            // 
            this.chkMaskText.AutoSize = true;
            this.chkMaskText.Checked = true;
            this.chkMaskText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMaskText.Location = new System.Drawing.Point(139, 18);
            this.chkMaskText.Name = "chkMaskText";
            this.chkMaskText.Size = new System.Drawing.Size(72, 17);
            this.chkMaskText.TabIndex = 4;
            this.chkMaskText.Text = "Mask text";
            this.chkMaskText.UseVisualStyleBackColor = true;
            this.chkMaskText.CheckedChanged += new System.EventHandler(this.chkMaskText_CheckedChanged);
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.Location = new System.Drawing.Point(139, 143);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(75, 23);
            this.btnEncrypt.TabIndex = 5;
            this.btnEncrypt.Text = "Encrypt";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(258, 143);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(75, 23);
            this.btnDecrypt.TabIndex = 6;
            this.btnDecrypt.Text = "Decrypt";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // CryptoHelperWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 185);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.chkMaskText);
            this.Controls.Add(this.txtEncryptedText);
            this.Controls.Add(this.txtClearText);
            this.Controls.Add(this.lblEncrypted);
            this.Controls.Add(this.lblText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CryptoHelperWindow";
            this.Text = "Crypto Helper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblEncrypted;
        private System.Windows.Forms.TextBox txtClearText;
        private System.Windows.Forms.TextBox txtEncryptedText;
        private System.Windows.Forms.CheckBox chkMaskText;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Button btnDecrypt;
    }
}