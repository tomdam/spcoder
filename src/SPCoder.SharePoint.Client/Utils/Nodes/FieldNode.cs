using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;

namespace SPCoder.Utils.Nodes
{
    public class FieldNode : BaseNode, LeafNode
    {
        public FieldNode(Field field)
        {
            base.Title = field.InternalName;
            base.SPObjectType = field.GetType().Name;
            base.Url = field.InternalName;
            base.IconPath = "DETAIL.GIF";
        }

        private Field realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object parentObj = base.ParentNode.SPObject;
            if (parentObj != null)
            {
                if (parentObj is List<Field>)
                {
                    List<Field> fields = parentObj as List<Field>;

                    if (fields.Any(c => c.InternalName == this.Title))
                    {
                        realObject = fields.FirstOrDefault(c => c.InternalName == this.Title);
                        return realObject;
                    }
                }
            }

            return null;
        }
    }
}
