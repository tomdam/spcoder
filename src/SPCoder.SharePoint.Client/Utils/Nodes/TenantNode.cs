using Microsoft.SharePoint.Client.Administration;
using Microsoft.SharePoint.Client;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Online.SharePoint.TenantAdministration;
using SPCoder.SharePoint.Client.Utils;

namespace SPCoder.Utils.Nodes
{
    public class TenantNode : BaseNode
    {
        private Tenant realObject;

        public TenantNode(Tenant tenant)
        {
            base.Title = tenant.RootSiteUrl;

            base.SPObjectType = tenant.GetType().Name;
            base.Url = tenant.RootSiteUrl;


            base.IconPath = "cat.gif";
            this.realObject = tenant;

            base.ParentNode = null;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = this.realObject;

            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:

                    if (realObj != null)
                    {
                        return realObj.Context.Url;
                    }
                    else
                        return null;
                case NodeActions.Copy:
                    if (realObj != null && actionItem.Name == "Copy link")
                    {
                        return realObj.Context.Url;
                    }
                    else
                        return null;
                //for plugins always return the real object
                case NodeActions.Plugin:
                    if (realObj != null)
                    {
                        return realObj;
                    }
                    else
                        return null;
                default:
                    return null;
            }
        }
    }
}
