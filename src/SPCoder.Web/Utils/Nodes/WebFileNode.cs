using HtmlAgilityPack;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System.Collections.Generic;

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
            if (!string.IsNullOrEmpty(node.Id))
            {
                base.Title = node.Name + " (id=" + node.Id + ")";
            }
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
                        OpenActionResult oar = new OpenActionResult();
                        oar.Source = node.OuterHtml;
                        oar.Language = "html";
                        return oar;
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

        public override string LocalImagesSubfolder { get { return "Web"; } }
    }
}
