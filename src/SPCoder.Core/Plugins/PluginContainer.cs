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
            if (Plugins.Any(p => p.Name == plugin.Name))
            {
                // remove & re-register
                var p = Plugins.First(p1 => p1.Name == plugin.Name);
                Plugins.Remove(p);
            }

            Plugins.Add(plugin);
        }
    }
}
