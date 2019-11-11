using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System.Collections.Generic;
using System.IO;

namespace SPCoder.Github.Utils.Nodes
{

    public class GithubDirectoryNode : BaseNode
    {
        GithubObject _folder = null;
        public GithubDirectoryNode(GithubObject folder)
        {
            base.Title = folder.name;
            base.SPObjectType = folder.GetType().Name;
            base.Url = folder.download_url;
            base.IconPath = "";
            this._folder = folder;
        }

        public override object GetRealSPObject()
        {
            return _folder;
        }

        public override List<BaseActionItem> GetNodeActions()
        {
            List<BaseActionItem> actions = new List<BaseActionItem>();
            actions.Add(new BaseActionItem { Node = this, Name = "Open in browser", Action = Core.Utils.NodeActions.ExternalOpen });
            actions.Add(new BaseActionItem { Node = this, Name = "Copy link", Action = Core.Utils.NodeActions.Copy });
            //actions.Add(new BaseActionItem { Node = this, Name = "Save", Action = Core.Utils.NodeActions.Save });
            return actions;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            
            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:

                    return this._folder.html_url;
                    
                case NodeActions.Copy:
                    if (actionItem.Name == "Copy link")
                    {
                        return this._folder.html_url;
                    }
                    else
                        return null;

                default:
                    return null;

            }

        }
    }
}
