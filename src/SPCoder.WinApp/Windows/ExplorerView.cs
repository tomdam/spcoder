using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using SPCoder.Config;
using SPCoder.Core.Utils;
using SPCoder.Utils;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using SPCoder.Properties;
using SPCoder.Core.Utils.Nodes;
using System.Diagnostics;
using SPCoder.SharePoint.Client.Utils;

namespace SPCoder.Windows
{
    public partial class ExplorerView : DockContent
    {
        #region Inner classes
        protected class SPTreeViewNode : Node
        {
            public override bool IsLeaf
            {
                get
                {
                    return (Tag is LeafNode);
                }
            }
        }

        private class ToolTipProvider : IToolTipProvider
        {
            public string GetToolTip(TreeNodeAdv node, NodeControl nodeControl)
            {
                return "Drag&Drop nodes to move them to context";
            }
        } 
        #endregion

        public ExplorerView()
        {
            InitializeComponent();
            
            _nodeTextBox.ToolTipProvider = new ToolTipProvider();
            _model = new TreeModel();
            tvSp.Model = _model;
            _childFont = new Font(tvSp.Font.FontFamily, 18, FontStyle.Bold);
            imagesUrlFolder = ConfigUtils.GetRootConfig().Properties.Find(s => s.Name == SPCoderConstants.IMAGES_FOLDER_PATH).Value;

            
            /**/if (SPCoderForm.MainForm.Modules == null)
            {
                //Workaround for .NET framework Bug related to MEF
                SPCoderForm.MainForm.Modules = new List<ModuleDescription>();
                //SPCoderForm.MainForm.Modules.Add(new FBModule());
                //SPCoderForm.MainForm.Modules.Add(new GithubModule());
                //SPCoderForm.MainForm.Modules.Add(new FSModule());
                //SPCoderForm.MainForm.Modules.Add(new WebModule());                
                SPCoderForm.MainForm.Modules.Add(new SharePointClientModule());                               
            }
            
            foreach (ModuleDescription module in SPCoderForm.MainForm.Modules)
            {
                if (module.ConnectorTypes != null)
                    foreach (string m in module.ConnectorTypes)
                        cbObjectModelType.Items.Add(m);
            }

            tvContextMenuEvent += tvContextMenuClicked;
            //Load last selected connector and url
            LoadSettings();
        }

        private void LoadSettings()
        {
            var explorerSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_EXPLORER];                        
            if (explorerSettings[SPCoderConstants.SP_SETTINGS_ADDRESS] != null)
            {
                string address = explorerSettings[SPCoderConstants.SP_SETTINGS_ADDRESS].ToString();
                this.txtUrl.Text = address;
            }

            if (explorerSettings[SPCoderConstants.SP_SETTINGS_CONNECTOR] != null)
            {
                string connector = explorerSettings[SPCoderConstants.SP_SETTINGS_CONNECTOR].ToString();
                int ind = cbObjectModelType.FindStringExact(connector);
                if (ind > -1)
                {
                    cbObjectModelType.SelectedIndex = ind;
                }
            }
        }

        private void SetAddress(string address)
        {
            var explorerSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_EXPLORER];
            this.txtUrl.Text = address;
            explorerSettings[SPCoderConstants.SP_SETTINGS_ADDRESS] = address;
        }

        private void SetConnector(string connector)
        {
            var explorerSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_EXPLORER];
            int ind = cbObjectModelType.FindStringExact(connector);
            if (ind > -1)
            {
                explorerSettings[SPCoderConstants.SP_SETTINGS_CONNECTOR] = connector;
                cbObjectModelType.SelectedIndex = ind;
            }
        }

        private string imagesUrlFolder = "";
        public List<BaseNode> Nodes = new List<BaseNode>();
        //public SPSite Site;
        private TreeModel _model;
        private Font _childFont;
        //private string _currentSiteUrl;

        public void Connect(string siteUrl, string omType)
        {
            Connect(siteUrl, omType, null, null);
        }

        public void Connect(string siteUrl, string omType, string username, string password)
        {
            try
            {
                SPCoderForm.MainForm.AppendToLog("Before connecting to: " + siteUrl);
                Application.DoEvents();
                //BaseNode rootNode = null;
                //ObjectModelType objectModelType = ObjectModelType.LOCAL;
                BaseConnector connector = null;

                string selectedType = omType;
                if (string.IsNullOrEmpty(omType))
                {
                    cbObjectModelType.SelectedItem.ToString();
                }

                foreach (ModuleDescription module in SPCoderForm.MainForm.Modules)
                {
                    if (module.ConnectorTypes != null)                        
                        if (module.ConnectorTypes.Contains(selectedType))                            
                            {
                                connector = module.GetConnector(selectedType);
                                break;
                            }                            
                }
                if (connector == null)
                {
                    SPCoderForm.MainForm.LogError("Cannot connect to: " + siteUrl + ". Connector " + selectedType + " does not exist.");
                    return;
                }

                //write details to form
                //this.txtUrl.Text = siteUrl;
                this.SetAddress(siteUrl);
                //cbObjectModelType.SelectedValue = omType;
                //cbObjectModelType.SelectedIndex = cbObjectModelType.FindStringExact(omType);
                this.SetConnector(omType);
                //write details to form

                connector.Username = username;
                connector.Password = password;
                connector.Endpoint = siteUrl;

                imagesUrlFolder = ConfigUtils.GetRootConfig().Properties.Find(s => s.Name == SPCoderConstants.IMAGES_FOLDER_PATH).Value;
                //imagesUrlFolder = siteUrl + "/_layouts/images/";
                
                //if this is not the absolute path, append it to Environment.CurrentDirectory
                imagesUrlFolder = Path.GetFullPath(imagesUrlFolder);

                //Now create tree view stuff;
                GetSPStructureDelegate getStructureScript = new GetSPStructureDelegate(GetStructureAndDrawTree);
                //execScript(script);                
                //disable connect button
                btnConnect.Enabled = false;
                btnSpinner.Visible = true;
                getStructureScript.BeginInvoke(siteUrl, connector, null, null);

                //rootNode = connector.GetSPStructure(siteUrl);
                //tvSp.BeginUpdate();
                //CreateAllTreeViewNodes(rootNode, objectModelType);
                //tvSp.EndUpdate();
            }
            catch (Exception exc)
            {
                //Log
                SPCoderForm.MainForm.LogException(exc);
            }
            finally
            {
                //SPCoderForm.MainForm.AppendToLog("After connecting to: " + siteUrl);
            }
        }

        public void GetStructureAndDrawTree(string siteUrl, BaseConnector connector)
        {
            try
            {
                SPCoderForm.MainForm.SetAppStatus("Connecting to: " + siteUrl);
                BaseNode rootNode = connector.GetSPStructure(siteUrl);
                SPCoderForm.MainForm.AppendToLog("Retrieved data.");
                if (rootNode == null)
                {
                    //hide spinner
                    btnConnect.Enabled = true;
                    btnSpinner.Visible = false;
                    return;
                }
                DrawTreeView(rootNode, siteUrl);

                //if it is CSOM connector, add the clientContext object to the SPCoder context
                var objectsForContext = connector.AutoAddToContext();
                if (objectsForContext != null)
                {
                    foreach (var obj in objectsForContext)
                    {
                        SPCoderForm.MainForm.AddToContext(obj);
                    }
                }

                /*if (connector is CSOMConnector)
                {
                    //SPCoderForm.MainForm.AddToContext(((CSOMConnector)connector).Context);
                    SPCoderForm.MainForm.AddToContext(((CSOMConnector)connector).ConnectorContext);
                }*/
            }
            catch (Exception exc)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        btnConnect.Enabled = true;
                        btnSpinner.Visible = false;
                    });
                }
                else
                {
                    btnConnect.Enabled = true;
                    btnSpinner.Visible = false;
                }
                
                SPCoderForm.MainForm.LogException(exc);
            }
            finally
            {
                SPCoderForm.MainForm.SetAppStatus("Ready");
            }
        }

        public void DrawTreeView(BaseNode rootNode, string siteUrl)
        {
            if (tvSp.InvokeRequired)
            {
                DrawTreeViewDelegate d = new DrawTreeViewDelegate(DrawTreeView);
                this.Invoke(d, new object[] { rootNode, siteUrl });
            }
            else
            {
                try
                {
                    SPCoderForm.MainForm.AppendToLog("Creating tree view.");
                    tvSp.BeginUpdate();
                    CreateAllTreeViewNodes(rootNode, rootNode.OMType);
                    tvSp.EndUpdate();
                    btnConnect.Enabled = true;
                    btnSpinner.Visible = false;
                }
                catch (Exception exc)
                {
                    btnConnect.Enabled = true;
                    btnSpinner.Visible = false;
                    SPCoderForm.MainForm.LogException(exc);
                }
                finally
                {
                    SPCoderForm.MainForm.AppendToLog("Finished creating tree view.");
                    SPCoderForm.MainForm.AppendToLog("After connecting to: " + siteUrl);
                }
            }
        }
        delegate void DrawTreeViewDelegate(BaseNode rootNode, string siteUrl);
        delegate void GetSPStructureDelegate(string script, BaseConnector connector);

        public void Connect(string siteUrl)
        {
            Connect(siteUrl, "SharePoint Server Side");
        }

        private void CreateAllTreeViewNodes(BaseNode rootNode, ObjectModelType objectModelType)
        {
            client = new WebClient();
            SPTreeViewNode root = CreateNode(rootNode.Title, rootNode.IconPath, rootNode, objectModelType);
            _model.Nodes.Add(root);

            CreateTreeViewNode(root, rootNode.Children[0], objectModelType);
            
            client.Dispose();
        }

        private void CreateTreeViewNode(Node tvNode, BaseNode node, ObjectModelType objectModelType)
        {
            SPTreeViewNode myNode = CreateNode(node.Title, node.IconPath, node, objectModelType);
            myNode.Parent = tvNode;
            
            tvNode.Nodes.Add(myNode);
            foreach (BaseNode child in node.Children)
            {
                CreateTreeViewNode(myNode, child, objectModelType);
            }
        }

        private void RecreateRootTreeViewChildren(Node tvNode, BaseNode rootNode, ObjectModelType objectModelType)
        {
            client = new WebClient();
            
            CreateTreeViewNode(tvNode, rootNode.Children[0], objectModelType);

            client.Dispose();
        }

        Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
        //For fetching images
        WebClient client;

        private SPTreeViewNode CreateNode(string tekst, string imageUrl, BaseNode o, ObjectModelType modelType)
        {
            string localImagesFolder = Path.Combine(imagesUrlFolder, o.LocalImagesSubfolder ?? "");
            Bitmap bitmap = null;
            if (o.IconObject != null)
            {
                bitmap = o.IconObject;
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        string[] s = imageUrl.Split('/');
                        if (s.Length > 0)
                            imageUrl = s[s.Length - 1];
                        if (imageUrl.Contains("?"))
                        {
                            imageUrl = imageUrl.Substring(0, imageUrl.IndexOf('?'));
                        }
                        if (images.ContainsKey(imageUrl))
                        {
                            bitmap = images[imageUrl];
                        }
                        else
                        {
                            string localImagePath = Path.Combine(localImagesFolder, imageUrl);
                            if (!File.Exists(localImagePath))
                            {
                                string remoteImagePath = Path.Combine(o.NodeConnector.ImagesPath, imageUrl);
                                using (var stream = client.OpenRead(remoteImagePath))
                                {
                                    using (var file = File.Create(localImagePath))
                                    {
                                        stream.CopyTo(file);
                                    }
                                }
                            }
                            if (imageUrl.ToLower().EndsWith(".gif") || imageUrl.ToLower().EndsWith(".png") || imageUrl.ToLower().EndsWith(".jpg") || imageUrl.ToLower().EndsWith(".bmp"))
                            {
                                bitmap = new Bitmap(localImagePath);
                                /*if (modelType == ObjectModelType.LOCAL)
                                {
                                    //bitmap = new Bitmap(Path.Combine(imagesUrlFolder, imageUrl));
                                    bitmap = new Bitmap(localImagePath);
                                }
                                else
                                {
                                    bitmap = new Bitmap(client.OpenRead(Path.Combine(imagesUrlFolder, imageUrl)));
                                } */                               
                            }
                            else if (imageUrl.ToLower().EndsWith(".ico"))
                            {
                                Icon icon = new Icon(localImagePath);
                                bitmap = icon.ToBitmap();
                                /*if (modelType == ObjectModelType.LOCAL)
                                {
                                    Icon icon = new Icon(Path.Combine(imagesUrlFolder, imageUrl));
                                    bitmap = icon.ToBitmap();
                                }
                                else
                                {
                                    Icon icon = new Icon(client.OpenRead(Path.Combine(imagesUrlFolder, imageUrl)));
                                    bitmap = icon.ToBitmap();
                                }*/
                            }
                            if (bitmap.Width != 16)
                            {
                                bitmap = new Bitmap(bitmap, 16, 16);
                            }
                            images[imageUrl] = bitmap;
                        }
                    }
                }
                catch (Exception)
                {
                    //SPCoderForm.MainForm.LogException(exc);
                }
            }

            SPTreeViewNode node = new SPTreeViewNode { Text = tekst, Tag = o };
            
            if (bitmap != null) node.Image = bitmap;
            return node;
        }

        

        

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string omType = cbObjectModelType.SelectedItem.ToString();
            Connect(txtUrl.Text, omType);
        }

        private void tvSp_ItemDrag_1(object sender, ItemDragEventArgs e)
        {
            try
            {                
                SPCoderForm.MainForm.DragedBaseNode = ((Node)((TreeNodeAdv[])e.Item)[0].Tag).Tag as BaseNode;
                if (SPCoderForm.MainForm.DragedBaseNode != null && SPCoderForm.MainForm.DragedBaseNode.SPObject != null)
                {
                    tvSp.DoDragDrop(SPCoderForm.MainForm.DragedBaseNode.SPObject, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
            catch (Exception exc)
            {
                //this can happen when user tries to drop object to a window different than context
                SPCoderForm.MainForm.LogException(exc);
            }
        }

        public void DisposeNodes()
        {
            foreach (BaseNode node in Nodes)
            {
                DisposeBaseNodes(node);
            }
        }

        private void DisposeBaseNodes(BaseNode node)
        {
            if (node != null)
            {
                if (node != null && node.SPObject != null && node.SPObject is IDisposable)
                {
                    try
                    {
                        ((IDisposable)node.SPObject).Dispose();
                    }
                    catch (Exception)
                    {
                        //just skip this
                    }
                }

                foreach (BaseNode treeNode in node.Children)
                {
                    DisposeBaseNodes(treeNode);
                }
            }
        }

        private void txtUrl_KeyPress(object sender, KeyPressEventArgs e)
        {            
            if (e.KeyChar == (char)Keys.Enter)
            {
                toolStripButton1_Click(sender, null);
            }
        }

        private void ExplorerView_Load(object sender, EventArgs e)
        {
            //if (cbObjectModelType != null && cbObjectModelType.Items.Count > 1)
              //  cbObjectModelType.SelectedIndex = 0;
        }

        private void tvSp_Expanding(object sender, TreeViewAdvEventArgs e)
        {
                      
        }

        private void CreateTreeViewNode2(Node tvNode, BaseNode node, ObjectModelType objectModelType)
        {
            RemoveAllChildren(tvNode);
            foreach (BaseNode child in node.Children)
            {
                SPTreeViewNode myNode = CreateNode(child.Title, child.IconPath, child, objectModelType);
                myNode.Parent = tvNode;

                tvNode.Nodes.Add(myNode);
            }
        }

        private void RemoveAllChildren(Node tvNode)
        {
            var cnt = tvNode.Nodes.Count;
            while (cnt-- > 0)
            {
                try
                {
                    tvNode.Nodes.RemoveAt(0);
                }
                catch (Exception)
                {
                }
            }
        }

        private void tvSp_Expanded(object sender, TreeViewAdvEventArgs e)
        {
            if (e == null || e.Node == null || e.Node.Tag == null)
                return;
            
            var node = ((Node)e.Node.Tag).Tag as BaseNode;
            
            if (node != null && node.RootNode != null && node.RootNode.NodeConnector != null)
            {
                if (!node.LoadedData)
                {
                    try
                    {
                        var oldImage = ((Node)e.Node.Tag).Image;
                        ((Node)e.Node.Tag).Image = Resources.tsSpinnerButton_Image_small; // btnSpinner.Image;
                        //SPCoderForm.MainForm.AppendToLog("Expanding " + node.Url);
                        //SPCoderForm.MainForm.AppendToLog("Retrieving data " + node.Url);
                        var newNode = node.RootNode.NodeConnector.ExpandNode(node);
                        //SPCoderForm.MainForm.AppendToLog("Data retrieved " + node.Url);
                        
                        CreateTreeViewNode2((Node)e.Node.Tag, newNode, newNode.RootNode.OMType);
                        
                        node.LoadedData = true;
                        ((Node)e.Node.Tag).Image = oldImage;
                    }
                    catch (Exception exc)
                    {
                        SPCoderForm.MainForm.LogException(exc);
                    }
                    //TODO: check if it is in context and refresh it.
                }
            }
        }

        private void tvSp_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            //check if there is a default action and execute it
            if (e == null || e.Node == null || e.Node.Tag == null)
                return;
            var node = ((Node)e.Node.Tag).Tag as BaseNode;
            if (node != null)
            {
                //default action will be to open the file in new tab (we will later add different options)
                //GenerateNewSourceTab
                object source = node.GetDefaultSource();
                if (source != null)
                {
                    if (source is OpenActionResult)
                    {
                        var _source = source as OpenActionResult;
                        SPCoderForm.MainForm.GenerateNewSourceTab(node.Title, _source.Source.ToString(), _source.Url, _source.Language);
                    }
                    else
                    {
                        SPCoderForm.MainForm.GenerateNewSourceTab(node.Title, source.ToString(), null);
                    }
                    //SPCoderForm.MainForm.GenerateNewSourceTab(node.Title, source, null);
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string omType = cbObjectModelType.SelectedItem.ToString();
            Connect(txtUrl.Text, omType);
        }

        private void tvSp_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);
                TreeNodeAdv clickedNode = tvSp.GetNodeAt(p);

                if (e == null || clickedNode == null || clickedNode.Tag == null)
                    return;
                var node = ((Node)clickedNode.Tag).Tag as BaseNode;
                if (node != null)
                {
                    //default action will be to open the file in new tab (we will later add different options)
                    //GenerateNewSourceTab
                    tvContextMenu.Items.Clear();
                    var actions = node.GetNodeActions();
                    if (actions != null && actions.Count > 0)
                    {
                        foreach (var action in actions)
                        {
                            ToolStripItem item = tvContextMenu.Items.Add(action.Name, null, tvContextMenuEvent);
                            item.Tag = action;
                            action.Tag = clickedNode;
                        }

                        tvContextMenu.Show(tvSp, p);
                    }
                }
            }
        }
        public event EventHandler tvContextMenuEvent;
        //public 

        delegate void ExecutePluginDelegate(object sender, EventArgs param);

        //public void ExecutePlugin(object sender, EventArgs param)
        //{
        //    tvContextMenuClickedAsync(sender, param);
        //}
        public void tvContextMenuClicked(object sender, EventArgs param)
        {
            //ExecutePlugin(sender, param);
            try
            {
                ExecutePluginDelegate execScript = new ExecutePluginDelegate(tvContextMenuClickedAsync);
                //execScript(script);
                execScript.BeginInvoke(sender, param, null, null);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        public void tvContextMenuClickedAsync(object sender, EventArgs param)
        {
            if (this.tvContextMenuEvent != null && ((ToolStripItem)sender).Tag != null)
            {
                //do something
                BaseActionItem action = ((BaseActionItem)((ToolStripItem)sender).Tag);
                switch (action.Action)
                {
                    case NodeActions.Open:
                        //get the code (string) and open it in new source window
                        object source = action.Node.ExecuteAction(action);
                        if (source != null)
                        {
                            //
                            if (source is OpenActionResult)
                            {
                                var _source = source as OpenActionResult;
                                SPCoderForm.MainForm.GenerateNewSourceTab(action.Node.Title, _source.Source.ToString(), _source.Url, _source.Language);
                            }
                            else
                            {
                                SPCoderForm.MainForm.GenerateNewSourceTab(action.Node.Title, source.ToString(), null);
                            }
                        }
                        break;
                    case NodeActions.Save:
                        break;
                    case NodeActions.Close:
                        //close works only on root
                        var autoAddedObjects = action.Node.NodeConnector.AutoAddToContext();

                        if (autoAddedObjects != null && autoAddedObjects.Count > 0)
                        {
                            foreach(var obj in autoAddedObjects)
                            {
                                SPCoderForm.MainForm.RemoveDataItemFromContext(obj);
                            }
                        }

                        object closedObject = action.Node.ExecuteAction(action);
                        

                        if (action.Node != null && action.Node.RootNode == null)
                        {
                            var tvNode = (Node)((TreeNodeAdv)action.Tag).Tag;
                            _model.Nodes.Remove(tvNode);
                        }

                        break;
                    case NodeActions.ExternalOpen:
                        object path = action.Node.ExecuteAction(action);
                        if (path != null)
                        {
                            Process.Start(path.ToString());
                        }
                        break;
                    case NodeActions.Refresh:
                        
                        BaseNode refreshedNode = (BaseNode)action.Node.ExecuteAction(action);
                        //if root, do it differently

                        //GenerateRootNode()
                        if (refreshedNode.RootNode != null)
                        {
                            refreshedNode = refreshedNode.RootNode.NodeConnector.ExpandNode(refreshedNode);

                            if (refreshedNode != null)
                            {
                                CreateTreeViewNode2((Node)((TreeNodeAdv)action.Tag).Tag, refreshedNode, refreshedNode.RootNode.OMType);
                            }
                        }
                        else
                        {
                            //refreshing root is different
                            refreshedNode = refreshedNode.NodeConnector.GenerateRootNode();
                            var tvNode = (Node)((TreeNodeAdv)action.Tag).Tag;
                            RemoveAllChildren(tvNode);
                            //CreateTreeViewNodeChildren(tvNode, refreshedNode, refreshedNode.OMType);
                            RecreateRootTreeViewChildren(tvNode, refreshedNode, refreshedNode.OMType);
                        }
                        break;
                    case NodeActions.None:
                        //Might be some special action not related to the UI
                        action.Node.ExecuteAction(action);
                        break;
                    case NodeActions.Copy:
                        //
                        object value = action.Node.ExecuteAction(action);
                        if (value != null)
                        {
                            
                            if (this.InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    Clipboard.SetText(value.ToString());
                                });
                            }
                            else
                            {
                                Clipboard.SetText(value.ToString());
                            }
                        }

                        break;
                    case NodeActions.Plugin:
                        //
                        object valueObject = action.Node.ExecuteAction(action);
                        if (valueObject != null)
                        {
                            try
                            {
                                action.Plugin.Execute(valueObject);
                            }
                            catch(Exception ex)
                            {
                                SPCoderForm.MainForm.LogException(ex);
                                //SPCoderLogging.Logger.Error($"Error in Plugin: {ex.ToString()}");
                            }
                        }
                        break;
                }
            }
        }

    }
}
