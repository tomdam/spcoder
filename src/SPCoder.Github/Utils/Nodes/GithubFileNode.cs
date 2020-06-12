using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System.Collections.Generic;
using System.IO;

namespace SPCoder.Github.Utils.Nodes
{
    public class GithubFileNode : BaseNode, LeafNode
    {
        GithubObject _file = null;
        public GithubFileNode(GithubObject file)
        {
            base.Title = file.name;
            base.SPObjectType = file.GetType().Name;
            base.Url = file.download_url;
            //base.IconPath = "";
            this._file = file;
        }

        public override object GetRealSPObject()
        {
            return _file;
        }

        public override List<BaseActionItem> GetNodeActions()
        {
            List<BaseActionItem> actions = new List<BaseActionItem>();
            actions.Add(new BaseActionItem { Node = this, Name = "Open", Action = Core.Utils.NodeActions.Open });
            actions.Add(new BaseActionItem { Node = this, Name = "Copy link", Action = Core.Utils.NodeActions.Copy });
            //actions.Add(new BaseActionItem { Node = this, Name = "Save", Action = Core.Utils.NodeActions.Save });
            return actions;
        }
        public override string GetDefaultSource()
        {            
            GithubConnector connector = (GithubConnector)this.RootNode.NodeConnector;
            return connector.GetSource(this.Url);
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            GithubConnector connector = (GithubConnector)this.RootNode.NodeConnector;
            switch (actionItem.Action)
            {
                case NodeActions.Open:
                    OpenActionResult oar = new OpenActionResult();
                    oar.Source = connector.GetSource(this.Url);
                    var els = this.Url.Split('.');                    
                    oar.Language = els[els.Length - 1];
                    return oar;
                    
                case NodeActions.Save:
                    
                    return connector.GetSource(this.Url);
                    
                case NodeActions.Copy:
                    if (actionItem.Name == "Copy link")
                    {
                        return this._file.html_url;
                    }
                    else
                        return null;
                    

                default:
                    return null;
                    
            }
            
        }
    }
}
