using System.Net.Http;
using System.Net.Http.Headers;
using SPCoder.Utils;
using SPCoder.Utils.Nodes;
using System;
using System.Linq;
using SPCoder.Github.Utils.Nodes;
using System.Drawing;

namespace SPCoder.Github.Utils
{
    public class GithubConnector : BaseConnector 
    {

        public GithubConnector(string connectorType) : this()
        { }

        public GithubConnector()
        {
            
        }

        string reposUrl = "https://api.github.com/repos/";
        string RootNodeTitle = "Github repo: ";


        static HttpClient client = new HttpClient();

        private GithubObject[] TryGetGithubObjectWithUrl(string siteUrl)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SPCoder", "2.0.0.0"));


                var response = client.GetAsync(siteUrl);
                //if (response.)

                var githubObject = response.Result.Content.ReadAsStringAsync();

                System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                var ghRoot = ser.Deserialize<GithubObject[]>(githubObject.Result);
                return ghRoot;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        public override SPCoder.Utils.Nodes.BaseNode GetSPStructure(string siteUrl)
        {
            
            //try get with another url (change it to api...
            string newSiteUrl = siteUrl;
            if (siteUrl.StartsWith("https://github.com"))
            {
                string contentsSufix = (siteUrl.EndsWith("/") ? "" : "/");
                newSiteUrl = "https://api.github.com/repos/" + siteUrl.Replace("https://github.com/", "") + contentsSufix + "contents/";                    
            }

            var ghRoot = TryGetGithubObjectWithUrl(newSiteUrl);
            
            if (ghRoot == null)
            {
                throw new Exception("Wrong url. Please enter valid api.github url");
            }
            
            GithubObject rootFolder = new GithubObject();
            if (ghRoot != null && ghRoot.Length > 0)
            {
                var firstItem = ghRoot[0];
                string tmpRepos = firstItem.url.Replace(reposUrl, "");
                string[] tmpArr = tmpRepos.Split('/');
                if (tmpArr != null && tmpArr.Length > 1)
                    rootFolder.name = tmpArr[0] + " - " + tmpArr[1];
                else
                    rootFolder.name = tmpRepos;

                rootFolder.url = siteUrl;
                rootFolder.html_url = siteUrl.Replace("api.", "").Replace("repos/", "").Replace("contents/", "");
            }
            
            BaseNode rootNode = new GithubDirectoryNode(rootFolder);
            rootNode.Title = RootNodeTitle + rootFolder.name;
            rootNode.NodeConnector = this;
            rootNode.OMType = ObjectModelType.REMOTE;
            rootNode.LoadedData = true;
            rootNode.IconPath = "github.png";

            BaseNode my = new GithubDirectoryNode(rootFolder);
            my.NodeConnector = this;
            my.OMType = ObjectModelType.REMOTE;
            my.RootNode = rootNode;
            my.ParentNode = rootNode;
            my.Title = rootFolder.name;
            my.SPObject = rootFolder;
            my.LoadedData = true;

            rootNode.Children.Add(my);
            ghRoot = ghRoot.OrderBy(m => m.type).ThenBy(m => m.name).ToArray();

            foreach (var node in ghRoot)
            {
                BaseNode childNode = null;
                if (node.type == "file")
                {
                    childNode = new GithubFileNode(node);
                }
                else //dir
                {
                    childNode = new GithubDirectoryNode(node);
                }
                childNode.RootNode = rootNode;
                childNode.ParentNode = my;
                childNode.SPObject = node;

                try
                {
                    //
                    if (node.type == "file" && node.name != null && node.name.Contains("."))
                    {
                        var els = node.name.Split('.');
                        string extension = "." + els[els.Length - 1];
                        //Icon icon = Icon.ExtractAssociatedIcon(file.FullName);
                        Icon icon = ShellIcon.GetSmallIconFromExtension(extension);
                        childNode.IconObject = icon.ToBitmap();
                        if (childNode.IconObject.Width != 16)
                        {
                            childNode.IconObject = new Bitmap(childNode.IconObject, 16, 16);
                        }/**/
                    }
                }
                catch (Exception exc)
                {
                    //skip if exception happens here... the default icon will be shown
                }

                my.Children.Add(childNode);
            }
            

            //HtmlWeb page = new HtmlWeb();
            //HtmlDocument document = page.Load(siteUrl);
            //page.Get(siteUrl, "/");

            //rootNode.Title = page.ResponseUri.Host.ToString();




            //return rootNode;
            //rootNode.SPObject = site;
            //doPageNodes(document.DocumentNode, rootNode, rootNode);
            return rootNode;
        }

        public override BaseNode ExpandNode(BaseNode node, bool doIfLoaded = false)
        {
            //Ako je web node
            if (node is GithubDirectoryNode)
            {
                //Ako nije ucitan
                if (!node.LoadedData)
                {
                    if (node.ParentNode.Children != null && node.ParentNode.Children.Contains(node))
                    {
                        node.ParentNode.Children.Remove(node);
                    }

                    node = doSubNodes((GithubObject)node.SPObject, node.ParentNode, node.RootNode);
                }
            }
            return node;
        }

        private BaseNode doSubNodes(GithubObject githubNode, BaseNode parentNode, BaseNode rootNode)
        {
            try
            {
                var response = client.GetAsync(githubNode.url);
                //if (response.)

                var githubObject = response.Result.Content.ReadAsStringAsync();

                System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                var ghRoot = ser.Deserialize<GithubObject[]>(githubObject.Result);

                GithubObject rootFolder = new GithubObject();
                if (ghRoot != null && ghRoot.Length > 0)
                {
                    var firstItem = ghRoot[0];
                    string tmpRepos = firstItem.url.Replace(reposUrl, "");
                    string[] tmpArr = tmpRepos.Split('/');
                    if (tmpArr != null && tmpArr.Length > 1)
                        rootFolder.name = tmpArr[0] + " - " + tmpArr[1];
                    else
                        rootFolder.name = tmpRepos;
                }

                BaseNode my = new GithubDirectoryNode(rootFolder);
                my.NodeConnector = this;
                my.OMType = ObjectModelType.REMOTE;
                my.RootNode = rootNode;
                my.ParentNode = rootNode;
                my.Title = rootFolder.name;
                my.SPObject = rootFolder;

                ghRoot = ghRoot.OrderBy(m => m.type).ThenBy(m => m.name).ToArray();
                foreach (var node in ghRoot)
                {
                    BaseNode childNode = null;
                    if (node.type == "file")
                    {
                        childNode = new GithubFileNode(node);
                    }
                    else //dir
                    {
                        childNode = new GithubDirectoryNode(node);
                    }

                    try
                    {
                        //
                        if (node.type == "file" && node.name != null && node.name.Contains("."))
                        {
                            var els = node.name.Split('.');
                            string extension = "." + els[els.Length - 1];
                            //Icon icon = Icon.ExtractAssociatedIcon(file.FullName);
                            Icon icon = ShellIcon.GetSmallIconFromExtension(extension);
                            childNode.IconObject = icon.ToBitmap();
                            if (childNode.IconObject.Width != 16)
                            {
                                childNode.IconObject = new Bitmap(childNode.IconObject, 16, 16);
                            }/**/
                        }
                    }
                    catch (Exception exc)
                    {
                        //skip if exception happens here... the default icon will be shown
                    }
                    childNode.NodeConnector = this;
                    childNode.OMType = ObjectModelType.REMOTE;
                    childNode.RootNode = rootNode;
                    childNode.ParentNode = my;
                    childNode.SPObject = node;
                    my.Children.Add(childNode);
                }
                return my;
            }
            catch (Exception exc)
            {
                //SPCoderForm.MainForm.LogException(exc);
                return parentNode;
            }
        }

        public string GetSource(string url)
        {
            try
            {
                var response = client.GetAsync(url);
                
                var githubObject = response.Result.Content.ReadAsStringAsync();
                return githubObject.Result;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public override string ImagesPath
        {
            get
            {
                return null;
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
    }
}
