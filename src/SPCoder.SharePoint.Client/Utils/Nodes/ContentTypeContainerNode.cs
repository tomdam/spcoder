using CamlexNET;
using Microsoft.SharePoint.Client;
using SPCoder.Core.Plugins;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.SharePoint.Client.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SPCoder.Utils.Nodes
{
    public class ContentTypeContainerNode :BaseNode
    {
        public ContentTypeContainerNode(ContentTypeCollection contentTypeCollection)
        {
            base.Title = "Content Types";
            base.SPObjectType = contentTypeCollection.GetType().Name;
        }

        private ContentTypeCollection realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object objWeb = base.ParentNode.SPObject;
            if (objWeb != null)
            {
                if (objWeb is Web)
                {
                    Web web = objWeb as Web;
                    web.EnsureProperties(w => w.ContentTypes);

                    ContentTypeCollection contentTypes = web.ContentTypes;

                    realObject = contentTypes;
                    return contentTypes;
                }

                if (objWeb is List)
                {
                    List list = objWeb as List;
                    list.EnsureProperties(w => w.ContentTypes);

                    ContentTypeCollection contentTypes = list.ContentTypes;

                    realObject = contentTypes;
                    return contentTypes;
                }
            }

            return null;
        }
    }
}
