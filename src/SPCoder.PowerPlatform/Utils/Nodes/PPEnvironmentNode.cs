
using Newtonsoft.Json;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System.Collections.Generic;

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

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = GetRealSPObject();
            switch (actionItem.Action)
            {
                case NodeActions.Refresh:
                    
                    var rezFlows = ((PPConnector)this.NodeConnector).GetAllFlows();
                    dynamic jsond = JsonConvert.DeserializeObject(rezFlows.Result.ToString());
                    this.SPObject = jsond["value"];

                    return this;
                //for plugins always return the real object
                case NodeActions.Plugin:
                    if (realObj != null)
                    {
                        return realObj;
                    }
                    else
                        return null;
                default:
                    return null;
            }
        }

        public override List<BaseActionItem> GetNodeActions()
        {
            //if (this.Title == "Flows")
            {
                List<BaseActionItem> actions = new List<BaseActionItem>();

                actions.Add(new BaseActionItem { Node = this, Name = "Refresh", Action = Core.Utils.NodeActions.Refresh });

                //Check all plugins
                var baseActions = base.GetNodeActions();
                if (baseActions.Count > 0)
                    actions.AddRange(baseActions);

                return actions;
            }
            return null;
        }
    }
}
