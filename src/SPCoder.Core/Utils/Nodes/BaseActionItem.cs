using SPCoder.Core.Plugins;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
