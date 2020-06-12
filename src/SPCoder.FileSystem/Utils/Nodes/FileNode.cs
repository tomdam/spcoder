using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System.Collections.Generic;
using System.IO;

namespace SPCoder.FileSystem.Utils.Nodes
{
    public class FileNode : BaseNode, LeafNode
    {
        FileInfo _file = null;
        public FileNode(FileInfo file)
        {
            base.Title = file.Name;
            base.SPObjectType = file.GetType().Name;
            base.Url = file.FullName;
            base.IconPath = "";
            //this._file = file;
        }

        public override object GetRealSPObject()
        {
            _file = new FileInfo(Url);
            return _file;
        }

        public override string GetDefaultSource()
        {
            return File.ReadAllText(this.Url);
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            switch (actionItem.Action)
            {
                case NodeActions.Open:
                    OpenActionResult oar = new OpenActionResult();
                    oar.Source = File.ReadAllText(this.Url);
                    var els = this.Url.Split('.');
                    oar.Language = els[els.Length - 1];
                    return oar;
                    
                case NodeActions.ExternalOpen:
                    //for this we only need the path
                    return this.Url;
                //for plugins always return the real object
                case NodeActions.Plugin:
                    if (_file != null)
                    {
                        return _file;
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
            actions.Add(new BaseActionItem { Node = this, Name = "Open", Action = Core.Utils.NodeActions.Open });
            actions.Add(new BaseActionItem { Node = this, Name = "Open in application", Action = Core.Utils.NodeActions.ExternalOpen });

            var baseActions = base.GetNodeActions();
            if (baseActions.Count > 0)
                actions.AddRange(baseActions);

            return actions;
        }
    }
}