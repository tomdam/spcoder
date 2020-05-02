using Microsoft.SharePoint.Client;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Utils.Nodes
{
    public class WebNode : BaseNode
    {
         public WebNode()
        {
            //default icon for web
            base.IconPath = "SharePointFoundation16.png";
            //base.IconPath = null;
        }

        private Web realObject;
         public WebNode(Web web) : this()
        {
            base.Title = web.Title;
            base.SPObjectType = web.GetType().Name;
            base.Url = web.ServerRelativeUrl;
            base.LoadedData = false;
        }

        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            if (RootNode != null && RootNode.SPObject != null)
            {                               
                if (RootNode.SPObject is Site)
                {
                    Site site = RootNode.SPObject as Site;
                    Web web = site.OpenWeb(base.Url);
                    web.Context.Load(web);
                    web.Context.ExecuteQuery();
                    realObject = web;
                    return web;
                }                
            }
            return null;            
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = GetRealSPObject();
            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:
                    
                    if (realObj != null)
                    {
                        return ((Web)realObj).Url;
                    }
                    else
                        return null;
                case NodeActions.Copy:
                    if (realObj != null && actionItem.Name == "Copy link")
                    {
                        string url = ((Web)realObj).Url;
                        return url;
                    }
                    else
                        return null;
                case NodeActions.Refresh:
                    
                    Site site = RootNode.SPObject as Site;
                    Web web = site.OpenWeb(base.Url);
                    web.Context.Load(web);
                    web.Context.ExecuteQuery();
                    realObject = web;
                    return this;
                //for plugins always return the real object
                case NodeActions.Plugin:
                    if (realObj != null)
                    {
                        return realObj;
                    }
                    else
                        return null;
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

        public override string LocalImagesSubfolder { get { return "SP"; } }
    }
}