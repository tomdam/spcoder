using Microsoft.SharePoint;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPCoder.Utils.Nodes
{
    public class SPListNode : BaseNode, LeafNode
    {
        public SPListNode(SPList list)
        {
            base.Title = list.Title;
            base.SPObjectType = list.GetType().Name;
            base.Url = list.Title;
            base.IconPath = list.ImageUrl;
        }

        public override object GetRealSPObject()
        {
            object objWeb = base.ParentNode.SPObject;
            if (objWeb != null)
            {
                if (objWeb is SPWeb)
                {
                    SPList list = ((SPWeb)objWeb).Lists[base.Url];                    
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
                        SPWeb objWeb = (SPWeb)base.ParentNode.SPObject;

                        string url = objWeb.Url.Replace(objWeb.ServerRelativeUrl, ((SPList)realObj).DefaultView.ServerRelativeUrl);
                        return url;
                    }
                    else
                        return null;
                case NodeActions.Copy:
                    if (realObj != null && actionItem.Name == "Copy link")
                    {
                        SPWeb objWeb = (SPWeb)base.ParentNode.SPObject;

                        string url = objWeb.Url.Replace(objWeb.ServerRelativeUrl, ((SPList)realObj).DefaultView.ServerRelativeUrl);
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

    }
}
