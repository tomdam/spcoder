using SPCoder.Core.Plugins;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using System.Collections.Generic;
using System.Drawing;

namespace SPCoder.Utils.Nodes
{
    public class BaseNode
    {
        public BaseNode()
        {
            Children = new List<BaseNode>();
        }
        public string Title { get; set; }

        public virtual string IconPath { get; set; }

        public string Url { get; set; }

        public Bitmap IconObject { get; set; }

        protected object _spObject;

        public object SPObject
        {
            get
            {
                if (_spObject == null)
                {
                    _spObject = GetRealSPObject();
                }
                return _spObject;
            }
            set
            {
                _spObject = value;
            }
        }

        public string SPObjectType { get; set; }

        public BaseNode ParentNode { get; set; }
        public IList<BaseNode> Children { get; set; }

        public BaseNode RootNode { get; set; }

        public ObjectModelType OMType { get; set; }

        public virtual object GetRealSPObject()
        {
            return null;
        }

        public bool LoadedData { get; set; }

        public BaseConnector NodeConnector { get; set; }

        public virtual object GetDefaultSource()
        {
            return null;
        }

        public virtual List<BaseActionItem> GetNodeActions()
        {
            List<BaseActionItem> actions = new List<BaseActionItem>();
            foreach (var plugin in PluginContainer.Plugins)
            {
                if (plugin.IsActive(this.SPObject))
                {
                    actions.Add(new BaseActionItem { Plugin = plugin, Node = this, Name = plugin.Name, Action = Core.Utils.NodeActions.Plugin });
                }
            }
            return actions;
        }

        public virtual object ExecuteAction(BaseActionItem actionItem)
        {
            return null;
        }

        public virtual string LocalImagesSubfolder { get { return null; } }

        public virtual void Clean()
        { }
    }
}
