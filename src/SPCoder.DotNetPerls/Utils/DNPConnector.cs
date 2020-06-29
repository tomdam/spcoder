using HtmlAgilityPack;
using SPCoder.DotNetPerls.Utils.Nodes;
using SPCoder.Utils;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SPCoder.DotNetPerls.Utils
{
    public class DNPConnector : BaseConnector
    {

        public HtmlDocument Document { get; set; }
        public DNPConnector(string connectorType)
        { }

        public DNPConnector()
        { }


        List<string> notcsharp = new List<string>(new string[] { "-go", "-java", "-vbnet", "-js", "-python", "-ruby", "-scala", "-swift", "-wpf", "-fs", "." });
        HtmlWeb page = new HtmlWeb();
        List<DnpPage> links = new List<DnpPage>();

        string RootNodeTitle = "Dotnetperls C# examples";
        
        public override SPCoder.Utils.Nodes.BaseNode GetSPStructure(string siteUrl)
        {
            if (string.IsNullOrEmpty(siteUrl))
            {
                siteUrl = "https://www.dotnetperls.com/";
            }
            if (!siteUrl.EndsWith("/")) siteUrl += "/";
            base.Endpoint = siteUrl;    
            HtmlAgilityPack.HtmlWeb.PreRequestHandler handler = delegate (HttpWebRequest request)
            {
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.CookieContainer = new System.Net.CookieContainer();
                return true;
            };
            page.PreRequest += handler;
            
            BaseNode rootNode = new PageNode();
            rootNode.NodeConnector = this;

            rootNode.Title = RootNodeTitle;
            rootNode.LoadedData = true;
            
            Visit(page, links, siteUrl, "", rootNode, rootNode);
            return rootNode;
        }
        

        private BaseNode Visit(HtmlWeb page, List<DnpPage> links, string root, string link, BaseNode parentNode, BaseNode rootNode)
        {
            HtmlNode htmlnode = null;
            var dnp1 = links.Where(m => m.Link == link).FirstOrDefault();
            if (dnp1 == null)
            {
                var document = page.Load(root + link);
                htmlnode = document.DocumentNode;

                dnp1 = new DnpPage();
                dnp1.Link = link;
                dnp1.Html = htmlnode.OuterHtml;
                dnp1.PLink = link;
                links.Add(dnp1);

                BaseNode myNode = new PageNode(htmlnode);
                myNode.LoadedData = true;
                myNode.Url = link;
                if (!string.IsNullOrEmpty(link))
                {
                    myNode.Title = link;                    
                }
                else
                {
                    myNode.Title = "Please choose the example:";
                }
                parentNode.Children.Add(myNode);
                myNode.ParentNode = parentNode;
                myNode.RootNode = rootNode;

                dnp1.Node = myNode;

                GenerateSourceCodeNodes(htmlnode, myNode, rootNode);
            }
            else
            {
                if (string.IsNullOrEmpty(dnp1.Html))
                {
                    var document = page.Load(root + link);
                    htmlnode = document.DocumentNode;
                    dnp1.Html = htmlnode.OuterHtml;
                    dnp1.Node.SPObject = htmlnode;
                }
                else
                {
                    var document = new HtmlDocument();
                    document.LoadHtml(dnp1.Html);
                    htmlnode = document.DocumentNode;
                    dnp1.Node.SPObject = htmlnode;
                }
            }

            var htmla = htmlnode.SelectNodes("//a");

            foreach (var a in htmla)
            {
                if (a.Attributes.Contains("href"))
                {
                    var href = a.Attributes["href"].Value;
                    bool exists = false;
                    foreach (string s in notcsharp)
                    {
                        if (href.EndsWith(s))
                        {
                            {
                                exists = true;
                                break;
                            }
                        }
                    }
                    //skip external links, skip "s" link
                    if (href.Contains("https://") || href.Contains("http://") || href == "s")
                    {
                        exists = true;
                        //break;
                    }

                    if (!exists)
                    {
                        DnpPage dnp = null;
                        if (links.Where(m => m.Link == href).Count() == 0)
                        {
                            dnp = new DnpPage();
                            dnp.Link = href;
                            dnp.PLink = link;
                            links.Add(dnp);
                        }
                        else
                        {
                            dnp = links.Where(m => m.Link == href).First();
                        }

                        //BaseNode myNode = new PageNode(htmlnode);
                        if (dnp.Node == null)
                        {
                            BaseNode myNode = new PageNode();
                            dnp.Node = myNode;
                            myNode.Title = href;
                            myNode.Url = href;
                            dnp1.Node.Children.Add(myNode);
                            myNode.ParentNode = dnp1.Node;
                            myNode.RootNode = rootNode;
                        }
                    }
                }
            }
            return dnp1.Node;
        }


        private void GenerateSourceCodeNodes(HtmlNode htmlnode, BaseNode pageNode, BaseNode rootNode)
        {
            var htmla = htmlnode.SelectNodes("//div[@class='e']");

            if (htmla == null) return;

            int cnt = htmla.Count;
            for (var i = 0; i < cnt; i++)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    var prog = htmla[i];
                    string title = "";
                    for (int j = 0; j < prog.ChildNodes.Count; j++)
                    {
                        var div = prog.ChildNodes[j];
                        if (j == 0 && div.OuterHtml.Contains("<b>C# program"))
                        {
                            sb.Append("//" + div.InnerText);
                            title = div.InnerText;
                            continue;
                        }

                        if (div.OuterHtml.Contains("<b>Output</b>"))
                        {
                            break;
                        }

                        sb.Append(System.Web.HttpUtility.HtmlDecode(div.InnerText));
                    }

                    if (string.IsNullOrEmpty(title))
                    {
                        //skip if there is no title
                        continue;
                    }

                    sb.Replace("static void Main()", "public static void Main()");
                    sb.AppendLine("Program.Main();");

                    BaseNode mySourceCodeNode = new PageLeafNode(sb.ToString());
                    mySourceCodeNode.Url = pageNode.Url;
                    mySourceCodeNode.Title = title;
                    
                    pageNode.Children.Add(mySourceCodeNode);
                    mySourceCodeNode.ParentNode = pageNode;
                    mySourceCodeNode.RootNode = rootNode;
                    mySourceCodeNode.NodeConnector = this;
                }
                catch (Exception)
                {
                    
                }
            }
        }

        public override BaseNode ExpandNode(BaseNode node, bool doIfLoaded = false)
        {
            //Ako je web node
            if (node is PageNode)
            {
                //Ako nije ucitan
                if (!node.LoadedData)
                {
                    //if (node.ParentNode.Children != null && node.ParentNode.Children.Contains(node))
                    //{
                    //    node.ParentNode.Children.Remove(node);
                    //}

                    node = Visit(page, links, base.Endpoint, node.Url, node, node.RootNode);
                    GenerateSourceCodeNodes((HtmlNode)node.SPObject, node, node.RootNode);
                }
            }
            return node;
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

        /*public override List<object> AutoAddToContext()
        {
            List<object> objects = new List<object>();
            objects.Add(this.Document);

            return objects;
        }*/
    }
}
