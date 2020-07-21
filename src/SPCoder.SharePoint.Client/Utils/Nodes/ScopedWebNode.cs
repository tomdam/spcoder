using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Utils.Nodes
{
    public class ScopedWebNode : WebNode
    {
        private readonly ClientContext ctx;

        public ScopedWebNode(ClientContext ctx)
        {
            this.ctx = ctx;

            base.SPObjectType = ctx.Web.GetType().Name;
        }

        public override object GetRealSPObject()
        {
            return this.ctx.Web;
        }
    }
}
