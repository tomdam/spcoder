using Microsoft.SharePoint.Client;
using SPCoder.Core.Utils;

namespace SPCoder.Utils.Nodes
{
    public class FlowContainerNode : BaseNode
    {
        public FlowContainerNode()
        {
            base.Title = "Flows";
            //base.SPObjectType = fieldsCollection.GetType().Name;
        }

        private string realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object objWeb = base.ParentNode.SPObject;
            if (objWeb != null)
            {
                if (objWeb is List)
                {
                    List list = objWeb as List;

                    try
                    {
                        var flows = list.SyncFlowInstances(false);
                        list.Context.Load(flows);
                        list.Context.ExecuteQuery();

                        if (flows != null && flows.SynchronizationData != null)
                        {
                            realObject = flows.SynchronizationData;
                            return realObject;
                        }

                        flows = list.SyncFlowInstances(true);
                        list.Context.Load(flows);
                        list.Context.ExecuteQuery();
                        if (flows != null && flows.SynchronizationData != null)
                        {
                            realObject = flows.SynchronizationData;
                            return realObject;
                        }
                    }
                    catch (System.Exception exc)
                    {
                        SPCoderLogging.Logger.Error("Error while getting the Flows for list.",exc);
                    }
                   
                    return realObject;
                }
            }
            return null;
        }
    }
}
