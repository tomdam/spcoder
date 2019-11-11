using HtmlAgilityPack;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Web.Utils.Nodes
{
    public class PageLeafNode : BaseNode, LeafNode
    {
        public PageLeafNode()
        {
            //default icon for web
            base.IconPath = null;
        }

        public PageLeafNode(HtmlNode node) : this()
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
