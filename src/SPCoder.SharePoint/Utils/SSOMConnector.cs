using Microsoft.SharePoint;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SPCoder.Utils
{
    public class SSOMConnector : BaseConnector
    {

        public SSOMConnector()
        { }

        public SSOMConnector(string connectorType)
        { }

        public override Nodes.BaseNode GetSPStructure(string siteUrl)
        {            
            SPSite site = new SPSite(siteUrl);
            BaseNode rootNode = new SPSiteNode(site);
            rootNode.OMType = ObjectModelType.LOCAL;
            rootNode.SPObject = site;
            rootNode.NodeConnector = this;
            doSPWeb(site.RootWeb, rootNode, rootNode);
            return rootNode;
        }

        private void doSPWeb(SPWeb web, BaseNode parentNode, BaseNode rootNode)
        {
            BaseNode myNode = new SPWebNode(web);
            parentNode.Children.Add(myNode);
            myNode.ParentNode = parentNode;
            myNode.RootNode = rootNode;
            myNode.NodeConnector = this;

            foreach (SPWeb childWeb in web.Webs)
            {
                doSPWeb(childWeb, myNode, rootNode);                
            }

            foreach (SPList list in web.Lists)
            {
                BaseNode myListNode = new SPListNode(list);
                myNode.Children.Add(myListNode);
                myListNode.ParentNode = myNode;
                myListNode.RootNode = rootNode;
                myListNode.NodeConnector = this;
            }
            web.Dispose();
        }

        public override string ImagesPath
        {
            get
            {
                return @"C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\15\TEMPLATE\IMAGES";
            }
            set
            {}
        }

        public override bool IsImagesPathLocal
        {
            get
            {
                return true;
            }
            set
            {}
        }
    }
}
