using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPCoder.Utils.Nodes
{
    public class SPSiteNode : BaseNode
    {
        public SPSiteNode(SPSite site)
        {
            base.Title = site.WebApplication.Name;
            base.SPObjectType = site.GetType().Name;
            base.Url = site.Url;
            base.IconPath = "";
            base.ParentNode = null;
        }
    }
}
