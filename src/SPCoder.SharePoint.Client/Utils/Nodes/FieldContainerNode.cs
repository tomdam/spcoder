using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;

namespace SPCoder.Utils.Nodes
{
    public class FieldContainerNode : BaseNode
    {
        public FieldContainerNode(FieldCollection fieldsCollection)
        {
            base.Title = "Fields";
            base.SPObjectType = fieldsCollection.GetType().Name;
        }

        private List<Field> realObject;
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
                    web.EnsureProperties(w => w.Fields);

                    //FieldCollection fields = web.Fields;
                    var fields = web.Fields.ToList();

                    realObject = fields;
                    return fields;
                }

                if (objWeb is List)
                {
                    List list = objWeb as List;
                    list.EnsureProperties(w => w.Fields);

                    //For lists show only the "useful" fields
                    var fieldsNotFromBaseType = list.Fields.Where(m => m.FromBaseType == false).ToList();
                    
                    realObject = fieldsNotFromBaseType;
                    return fieldsNotFromBaseType;
                }
            }

            return null;
        }
    }
}
