namespace SPCoder.Describer
{
    partial class DescriberProperties
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DescriberProperties));
            this.chkIsEditable = new System.Windows.Forms.CheckBox();
            this.txtMaxDisplaySize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // chkIsEditable
            // 
            this.chkIsEditable.AutoSize = true;
            this.chkIsEditable.Location = new System.Drawing.Point(12, 49);
            this.chkIsEditable.Name = "chkIsEditable";
            this.chkIsEditable.Size = new System.Drawing.Size(109, 17);
            this.chkIsEditable.TabIndex = 3;
            this.chkIsEditable.Text = "Is display editable";
            this.chkIsEditable.UseVisualStyleBackColor = true;
            this.chkIsEditable.CheckedChanged += new System.EventHandler(this.chkIsEditable_CheckedChanged);
            // 
            // txtMaxDisplaySize
            // 
            this.txtMaxDisplaySize.Location = new System.Drawing.Point(154, 17);
            this.txtMaxDisplaySize.Name = "txtMaxDisplaySize";
            this.txtMaxDisplaySize.Size = new System.Drawing.Size(41, 20);
            this.txtMaxDisplaySize.TabIndex = 1;
            this.txtMaxDisplaySize.Text = "2048";
            this.txtMaxDisplaySize.TextChanged += new System.EventHandler(this.txtMaxDisplaySize_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Max Displayed Size (bytes):";
            this.toolTip1.SetToolTip(this.label1, "The maximum size of the value of the displayed element.\r\n");
            // 
            // DescriberProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 100);
            this.Controls.Add(this.chkIsEditable);
            this.Controls.Add(this.txtMaxDisplaySize);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DescriberProperties";
            this.Text = "Describer Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkIsEditable;
        private System.Windows.Forms.TextBox txtMaxDisplaySize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}