using Microsoft.SharePoint.Client;
using SPCoder.Core.Plugins;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.SharePoint.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SPCoder.Utils.Nodes
{
    public class ContentTypeNode : BaseNode, LeafNode
    {
        public ContentTypeNode(ContentType contentType)
        {
            base.Title = contentType.Name;
            base.SPObjectType = contentType.GetType().Name;
            base.Url = contentType.Name;
            base.IconPath = "DETAIL.GIF";
        }

        private ContentType realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object parentObj = base.ParentNode.SPObject;
            if (parentObj != null)
            {
                if (parentObj is ContentTypeCollection)
                {
                    ContentTypeCollection contentTypes = parentObj as ContentTypeCollection;

                    if (contentTypes.Any(c => c.Name == this.Title))
                    {
                        realObject = contentTypes.FirstOrDefault(c => c.Name == this.Title);
                        return realObject;
                    }
                }
            }

            return null;
        }
    }
}
