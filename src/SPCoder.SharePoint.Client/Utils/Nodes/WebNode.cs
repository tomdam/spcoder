using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeOpenXml.Packaging.Ionic.Zip;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.SharePoint.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Utils.Nodes
{
    public class WebNode : BaseNode
    {
        public string AbsoluteUrl { get; set; }

        public WebNode()
        {
            //default icon for web
            base.IconPath = "SharePointFoundation16.png";
            //base.IconPath = null;
        }

        private Web realObject;
        public WebNode(Web web) : this()
        {
            web.EnsureProperties(w => w.Title, w => w.ServerRelativeUrl);

            base.Title = web.Title;
            base.SPObjectType = web.GetType().Name;
            base.Url = web.ServerRelativeUrl;
            base.LoadedData = false;

            this.AbsoluteUrl = WebUtils.MakeAbsoluteUrl(web, base.Url);
            this.realObject = web;
        }

        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            if (RootNode != null && RootNode.SPObject != null)
            {
                if (RootNode.SPObject is Site)
                {
                    Site site = RootNode.SPObject as Site;
                    Web web = site.OpenWeb(base.Url);
                    web.Context.Load(web);
                    web.Context.ExecuteQuery();
                    realObject = web;
                    return web;
                }

                if (RootNode.SPObject is Tenant)
                {
                    Tenant tenant = RootNode.SPObject as Tenant;
                    Site site = tenant.GetSiteByUrl(this.AbsoluteUrl);

                    tenant.Context.Load(site);
                    tenant.Context.Load(site.RootWeb);

                    tenant.Context.ExecuteQueryRetry();

                    realObject = site.RootWeb;
                    return realObject;
                }
            }
            return null;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = GetRealSPObject();
            Web realWeb = realObj as Web;
            realWeb.EnsureProperties(w => w.Url);

            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:

                    if (realObj != null)
                    {
                        return realWeb.Url;
                    }
                    else
                        return null;
                case NodeActions.Copy:
                    if (realObj != null && actionItem.Name == "Copy link")
                    {
                        string url = realWeb.Url;
                        return url;
                    }
                    else
                        return null;
                case NodeActions.Refresh:

                    Site site = RootNode.SPObject as Site;
                    Web web = site.OpenWeb(base.Url);
                    web.Context.Load(web);
                    web.Context.ExecuteQuery();
                    realObject = web;
                    return this;
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

        public override List<BaseActionItem> GetNodeActions()
        {
            List<BaseActionItem> actions = new List<BaseActionItem>();
            actions.Add(new BaseActionItem { Node = this, Name = "Open in browser", Action = Core.Utils.NodeActions.ExternalOpen });
            actions.Add(new BaseActionItem { Node = this, Name = "Copy link", Action = Core.Utils.NodeActions.Copy });
            actions.Add(new BaseActionItem { Node = this, Name = "Refresh", Action = Core.Utils.NodeActions.Refresh });
            var baseActions = base.GetNodeActions();
            if (baseActions.Count > 0)
                actions.AddRange(baseActions);

            return actions;
        }

        public override string LocalImagesSubfolder { get { return "SP"; } }
    }
}