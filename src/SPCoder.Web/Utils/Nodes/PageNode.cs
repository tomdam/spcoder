using HtmlAgilityPack;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System.Collections.Generic;

namespace SPCoder.Web.Utils.Nodes
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
            base.Title = node.Name;
            base.SPObjectType = node.GetType().Name;
            base.Url = node.XPath;
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
                        return node.OuterHtml;
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
            return actions;
        }
    }
}
