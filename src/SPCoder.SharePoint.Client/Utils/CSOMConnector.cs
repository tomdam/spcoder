using Microsoft.Graph;
using Microsoft.SharePoint.Client;
using SPCoder.HelperWindows;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Windows.Forms;


namespace SPCoder.Utils
{
    public class CSOMConnector : BaseConnector
    {
        public ClientContext Context { get; set; }
        public CSOMConnector() : base()
        { }

        string RootNodeTitle = "SharePoint site: ";
        public CSOMConnector(string connectorType)
        {
            if (connectorType.Contains(SPCoderConstants.O365_APP))
                this.AuthenticationType = SPCoderConstants.O365_APP;
            else if (connectorType.Contains(SPCoderConstants.O365))
                this.AuthenticationType = SPCoderConstants.O365;
            else if (connectorType.Contains(SPCoderConstants.FBA))
                this.AuthenticationType = SPCoderConstants.FBA;
            else if (connectorType.Contains(SPCoderConstants.WIN))
                this.AuthenticationType = SPCoderConstants.WIN;
        }

        public CSOMConnector(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public override BaseNode ExpandNode(BaseNode node, bool doIfLoaded = false)
        {
            //If it is a web node
            if (node is WebNode)
            {
                //If not loaded
                if (!doIfLoaded)
                {
                    if (node.ParentNode.Children != null && node.ParentNode.Children.Contains(node))
                    {
                        node.ParentNode.Children.Remove(node);
                    }

                    node = DoSPWeb((Web)node.SPObject, node.ParentNode, node.RootNode);
                }
            }

            if (node is ListNode)
            {
                if (!doIfLoaded)
                {
                    if (node.ParentNode.Children != null && node.ParentNode.Children.Contains(node))
                    {
                        node.ParentNode.Children.Remove(node);
                    }

                    node = DoSPList((Microsoft.SharePoint.Client.List)node.SPObject, node.ParentNode, node.RootNode);
                }
            }

            if (node is FolderNode)
            {
                if (!doIfLoaded)
                {
                    if (node.ParentNode.Children != null && node.ParentNode.Children.Contains(node))
                    {
                        node.ParentNode.Children.Remove(node);
                    }

                    node = DoSPFolder((Microsoft.SharePoint.Client.Folder)node.SPObject, node.ParentNode, node.RootNode);
                }
            }

            return node;
        }
        public override BaseNode GetSPStructure(string siteUrl)
        {
            this.Endpoint = siteUrl;

            if (string.IsNullOrEmpty(this.Endpoint) || string.IsNullOrEmpty(this.Username) || string.IsNullOrEmpty(this.Password))
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.PortalUrl = siteUrl;
                loginWindow.Username = Username;
                loginWindow.Password = Password;

                if (this.AuthenticationType == SPCoderConstants.O365_APP)
                {
                    loginWindow.lblUsername.Text = "Client Id";
                    loginWindow.lblPassword.Text = "Client Secret";
                }

                loginWindow.StartPosition = FormStartPosition.CenterParent;
                var rez = loginWindow.ShowDialog();
                if (rez == DialogResult.OK)
                {
                    Username = loginWindow.Username;
                    Password = loginWindow.Password;
                    this.Endpoint = siteUrl = loginWindow.PortalUrl;
                    if (this.AuthenticationType == SPCoderConstants.O365_APP && !String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password))
                    {
                        ConfigurationManager.AppSettings["ClientId"] = Username;
                        ConfigurationManager.AppSettings["ClientSecret"] = Password;
                    }
                }
                else
                {
                    //If Close/Cancel has been clicked
                    return null;
                }
            }


            if (this.AuthenticationType == SPCoderConstants.O365)
            {
                Context = new ClientContext(siteUrl);
                SecureString pass = new SecureString();
                foreach (char c in Password.ToCharArray()) pass.AppendChar(c);
                Context.Credentials = new SharePointOnlineCredentials(Username, pass);
            }
            else if (this.AuthenticationType == SPCoderConstants.O365_APP)
            {
                //Get the realm for the URL
                string realm = SPCoder.SharePoint.Client.TokenHelper.GetRealmFromTargetUrl(new Uri(siteUrl));
                string accessToken = SPCoder.SharePoint.Client.TokenHelper.GetAppOnlyAccessToken(SPCoder.SharePoint.Client.TokenHelper.SharePointPrincipal, new Uri(siteUrl).Authority, realm).AccessToken;
                Context = SPCoder.SharePoint.Client.TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken);

            }
            else if (this.AuthenticationType == SPCoderConstants.FBA)
            {
                Context = new ClientContext(siteUrl);
                Context.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                Context.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(Username, Password);
            }
            else if (this.AuthenticationType == SPCoderConstants.WIN)
            {
                Context = new ClientContext(siteUrl);
                Context.AuthenticationMode = ClientAuthenticationMode.Default;
                Context.Credentials = new NetworkCredential(Username, Password);
            }

            var rootNode = this.GenerateRootNode();
            return rootNode;
        }

        public override BaseNode GenerateRootNode()
        {
            Microsoft.SharePoint.Client.Site site = Context.Site;
            Context.Load(site);
            Context.ExecuteQuery();
            BaseNode rootNode = new SiteNode(site);
            rootNode.Title = RootNodeTitle + rootNode.Title;
            rootNode.NodeConnector = this;
            rootNode.OMType = ObjectModelType.REMOTE;
            rootNode.SPObject = site;
            rootNode.LoadedData = true;
            DoSPWeb(site.RootWeb, rootNode, rootNode);
            return rootNode;
        }

        private BaseNode DoSPFolder(Microsoft.SharePoint.Client.Folder folder, BaseNode parentNode, BaseNode rootNode)
        {
            BaseNode myNode = null;
            folder.EnsureProperties(f => f.Folders, f => f.Files, f => f.Name, f => f.ServerRelativeUrl);

            try
            {
                myNode = new FolderNode(folder);
                parentNode.Children.Add(myNode);

                myNode.ParentNode = parentNode;
                myNode.RootNode = rootNode;
                myNode.NodeConnector = this;
                myNode.LoadedData = true;

                folder.Context.Load(folder.Folders);
                folder.Context.ExecuteQueryRetry();

                try
                {
                    foreach (var subfolder in folder.Folders.OrderBy(f => f.Name))
                    {
                        BaseNode childNode = new FolderNode(subfolder);
                        myNode.Children.Add(childNode);

                        childNode.ParentNode = parentNode;
                        childNode.RootNode = rootNode;
                        childNode.NodeConnector = this;
                    }

                    foreach(var file in folder.Files.OrderBy(f => f.Name))
                    {
                        BaseNode fileNode = new FileNode(file);
                        myNode.Children.Add(fileNode);

                        fileNode.ParentNode = parentNode;
                        fileNode.RootNode = rootNode;
                        fileNode.NodeConnector = this;

                        try
                        {
                            //
                            if (fileNode.Title != null && fileNode.Title.Contains("."))
                            {
                                var els = fileNode.Title.Split('.');
                                string extension = "." + els[els.Length - 1];
                                //Icon icon = Icon.ExtractAssociatedIcon(file.FullName);
                                Icon icon = ShellIcon.GetSmallIconFromExtension(extension);
                                fileNode.IconObject = icon.ToBitmap();
                                if (fileNode.IconObject.Width != 16)
                                {
                                    fileNode.IconObject = new Bitmap(fileNode.IconObject, 16, 16);
                                }/**/
                            }
                        }
                        catch (Exception)
                        {
                            //skip if exception happens here... the default icon will be shown
                        }
                    }
                }
                catch
                {
                    return myNode;
                }
            }
            catch
            {
                return myNode;
            }

            return myNode;
        }

        private BaseNode DoSPList(Microsoft.SharePoint.Client.List list, BaseNode parentNode, BaseNode rootNode)
        {         
            list.EnsureProperties(l => l.RootFolder, l => l.BaseType);

            return this.DoSPFolder(list.RootFolder, parentNode, rootNode);
        }

        private BaseNode DoSPWeb(Web web, BaseNode parentNode, BaseNode rootNode)
        {
            BaseNode myNode = null;
            try
            {
                myNode = new WebNode(web);
                parentNode.Children.Add(myNode);
                myNode.ParentNode = parentNode;
                myNode.RootNode = rootNode;
                myNode.NodeConnector = this;
                myNode.LoadedData = true;
                web.Context.Load(web.Webs);
                web.Context.Load(web.Lists);

                web.Context.ExecuteQuery();
                try
                {
                    foreach (Web childWeb in web.Webs)
                    {
                        //doSPWeb(childWeb, myNode, rootNode);
                        //Draw the nodes - user will expand them later if necessary
                        BaseNode childNode = new WebNode(childWeb);
                        myNode.Children.Add(childNode);
                        childNode.ParentNode = parentNode;
                        childNode.RootNode = rootNode;
                        childNode.NodeConnector = this;
                    }
                }
                catch (Exception)
                {
                    return myNode;
                }

                foreach (Microsoft.SharePoint.Client.List list in web.Lists)
                {
                    BaseNode myListNode = new ListNode(list);
                    myNode.Children.Add(myListNode);
                    myListNode.ParentNode = myNode;
                    myListNode.RootNode = rootNode;
                    myListNode.NodeConnector = this;
                }
                return myNode;
            }
            catch (Exception)
            {
                return myNode;
            }
        }

        public override string ImagesPath
        {
            get
            {
                return Endpoint + "/_layouts/15/images/";
            }
            set
            { }
        }

        public override bool IsImagesPathLocal
        {
            get
            {
                return false;
            }
            set
            { }
        }

        public override List<object> AutoAddToContext()
        {
            List<object> objects = new List<object>();
            objects.Add(this.Context);
            objects.Add(this.Context.Web);
            objects.Add(this.Context.Site);
            return objects;
        }
    }
}
