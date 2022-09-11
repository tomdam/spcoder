
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SPCoder.Core.Utils;
using SPCoder.Core.Utils.Nodes;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SPCoder.PowerPlatform.Utils.Nodes
{
    public class PPEnvironmentNode : BaseNode
    {
        public string EnvironmentUrl { get; set; }
        public AuthenticationResult Token { get; set; }
        public PPEnvironmentNode(dynamic dynamicDataFromJson)
        {
            
            this.realObject = dynamicDataFromJson;
            base.Title = dynamicDataFromJson["properties"]["displayName"].ToString();
            this.EnvironmentUrl = dynamicDataFromJson["properties"]["linkedEnvironmentMetadata"]["instanceUrl"].ToString();
        }

        private dynamic realObject;
        public override object GetRealSPObject()
        {
            if (realObject != null)
                return realObject;
            return null;
        }

        public async Task<object> GetAllFlows()
        {
            if (Token == null)
            { 
                Token = ((PPConnector)NodeConnector).SilentlyGetTokenForUrl(this.EnvironmentUrl);
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
                client.Timeout = new TimeSpan(0, 3, 0);
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    HttpResponseMessage response = client.GetAsync(this.EnvironmentUrl + "/api/data/v9.1/workflows").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        return jsonString;
                    }
                    else
                    {
                        SPCoderLogger.Logger.LogError(string.Format("Getting flows failed: {0}", response.ReasonPhrase));
                        Console.WriteLine("Getting flows failed: {0}", response.ReasonPhrase);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    SPCoderLogger.Logger.LogError(ex);
                    Console.WriteLine("Error: {0}", ex.Message);
                    return null;
                }
            }
        }
        

        public override object ExecuteAction(BaseActionItem actionItem)
        {
            var realObj = GetRealSPObject();
            switch (actionItem.Action)
            {
                case NodeActions.Refresh:
                    
                    var rezFlows = GetAllFlows();
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
