namespace SPCoder.Windows
{
    partial class ExplorerView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplorerView));
            this.tvSp = new Aga.Controls.Tree.TreeViewAdv();
            this._nodeStateIcon = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this._nodeTextBox = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.btnSpinner = new System.Windows.Forms.ToolStripButton();
            this.btnConnect = new System.Windows.Forms.ToolStripButton();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.tvContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbObjectModelType = new System.Windows.Forms.ComboBox();
            this.toolStrip3.SuspendLayout();
            this.tvContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvSp
            // 
            this.tvSp.AllowDrop = true;
            this.tvSp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvSp.AutoRowHeight = true;
            this.tvSp.BackColor = System.Drawing.SystemColors.Window;
            this.tvSp.ColumnHeaderHeight = 0;
            this.tvSp.DefaultToolTipProvider = null;
            this.tvSp.DisplayDraggingNodes = true;
            this.tvSp.DragDropMarkColor = System.Drawing.Color.Black;
            this.tvSp.FullRowSelectActiveColor = System.Drawing.Color.Empty;
            this.tvSp.FullRowSelectInactiveColor = System.Drawing.Color.Empty;
            this.tvSp.LineColor = System.Drawing.SystemColors.ControlDark;
            this.tvSp.LoadOnDemand = true;
            this.tvSp.Location = new System.Drawing.Point(0, 75);
            this.tvSp.Model = null;
            this.tvSp.Name = "tvSp";
            this.tvSp.NodeControls.Add(this._nodeStateIcon);
            this.tvSp.NodeControls.Add(this._nodeTextBox);
            this.tvSp.NodeFilter = null;
            this.tvSp.SelectedNode = null;
            this.tvSp.ShowNodeToolTips = true;
            this.tvSp.Size = new System.Drawing.Size(372, 397);
            this.tvSp.TabIndex = 1;
            this.tvSp.Text = "tvSp";
            this.tvSp.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvSp_ItemDrag_1);
            this.tvSp.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.tvSp_NodeMouseDoubleClick);
            this.tvSp.Expanding += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.tvSp_Expanding);
            this.tvSp.Expanded += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.tvSp_Expanded);
            this.tvSp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvSp_MouseUp);
            // 
            // _nodeStateIcon
            // 
            this._nodeStateIcon.DataPropertyName = "Image";
            this._nodeStateIcon.LeftMargin = 1;
            this._nodeStateIcon.ParentColumn = null;
            this._nodeStateIcon.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            // 
            // _nodeTextBox
            // 
            this._nodeTextBox.DataPropertyName = "Text";
            this._nodeTextBox.IncrementalSearchEnabled = true;
            this._nodeTextBox.LeftMargin = 3;
            this._nodeTextBox.ParentColumn = null;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSpinner,
            this.btnConnect});
            this.toolStrip3.Location = new System.Drawing.Point(0, 47);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(372, 25);
            this.toolStrip3.TabIndex = 3;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // btnSpinner
            // 
            this.btnSpinner.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSpinner.Image = global::SPCoder.Properties.Resources.tsSpinnerButton_Image;
            this.btnSpinner.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpinner.Name = "btnSpinner";
            this.btnSpinner.Size = new System.Drawing.Size(23, 22);
            this.btnSpinner.Text = "Work in progress";
            this.btnSpinner.Visible = false;
            // 
            // btnConnect
            // 
            this.btnConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnConnect.Image")));
            this.btnConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(56, 22);
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtUrl
            // 
            this.txtUrl.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUrl.Location = new System.Drawing.Point(0, 0);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(372, 23);
            this.txtUrl.TabIndex = 4;
            this.txtUrl.Text = "http://localhost";
            // 
            // tvContextMenu
            // 
            this.tvContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.downloadToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.tvContextMenu.Name = "tvContextMenu";
            this.tvContextMenu.Size = new System.Drawing.Size(129, 70);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.downloadToolStripMenuItem.Text = "Download";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // cbObjectModelType
            // 
            this.cbObjectModelType.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbObjectModelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbObjectModelType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbObjectModelType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbObjectModelType.FormattingEnabled = true;
            this.cbObjectModelType.ItemHeight = 16;
            this.cbObjectModelType.Location = new System.Drawing.Point(0, 23);
            this.cbObjectModelType.MaxDropDownItems = 20;
            this.cbObjectModelType.Name = "cbObjectModelType";
            this.cbObjectModelType.Size = new System.Drawing.Size(372, 24);
            this.cbObjectModelType.TabIndex = 5;
            // 
            // ExplorerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 471);
            this.Controls.Add(this.toolStrip3);
            this.Controls.Add(this.cbObjectModelType);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.tvSp);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExplorerView";
            this.Text = "Explorer";
            this.Load += new System.EventHandler(this.ExplorerView_Load);
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.tvContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Aga.Controls.Tree.TreeViewAdv tvSp;
        private Aga.Controls.Tree.NodeControls.NodeTextBox _nodeTextBox;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon _nodeStateIcon;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton btnConnect;
        private System.Windows.Forms.ToolStripButton btnSpinner;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.ContextMenuStrip tvContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ComboBox cbObjectModelType;
    }
}