using Microsoft.SharePoint.Client;
using SPCoder.Core.Plugins;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace SPCoder.Utils.Nodes
{
    public class FolderNode : BaseNode
    {
        public FolderNode(Folder folder)
        {
            folder.Context.Load(folder);
            base.Title = folder.Name;
            base.SPObjectType = folder.GetType().Name;
            base.Url = folder.ServerRelativeUrl;
        }

        private Folder realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object objParent = base.ParentNode.SPObject;
            if (objParent != null)
            {
                if (objParent is List)
                {
                    Folder folder = ((List)objParent).RootFolder.ResolveSubFolder(this.Title);

                    realObject = folder;
                    return folder;
                }

                if (objParent is Web)
                {
                    Folder folder = ((Web)objParent).GetFolderByServerRelativeUrl(this.Url);
                    realObject = folder;

                    return folder;
                }

                if (objParent is Folder)
                {
                    Folder folder = ((Folder)objParent).ResolveSubFolder(this.Title);

                    realObject = folder;
                    return folder;
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
    }
}
