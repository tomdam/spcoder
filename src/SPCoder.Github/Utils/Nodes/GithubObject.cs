using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Github.Utils.Nodes
{
    public class GithubObject
    {
        public string name { get; set; }
        public string path { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string git_url { get; set; }

        public string download_url { get; set; }

        public string type { get; set; }
        public int size { get; set; }

        //public string _links { get; set; }
        
    }
}
