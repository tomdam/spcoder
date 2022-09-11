using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using System.Collections.Generic;
using Newtonsoft.Json;
using SPCoder.Utils.Nodes;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SPCoder.PowerPlatform.Utils.Nodes
{
    public class PPFlowNode : BaseNode, LeafNode
    {
        public PPEnvironmentNode EnvironmentNode { get; set; }
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

        


        private string GetWorkflowId()
        {
            return this.realObject.workflowid;
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
                    if (realObj != null && actionItem.Name == PPConstants.View_Json)
                    {
                        OpenActionResult oar = new OpenActionResult();
                        oar.Source = JsonConvert.SerializeObject(System.Web.Helpers.Json.Decode(realObject.ToString()), Formatting.Indented);
                        oar.Language = "JSON";
                        return oar;
                    }
                    return null;
                case NodeActions.Save:
                    if (actionItem.Name == PPConstants.Update_Cloud_Flow)
                    {
                        if (realObj != null && realObject["category"] != null)
                        {
                            var flowdata = new
                            {
                                clientdata = realObject["clientdata"].ToString()
                            };
                            string dataToUpdate = JsonConvert.SerializeObject(flowdata);
                            UpdateFlow(dataToUpdate);
                        }
                    }
                    else if (actionItem.Name == PPConstants.Turn_On_The_Flow)
                    {
                        TurnTheFlowOnOff(1);
                    }
                    else if (actionItem.Name == PPConstants.Turn_Off_The_Flow)
                    {
                        TurnTheFlowOnOff(0);
                    }
                    return null;
                    break;
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
            //actions.Add(new BaseActionItem { Node = this, Name = "Open in browser", Action = Core.Utils.NodeActions.ExternalOpen });
            //actions.Add(new BaseActionItem { Node = this, Name = "Copy link", Action = Core.Utils.NodeActions.Copy });
            
            actions.Add(new BaseActionItem { Node = this, Name = PPConstants.Turn_On_The_Flow, Action = Core.Utils.NodeActions.Save });
            actions.Add(new BaseActionItem { Node = this, Name = PPConstants.Turn_Off_The_Flow, Action = Core.Utils.NodeActions.Save });
            actions.Add(new BaseActionItem { Node = this, Name = PPConstants.Update_Cloud_Flow, Action = Core.Utils.NodeActions.Save });


            actions.Add(new BaseActionItem { Node = this, Name = PPConstants.View_Json, Action = Core.Utils.NodeActions.Open });

            //this will be done from a plugin
            //actions.Add(new BaseActionItem { Node = this, Name = "View client data", Action = Core.Utils.NodeActions.Open });

            //Check all plugins
            var baseActions = base.GetNodeActions();
            if (baseActions.Count > 0)
                actions.AddRange(baseActions);

            return actions;
        }

        /// <summary>
        /// Just checks if json object is valid.
        /// Used before sending json to the server to prevent obvious errors and not to make unnecessary trafic.
        /// </summary>
        /// <param name="jsonCode"></param>
        /// <returns></returns>
        public static bool CheckIfJsonIsValid(string jsonCode)
        {
            try
            {
                var tmp = JToken.Parse(jsonCode);
                return tmp.Type == JTokenType.Object;
            }
            catch (Exception) 
            {
                return false;
            }
        }

        public async Task<object> GetFlowFromServer(string workflowid)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentNode.EnvironmentUrl + "/api/data/v9.1/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvironmentNode.Token.AccessToken);
                client.Timeout = new TimeSpan(0, 3, 0);
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                try
                {
                    HttpResponseMessage response = client.GetAsync(EnvironmentNode.EnvironmentUrl + "/api/data/v9.1/workflows(" + workflowid + ")").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject(jsonString);
                    }
                    else
                    {
                        SPCoderLogger.Logger.LogError("Not able to get the flow definition from server: " + workflowid + ", " + response.ReasonPhrase);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    SPCoderLogger.Logger.LogError("Error while getting the flow from server: " + workflowid);
                    SPCoderLogger.Logger.LogError(ex);
                    return null;
                }
            }
        }

        public bool UpdateFlow(string dataToUpdate)
        {
            if (!CheckIfJsonIsValid(dataToUpdate))
            {
                SPCoderLogger.Logger.LogError("Json string is not valid. Please check the string before sending it to the server");
                return false;
            }

            SPCoderLogger.Logger.LogInfo("Updating the flow definition on the server.");            
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentNode.EnvironmentUrl + "/api/data/v9.1/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EnvironmentNode.Token.AccessToken);
                client.Timeout = new TimeSpan(0, 3, 0);
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), EnvironmentNode.EnvironmentUrl + "/api/data/v9.1/workflows(" + realObject["workflowid"].ToString() + ")");

                request.Content = new StringContent(dataToUpdate, Encoding.UTF8, "application/json");
                var response = client.SendAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    SPCoderLogger.Logger.LogError(string.Format("Flow update failed: {0}", response.ReasonPhrase));
                    //Console.WriteLine("Flow update failed: {0}", response.ReasonPhrase);
                }
                else
                {
                    SPCoderLogger.Logger.LogInfo("Successfully updated flow definition on the server!");
                    
                    //now get the flow and store the json in the node
                    var rez = GetFlowFromServer(GetWorkflowId());
                    if (rez != null && rez.Result != null)
                    {
                        dynamic jsond = JsonConvert.DeserializeObject(rez.Result.ToString());
                        this.realObject = jsond;
                    }
                }
                
                return response.IsSuccessStatusCode;
            }
        }

        public void TurnTheFlowOnOff(int stateCode) //0 off - 1 onn
        {
            /**/
            if (stateCode == 0)
            {
                SPCoderLogger.Logger.LogInfo("Turning the flow OFF. (On the server)");
            }
            else
            {
                SPCoderLogger.Logger.LogInfo("Turning the flow ON. (On the server)");
            }

            string dataToUpdate = "{\"statecode\":" + stateCode + "}";
            bool responseSuccessStatusCode = UpdateFlow(dataToUpdate);
            if (responseSuccessStatusCode)
            {
                if (stateCode == 0)
                {
                    SPCoderLogger.Logger.LogInfo("Successfully turned the flow OFF on the server");
                }
                else
                {
                    SPCoderLogger.Logger.LogInfo("Successfully turned the flow ON on the server");
                }
            }
        }
    }
}
