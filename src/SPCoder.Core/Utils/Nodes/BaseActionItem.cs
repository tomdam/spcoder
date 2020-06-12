using SPCoder.Core.Plugins;
using SPCoder.Utils.Nodes;

namespace SPCoder.Core.Utils.Nodes
{
    public class BaseActionItem
    {
        public string Name { get; set; }
        public BaseNode Node { get; set; }
        public NodeActions Action { get; set; }

        public BasePlugin Plugin { get; set; }

        public object Tag { get; set; }
    }
}
