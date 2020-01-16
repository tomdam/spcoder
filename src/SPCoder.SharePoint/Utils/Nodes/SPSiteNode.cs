using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
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
            //base.IconPath = "";
            base.IconPath = "cat.gif";
            this.realObject = site;

            base.ParentNode = null;
        }

        private SPSite realObject;
        public override object ExecuteAction(BaseActionItem actionItem)
        {

            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:

                    if (realObject != null)
                    {
                        return ((SPSite)realObject).Url;
                    }
                    else
                        return null;
                case NodeActions.Copy:
                    if (realObject != null && actionItem.Name == "Copy link")
                    {
                        string url = ((SPSite)realObject).Url;
                        return url;
                    }
                    else
                        return null;
                case NodeActions.Refresh:

                    //Site site = realObject;
                    

                    return this;
                case NodeActions.Close:
                    //clean 
                    //this.NodeConnector.
                    Clean();
                    return this;

                default:
                    return null;
            }
        }

        public override void Clean()
        {
            this.Children = null;
            realObject = null;
        }

        public override List<BaseActionItem> GetNodeActions()
        {
            List<BaseActionItem> actions = new List<BaseActionItem>();
            actions.Add(new BaseActionItem { Node = this, Name = "Open in browser", Action = Core.Utils.NodeActions.ExternalOpen });
            actions.Add(new BaseActionItem { Node = this, Name = "Copy link", Action = Core.Utils.NodeActions.Copy });
            //actions.Add(new BaseActionItem { Node = this, Name = "Refresh", Action = Core.Utils.NodeActions.Refresh });
            actions.Add(new BaseActionItem { Node = this, Name = "Close", Action = Core.Utils.NodeActions.Close });
            var baseActions = base.GetNodeActions();
            if (baseActions.Count > 0)
                actions.AddRange(baseActions);

            return actions;
        }

    }
}
