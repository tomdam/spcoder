using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPCoder.Utils.Nodes
{
    public class SPListNode : BaseNode, LeafNode
    {
        public SPListNode(SPList list)
        {
            base.Title = list.Title;
            base.SPObjectType = list.GetType().Name;
            base.Url = list.Title;
            base.IconPath = list.ImageUrl;
        }

        public override object GetRealSPObject()
        {
            object objWeb = base.ParentNode.SPObject;
            if (objWeb != null)
            {
                if (objWeb is SPWeb)
                {
                    SPList list = ((SPWeb)objWeb).Lists[base.Url];                    
                    return list;
                }                
            }
            return null;
        }
    }
}
