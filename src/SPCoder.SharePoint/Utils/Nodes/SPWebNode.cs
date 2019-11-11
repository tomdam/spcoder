using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPCoder.Utils.Nodes
{
    public class SPWebNode : BaseNode
    {
        public SPWebNode()
        {
            //default icon for spweb
            base.IconPath = "siteIcon.png";            
        }

        public SPWebNode(SPWeb web) : this()
        {
            base.Title = web.Title;
            base.SPObjectType = web.GetType().Name;
            base.Url = web.ServerRelativeUrl;
            if (web.SiteLogoUrl != null)
            {
                base.IconPath = web.SiteLogoUrl;
            }
        }

        public override object GetRealSPObject()
        {
            if (RootNode != null && RootNode.SPObject != null)
            {
                if (RootNode.SPObject is SPSite)
                {
                    SPSite site = RootNode.SPObject as SPSite;
                    SPWeb web = site.OpenWeb(base.Url);
                    return web;
                }
            }
            return null;            
        }
    }
}