using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.FileSystem.Utils.Nodes
{
    
   public class FolderNode : BaseNode
    {
        DirectoryInfo _file = null;
        public FolderNode(DirectoryInfo folder)
        {
            base.Title = folder.Name;
            base.SPObjectType = folder.GetType().Name;
            base.Url = folder.FullName;
            //base.IconPath = null;
            //this._file = folder;
        }

        public override object GetRealSPObject()
        {
            _file = new DirectoryInfo(Url);
            return _file;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:
                    return this.Url;
                    
                default:
                    return null;
            }
        }

        public override List<BaseActionItem> GetNodeActions()
        {
            List<BaseActionItem> actions = new List<BaseActionItem>();
            actions.Add(new BaseActionItem { Node = this, Name = "Open in explorer", Action = Core.Utils.NodeActions.ExternalOpen });
            return actions;
        }
    }
}
