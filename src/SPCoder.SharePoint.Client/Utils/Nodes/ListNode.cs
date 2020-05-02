using Microsoft.SharePoint.Client;
using SPCoder.Core.Plugins;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SPCoder.Utils.Nodes
{
    /// <summary>
    /// Represents the List node in treeview when connecting through CSOM
    /// </summary>
    public class ListNode : BaseNode, LeafNode
    {
        List<Expression<Func<ListItemCollection, object>>> allIncludes = new List<Expression<Func<ListItemCollection, object>>>();
        public ListNode(List list)
        {
            list.Context.Load(list);
            base.Title = list.Title;
            base.SPObjectType = list.GetType().Name;
            base.Url = list.Title;
            base.IconPath = list.ImageUrl;
        }

        private List realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object objWeb = base.ParentNode.SPObject;
            if (objWeb != null)
            {                
                if (objWeb is Web)
                {
                    List list = ((Web)objWeb).Lists.GetByTitle(this.Title);
                    list.Context.Load(list);
                    //TODO:
                    //check this - in sp2010 DefaultView doesn't exist
                    //use DefaultViewUrl instead in externalopen and copy link
                    list.Context.Load(list.DefaultView);
                    //list.Context.ExecuteQuery();
                    realObject = list;
                    return list;
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
                        Web objWeb = (Web)base.ParentNode.SPObject;

                        string url = objWeb.Url.Replace(objWeb.ServerRelativeUrl, ((List)realObj).DefaultView.ServerRelativeUrl);
                        return url;
                    }
                    else
                        return null;
                case NodeActions.Copy:                    
                    if (realObj != null && actionItem.Name == "Copy link")
                    {
                        Web objWeb = (Web)base.ParentNode.SPObject;

                        string url = objWeb.Url.Replace(objWeb.ServerRelativeUrl, ((List)realObj).DefaultView.ServerRelativeUrl);
                        return url;
                    }
                    else
                        return null;
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
            //Check all plugins
            var baseActions = base.GetNodeActions();
            if (baseActions.Count > 0)
                actions.AddRange(baseActions);
            
            return actions;
        }

        
        public override string LocalImagesSubfolder { get { return "SP"; } }
    }
}
