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
    /// <summary>
    /// Represents a code example
    /// </summary>
    public class PageLeafNode : BaseNode, LeafNode
    {        
        public PageLeafNode()
        {
            //default icon for web
            base.IconPath = "csharp.png";
        }

        public PageLeafNode(String sourceCode) : this()
        {
            this.SPObjectType = sourceCode.GetType().Name;
            this.SPObject = sourceCode;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            switch (actionItem.Action)
            {
                case NodeActions.Open:
                    if (base.SPObject != null)
                    {
                        OpenActionResult oar = new OpenActionResult();
                        oar.Source = this.SPObject.ToString();
                        oar.Language = "csharp";
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
            actions.Add(new BaseActionItem { Node = this, Name = "View code", Action = Core.Utils.NodeActions.Open });
            actions.Add(new BaseActionItem { Node = this, Name = "Visit page", Action = Core.Utils.NodeActions.ExternalOpen });
            return actions;
        }

        public override string LocalImagesSubfolder { get { return "DNP"; } }
    }
}
