using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.DotNetPerls.Utils
{
    public class DnpPage
    {
        public string Link { get; set; }
        public string Html { get; set; }
        //parent
        public string PLink { get; set; }
        public BaseNode Node { get; set; }
    }
}
