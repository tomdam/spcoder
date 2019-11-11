namespace SPCoder.Describer
{
    partial class DescriberForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DescriberForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.numCounter = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cb_wordwrap = new System.Windows.Forms.CheckBox();
            this.linkMsdn = new System.Windows.Forms.LinkLabel();
            this.chkIsEditable = new System.Windows.Forms.CheckBox();
            this.txtMaxDisplaySize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDescribe = new System.Windows.Forms.Button();
            this.txtDescribeObject = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDoSortAsc = new System.Windows.Forms.Button();
            this.cmbSortByWhat = new System.Windows.Forms.ComboBox();
            this.btnDoSortDesc = new System.Windows.Forms.Button();
            this.fctb = new FastColoredTextBoxNS.FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCounter)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fctb)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.numCounter);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.btnDescribe);
            this.splitContainer1.Panel1.Controls.Add(this.txtDescribeObject);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1MinSize = 150;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.fctb);
            this.splitContainer1.Size = new System.Drawing.Size(693, 727);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 0;
            // 
            // numCounter
            // 
            this.numCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numCounter.Location = new System.Drawing.Point(633, 100);
            this.numCounter.Name = "numCounter";
            this.numCounter.Size = new System.Drawing.Size(48, 20);
            this.numCounter.TabIndex = 10;
            this.numCounter.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cb_wordwrap);
            this.groupBox4.Controls.Add(this.linkMsdn);
            this.groupBox4.Controls.Add(this.chkIsEditable);
            this.groupBox4.Controls.Add(this.txtMaxDisplaySize);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(201, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(373, 46);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Misc";
            // 
            // cb_wordwrap
            // 
            this.cb_wordwrap.AutoSize = true;
            this.cb_wordwrap.Location = new System.Drawing.Point(215, 13);
            this.cb_wordwrap.Name = "cb_wordwrap";
            this.cb_wordwrap.Size = new System.Drawing.Size(78, 17);
            this.cb_wordwrap.TabIndex = 6;
            this.cb_wordwrap.Text = "Word wrap";
            this.cb_wordwrap.UseVisualStyleBackColor = true;
            this.cb_wordwrap.CheckedChanged += new System.EventHandler(this.cb_wordwrap_CheckedChanged);
            // 
            // linkMsdn
            // 
            this.linkMsdn.AutoSize = true;
            this.linkMsdn.Location = new System.Drawing.Point(328, 18);
            this.linkMsdn.Name = "linkMsdn";
            this.linkMsdn.Size = new System.Drawing.Size(39, 13);
            this.linkMsdn.TabIndex = 5;
            this.linkMsdn.TabStop = true;
            this.linkMsdn.Text = "MSDN";
            this.linkMsdn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMsdn_LinkClicked);
            // 
            // chkIsEditable
            // 
            this.chkIsEditable.AutoSize = true;
            this.chkIsEditable.Location = new System.Drawing.Point(145, 14);
            this.chkIsEditable.Name = "chkIsEditable";
            this.chkIsEditable.Size = new System.Drawing.Size(64, 17);
            this.chkIsEditable.TabIndex = 4;
            this.chkIsEditable.Text = "Editable";
            this.chkIsEditable.UseVisualStyleBackColor = true;
            this.chkIsEditable.CheckedChanged += new System.EventHandler(this.chkIsEditable_CheckedChanged);
            // 
            // txtMaxDisplaySize
            // 
            this.txtMaxDisplaySize.Location = new System.Drawing.Point(94, 13);
            this.txtMaxDisplaySize.Name = "txtMaxDisplaySize";
            this.txtMaxDisplaySize.Size = new System.Drawing.Size(41, 20);
            this.txtMaxDisplaySize.TabIndex = 3;
            this.txtMaxDisplaySize.Text = "512";
            this.txtMaxDisplaySize.TextChanged += new System.EventHandler(this.txtMaxDisplaySize_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Max Value Size:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.flowLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(8, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(673, 39);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Bread crumbs";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(667, 20);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnDescribe
            // 
            this.btnDescribe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDescribe.Location = new System.Drawing.Point(571, 99);
            this.btnDescribe.Name = "btnDescribe";
            this.btnDescribe.Size = new System.Drawing.Size(60, 23);
            this.btnDescribe.TabIndex = 5;
            this.btnDescribe.Text = "Describe";
            this.btnDescribe.UseVisualStyleBackColor = true;
            this.btnDescribe.Click += new System.EventHandler(this.btnDescribe_Click);
            // 
            // txtDescribeObject
            // 
            this.txtDescribeObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescribeObject.Location = new System.Drawing.Point(7, 102);
            this.txtDescribeObject.Name = "txtDescribeObject";
            this.txtDescribeObject.Size = new System.Drawing.Size(558, 20);
            this.txtDescribeObject.TabIndex = 4;
            this.txtDescribeObject.TextChanged += new System.EventHandler(this.txtDescribeObject_TextChanged);
            this.txtDescribeObject.VisibleChanged += new System.EventHandler(this.txtDescribeObject_VisibleChanged);
            this.txtDescribeObject.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDescribeObject_KeyPress);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnDoSortAsc);
            this.groupBox3.Controls.Add(this.cmbSortByWhat);
            this.groupBox3.Controls.Add(this.btnDoSortDesc);
            this.groupBox3.Location = new System.Drawing.Point(7, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(188, 46);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sort";
            // 
            // btnDoSortAsc
            // 
            this.btnDoSortAsc.Location = new System.Drawing.Point(87, 14);
            this.btnDoSortAsc.Name = "btnDoSortAsc";
            this.btnDoSortAsc.Size = new System.Drawing.Size(37, 23);
            this.btnDoSortAsc.TabIndex = 5;
            this.btnDoSortAsc.Text = "Asc";
            this.btnDoSortAsc.UseVisualStyleBackColor = true;
            this.btnDoSortAsc.Click += new System.EventHandler(this.btnDoSortAsc_Click);
            // 
            // cmbSortByWhat
            // 
            this.cmbSortByWhat.Items.AddRange(new object[] {
            "Names",
            "Values"});
            this.cmbSortByWhat.Location = new System.Drawing.Point(6, 15);
            this.cmbSortByWhat.Name = "cmbSortByWhat";
            this.cmbSortByWhat.Size = new System.Drawing.Size(64, 21);
            this.cmbSortByWhat.TabIndex = 3;
            this.cmbSortByWhat.Text = "Names";
            // 
            // btnDoSortDesc
            // 
            this.btnDoSortDesc.Location = new System.Drawing.Point(133, 14);
            this.btnDoSortDesc.Name = "btnDoSortDesc";
            this.btnDoSortDesc.Size = new System.Drawing.Size(42, 23);
            this.btnDoSortDesc.TabIndex = 4;
            this.btnDoSortDesc.Text = "Desc";
            this.btnDoSortDesc.UseVisualStyleBackColor = true;
            this.btnDoSortDesc.Click += new System.EventHandler(this.btnDoSortDesc_Click);
            // 
            // fctb
            // 
            this.fctb.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fctb.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.fctb.BackBrush = null;
            this.fctb.CharHeight = 14;
            this.fctb.CharWidth = 8;
            this.fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctb.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctb.IsReplaceMode = false;
            this.fctb.Location = new System.Drawing.Point(0, 0);
            this.fctb.Name = "fctb";
            this.fctb.Paddings = new System.Windows.Forms.Padding(0);
            this.fctb.ReadOnly = true;
            this.fctb.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctb.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fctb.ServiceColors")));
            this.fctb.Size = new System.Drawing.Size(693, 573);
            this.fctb.TabIndex = 0;
            this.fctb.Zoom = 100;
            this.fctb.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fctb_TextChangedDelayed);
            this.fctb.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fctb_MouseDoubleClick);
            this.fctb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fctb_MouseDown);
            this.fctb.MouseMove += new System.Windows.Forms.MouseEventHandler(this.fctb_MouseMove);
            // 
            // DescriberForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 727);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DescriberForm";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Hidden;
            this.Text = "Describer";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numCounter)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fctb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDoSortAsc;
        private System.Windows.Forms.ComboBox cmbSortByWhat;
        private System.Windows.Forms.Button btnDoSortDesc;
        private System.Windows.Forms.Button btnDescribe;
        private System.Windows.Forms.TextBox txtDescribeObject;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtMaxDisplaySize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkIsEditable;
        private System.Windows.Forms.NumericUpDown numCounter;
        private System.Windows.Forms.LinkLabel linkMsdn;
        private FastColoredTextBoxNS.FastColoredTextBox fctb;
        private System.Windows.Forms.CheckBox cb_wordwrap;

    }
}