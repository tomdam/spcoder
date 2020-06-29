using HtmlAgilityPack;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.DotNetPerls.Utils.Nodes
{
    public class PageNode : BaseNode
    {
        public PageNode()
        {
            //default icon for web
            base.IconPath = null;
        }

        public PageNode(HtmlNode node) : this()
        {
            //base.Title = node.Name;
            base.SPObjectType = node.GetType().Name;
            base.SPObject = node;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            switch (actionItem.Action)
            {
                case NodeActions.Open:
                    if (base.SPObject != null)
                    {
                        HtmlNode node = (HtmlNode)base.SPObject;

                        OpenActionResult oar = new OpenActionResult();
                        oar.Source = node.OuterHtml;
                        oar.Language = "html";
                        return oar;
                    }
                    else
                        return null;

                case NodeActions.ExternalOpen:
                    if (!string.IsNullOrEmpty(base.Url))
                    {
                        return base.RootNode.NodeConnector.Endpoint + base.Url;
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
            actions.Add(new BaseActionItem { Node = this, Name = "View source", Action = Core.Utils.NodeActions.Open });
            actions.Add(new BaseActionItem { Node = this, Name = "Visit page", Action = Core.Utils.NodeActions.ExternalOpen });
            return actions;
        }

        public override string LocalImagesSubfolder { get { return "Web"; } }
    }
}
