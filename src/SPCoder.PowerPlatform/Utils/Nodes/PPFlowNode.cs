using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using System.Collections.Generic;
using Newtonsoft.Json;
using SPCoder.Utils.Nodes;


namespace SPCoder.PowerPlatform.Utils.Nodes
{
    public class PPFlowNode : BaseNode, LeafNode
    {
        public PPFlowNode(dynamic dynamicDataFromJson)
        {
            base.Title = dynamicDataFromJson.name;
            base.SPObjectType = dynamicDataFromJson.GetType().Name;
            //base.Url = field.InternalName;
            base.IconPath = "DETAIL.GIF";
            this.realObject = dynamicDataFromJson;
        }

        private dynamic realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;

            object parentObj = base.ParentNode.SPObject;
            if (parentObj != null)
            {
                return realObject;
            }

            return null;
        }

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = this.SPObject;
            
            switch (actionItem.Action)
            {
                case NodeActions.ExternalOpen:

                    if (realObj != null)
                    {
                        //((PPConnector)this.NodeConnector).EnvironmentUrl
                        //example realObject.id
                        ///providers/Microsoft.ProcessSimple/environments/Default-GUID/flows/GUID
                        //TODO: put this url to some configuration
                        string url = "https://emea.flow.microsoft.com/manage/" + realObject.id.Replace(@"/providers/Microsoft.ProcessSimple/", "");
                        return url;
                    }
                    else
                        return null;
                case NodeActions.Open:
                    if (realObj != null && actionItem.Name == "View json")
                    {
                        OpenActionResult oar = new OpenActionResult();
                        oar.Source = JsonConvert.SerializeObject(System.Web.Helpers.Json.Decode(realObject.ToString()), Formatting.Indented);
                        oar.Language = "JSON";
                        return oar;
                    }
                    else
                        if (realObj != null && actionItem.Name == "View client data" && realObject["clientdata"] != null)
                    {
                        OpenActionResult oar = new OpenActionResult();
                        oar.Source = JsonConvert.SerializeObject(System.Web.Helpers.Json.Decode(realObject["clientdata"].ToString()), Formatting.Indented);                        
                        oar.Language = "JSON";
                        return oar;
                    }
                    //
                    return null;
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
            List<BaseActionItem> actions = new List<BaseActionItem>();
            actions.Add(new BaseActionItem { Node = this, Name = "Open in browser", Action = Core.Utils.NodeActions.ExternalOpen });
            //actions.Add(new BaseActionItem { Node = this, Name = "Copy link", Action = Core.Utils.NodeActions.Copy });
            actions.Add(new BaseActionItem { Node = this, Name = "View json", Action = Core.Utils.NodeActions.Open });
            actions.Add(new BaseActionItem { Node = this, Name = "View client data", Action = Core.Utils.NodeActions.Open });
            //Check all plugins
            var baseActions = base.GetNodeActions();
            if (baseActions.Count > 0)
                actions.AddRange(baseActions);

            return actions;
        }
    }
}
