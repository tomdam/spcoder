using HtmlAgilityPack;
using SPCoder.Utils;
using SPCoder.Utils.Nodes;
using SPCoder.Web.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;


namespace SPCoder.Web.Utils
{
    public class WebConnector : BaseConnector 
    {

        public HtmlDocument Document { get; set; }
        public WebConnector(string connectorType)
        { }

        public WebConnector()
        { }

        string RootNodeTitle = "Web page: ";
        public override SPCoder.Utils.Nodes.BaseNode GetSPStructure(string siteUrl)
        {
            BaseNode rootNode = new PageNode();
            
            HtmlWeb page = new HtmlWeb();
            HtmlAgilityPack.HtmlWeb.PreRequestHandler handler = delegate (HttpWebRequest request)
            {
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.CookieContainer = new System.Net.CookieContainer();
                return true;
            };
            page.PreRequest += handler;

            Document = page.Load(siteUrl);
            page.Get(siteUrl, "/");

            rootNode.Title = RootNodeTitle + page.ResponseUri.Host.ToString();
            rootNode.IconPath = "html.png";

            BaseNode my = new PageNode(Document.DocumentNode);
            my.RootNode = rootNode;
            my.ParentNode = rootNode;
            my.Title = Document.DocumentNode.Name;
            my.SPObject = Document;
            
            //return rootNode;
            //rootNode.SPObject = site;
            doPageNodes(Document.DocumentNode, rootNode, rootNode);
            return rootNode;
        }

        private void doPageNodes(HtmlNode web, BaseNode parentNode, BaseNode rootNode)
        {
            try
            {
                BaseNode myNode = new PageNode(web);
                parentNode.Children.Add(myNode);
                myNode.ParentNode = parentNode;
                myNode.RootNode = rootNode;
                
                try
                {
                    foreach (HtmlNode childWeb in web.ChildNodes)
                    {
                        var hasNonTextChildren = childWeb.ChildNodes.Any(m => !(m is HtmlTextNode));
                        if (hasNonTextChildren) //childWeb.HasChildNodes)
                        {
                            doPageNodes(childWeb, myNode, rootNode);
                        }
                        else
                        {
                            //Ako je #text preskoci ga
                            if (childWeb is HtmlTextNode)
                                continue;
                            BaseNode myListNode = new PageLeafNode(childWeb);
                            myNode.Children.Add(myListNode);
                            myListNode.ParentNode = myNode;
                            myListNode.RootNode = rootNode;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
                //SPCoderForm.MainForm.LogException(exc);
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

        public override List<object> AutoAddToContext()
        {
            List<object> objects = new List<object>();
            objects.Add(this.Document);
            
            return objects;
        }
    }
}
