using Microsoft.SharePoint;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
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

        private SPWeb realObject;
        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = GetRealSPObject();
            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:

                    if (realObj != null)
                    {
                        return ((SPWeb)realObj).Url;
                    }
                    else
                        return null;
                case NodeActions.Copy:
                    if (realObj != null && actionItem.Name == "Copy link")
                    {
                        string url = ((SPWeb)realObj).Url;
                        return url;
                    }
                    else
                        return null;
                case NodeActions.Refresh:
                    //check this
                    SPSite site = RootNode.SPObject as SPSite;
                    SPWeb web = site.OpenWeb(base.Url);
                    
                    realObject = web;
                    return this;

                default:
                    return null;
            }
        }

        public override List<BaseActionItem> GetNodeActions()
        {
            List<BaseActionItem> actions = new List<BaseActionItem>();
            actions.Add(new BaseActionItem { Node = this, Name = "Open in browser", Action = Core.Utils.NodeActions.ExternalOpen });
            actions.Add(new BaseActionItem { Node = this, Name = "Copy link", Action = Core.Utils.NodeActions.Copy });
            actions.Add(new BaseActionItem { Node = this, Name = "Refresh", Action = Core.Utils.NodeActions.Refresh });
            var baseActions = base.GetNodeActions();
            if (baseActions.Count > 0)
                actions.AddRange(baseActions);

            return actions;
        }
    }
}