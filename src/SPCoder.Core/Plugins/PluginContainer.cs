using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Core.Plugins
{
    public class PluginContainer
    {
        public static List<BasePlugin> Plugins = new List<BasePlugin>();

        public static void Register(BasePlugin plugin)
        {
            Plugins.Add(plugin);
        }
    }
}
