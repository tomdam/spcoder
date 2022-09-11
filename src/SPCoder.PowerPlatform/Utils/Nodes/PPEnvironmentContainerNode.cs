
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System.Collections.Generic;

namespace SPCoder.PowerPlatform.Utils.Nodes
{
    public class PPEnvironmentContainerNode : BaseNode
    {
        public PPEnvironmentContainerNode()
        {
            base.Title = "Flows";
            //base.SPObjectType = fieldsCollection.GetType().Name;
        }

        private string realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;
            
            return null;
        }

        
    }
}
