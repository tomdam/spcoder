
using SPCoder.Core.Utils;
using SPCoder.Utils.Nodes;

namespace SPCoder.PowerPlatform.Utils.Nodes
{
    public class PPEnvironmentNode : BaseNode
    {
        public PPEnvironmentNode()
        {
            base.Title = "Environment";
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
