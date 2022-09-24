using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SPCoder.Core.Utils;
using SPCoder.PowerPlatform.Utils.Nodes;
using SPCoder.Utils;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace SPCoder.PowerPlatform.Utils
{

    public class PPConnector : BaseConnector
    {
        public AuthenticationResult AzureManagementToken { get; set; }
        public AuthenticationResult FlowsApiToken {
            get
            {
                if (_flowsApiToken == null)
                {
                    _flowsApiToken = GetFlowsApiToken();
                }
                return _flowsApiToken;
            }
        }
        //public string EnvironmentUrl { get; set; }

        
        DirectoryInfo Folder { get; set; }
        public PPConnector(string connectorType)
        { }

        public PPConnector()
        { }

        string RootNodeTitle = "Power platform";

        private string clientId = "";
        private Uri redirectUrl = null;
        private string authUrl = "";
        private string flowApiUrl = "";
        private AuthenticationResult _flowsApiToken;

        private string azureManagementApiUrl = "";

        private Configuration _configuration;
        public override SPCoder.Utils.Nodes.BaseNode GetSPStructure(string envUrl)
        {
            _configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            //this.EnvironmentUrl = envUrl;
            //the default value is the one of MS Samples app.
            clientId = _configuration.AppSettings.Settings["PPClientId"].Value;//  "51f81489-12ee-4a9e-aaae-a2591f45987d";
            //the default value is the one of MS Samples app.
            redirectUrl = new Uri(_configuration.AppSettings.Settings["PPRedirectUrl"].Value);// new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");            
            authUrl = _configuration.AppSettings.Settings["PPAuthUrl"].Value;// "https://login.microsoftonline.com/common";
            var context = new AuthenticationContext(authUrl, false);
            var platformParameters = new PlatformParameters(PromptBehavior.SelectAccount);
            azureManagementApiUrl = _configuration.AppSettings.Settings["PPFAzureManagementApiUrl"].Value; // "https://management.azure.com";

            //This could be used to connect just to one Environment
            AzureManagementToken = context.AcquireTokenAsync(azureManagementApiUrl, clientId, redirectUrl, platformParameters, UserIdentifier.AnyUser).Result;

            return GenerateRootNode();
        }

        /*public async Task<AuthenticationResult> SilentlyGetTokenForUrl(string apiUrlForGettingToken)
        {
            var context = new AuthenticationContext(authUrl, false);
            var tkn = await context.AcquireTokenSilentAsync(apiUrlForGettingToken, clientId);
            return tkn;
        }*/

        public AuthenticationResult SilentlyGetTokenForUrl(string apiUrlForGettingToken)
        {
            var context = new AuthenticationContext(authUrl, false);
            //, AzureManagementToken.UserInfo.UniqueId
            var ui = new UserIdentifier(AzureManagementToken.UserInfo.UniqueId, UserIdentifierType.UniqueId);
            var tkn = context.AcquireTokenSilentAsync(apiUrlForGettingToken, clientId, ui).Result;
            return tkn;
        }

        private AuthenticationResult GetFlowsApiToken()
        {
            flowApiUrl = _configuration.AppSettings.Settings["PPFlowApiUrl"].Value; // "https://service.flow.microsoft.com";
            return SilentlyGetTokenForUrl(flowApiUrl);
        }

        public override BaseNode GenerateRootNode()
        {
            //PPFlowContainerNode
            var rezFlows = GetAllEnvironments();
            dynamic jsond = JsonConvert.DeserializeObject(rezFlows.Result.ToString());
            //rezFlows.Result;


            BaseNode rootNode = new PPEnvironmentContainerNode();
            rootNode.Title = RootNodeTitle;
            rootNode.NodeConnector = this;
            rootNode.OMType = ObjectModelType.REMOTE;
            rootNode.SPObject = jsond["value"];
            rootNode.LoadedData = true;
            GenerateEnvironmentNodes(rootNode, jsond);
            return rootNode;
        }

        public void GenerateEnvironmentNodes(BaseNode rootNode, dynamic jsond)
        {
            

            List<Newtonsoft.Json.Linq.JObject> list = new List<Newtonsoft.Json.Linq.JObject>();
            foreach (var flow in jsond["value"])
            {
                list.Add(flow);
            }

            list = list.OrderBy(m => m["properties"]["displayName"]).ToList();
            foreach (var environment in list)
            {
                var myNode = new PPEnvironmentNode(environment);
                rootNode.Children.Add(myNode);
                myNode.ParentNode = rootNode;
                myNode.RootNode = rootNode;
                myNode.NodeConnector = this;
                myNode.LoadedData = false;
                myNode.SPObject = environment;
            }

        }

        public override BaseNode ExpandNode(BaseNode node, bool doIfLoaded = false)
        {
            if (node is PPEnvironmentNode)
            {
                //If not loaded
                if (!doIfLoaded)
                {
                    if (node.ParentNode.Children != null && node.ParentNode.Children.Contains(node))
                    {
                        node.ParentNode.Children.Remove(node);
                    }
                    //
                    var rezFlows = ((PPEnvironmentNode)node).GetAllFlows();
                    dynamic jsond = JsonConvert.DeserializeObject(rezFlows.Result.ToString());
                    GenerateFlowNodes(node.RootNode, node, jsond);
                }
            }
            return node;
        }

        public async Task<object> GetAllEnvironments()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AzureManagementToken.AccessToken);
                client.Timeout = new TimeSpan(0, 3, 0);
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    string endpoint = _configuration.AppSettings.Settings["PPFAzureManagementApiGetEnvironmentsEndpoint"].Value;
                    HttpResponseMessage response = client.GetAsync(endpoint).Result;
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
        /*public BaseNode ExpandEnvironmentNode(string environmentUrl)
        {
            //PPFlowContainerNode
            var rezFlows = GetAllFlows(environmentUrl);
            dynamic jsond = JsonConvert.DeserializeObject(rezFlows.Result.ToString());
            //rezFlows.Result;


            BaseNode rootNode = new PPEnvironmentNode();
            rootNode.Title = RootNodeTitle + environmentUrl;
            rootNode.NodeConnector = this;
            rootNode.OMType = ObjectModelType.REMOTE;
            rootNode.SPObject = jsond["value"];
            rootNode.LoadedData = true;
            GenerateFlowNodes(rootNode, jsond);
            return rootNode;
        }*/
        public void GenerateFlowNodes(BaseNode rootNode, BaseNode environmentNode, dynamic jsond)
        {
            var myNode = new PPFlowContainerNode();
            environmentNode.Children.Add(myNode);
            myNode.ParentNode = environmentNode;
            myNode.RootNode = rootNode;
            myNode.NodeConnector = this;
            myNode.LoadedData = false;

            List<Newtonsoft.Json.Linq.JObject> list = new List<Newtonsoft.Json.Linq.JObject>();
            foreach (var flow in jsond["value"])
            {
                list.Add(flow);
            }

            list = list.OrderBy(m => m["name"]).ToList();

            //add category node and flows under it for each category
            AddCategoryAndFlows(myNode, rootNode, list, "0", "Classic Dataverse workflows" );
            AddCategoryAndFlows(myNode, rootNode, list, "1", "Classic Dataverse dialogs");
            AddCategoryAndFlows(myNode, rootNode, list, "2", "Business rules");
            AddCategoryAndFlows(myNode, rootNode, list, "3", "Classic Dataverse actions");
            AddCategoryAndFlows(myNode, rootNode, list, "4", "Business process flows");
            AddCategoryAndFlows(myNode, rootNode, list, "5", "Automated, instant or scheduled flows");
            AddCategoryAndFlows(myNode, rootNode, list, "6", "Desktop flows");
        }

        protected void AddCategoryAndFlows(BaseNode parentNode, BaseNode rootNode, List<Newtonsoft.Json.Linq.JObject> flows, string categoryNumber, string categoryName)
        {
            var myCategoryNode = new PPFlowContainerNode();            
            myCategoryNode.Title = categoryName;
            parentNode.Children.Add(myCategoryNode);
            myCategoryNode.ParentNode = parentNode;
            myCategoryNode.RootNode = rootNode;
            myCategoryNode.NodeConnector = this;
            myCategoryNode.LoadedData = false;

            foreach (var flow in flows)
            {
                if (categoryNumber != flow["category"].ToString()) continue;

                PPFlowNode myListNode = new PPFlowNode(flow);
                myListNode.EnvironmentNode = (PPEnvironmentNode)parentNode.ParentNode;
                myCategoryNode.Children.Add(myListNode);
                myListNode.ParentNode = myCategoryNode;
                myListNode.RootNode = rootNode;
                myListNode.NodeConnector = this;
            }
        }
        //GenerateFlowNodes
        

        public override string ImagesPath
        {
            get
            {
                return null;
            }
            set
            {}
        }

        public override bool IsImagesPathLocal
        {
            get
            {
                return true;
            }
            set
            {}
        }


        public override List<object> AutoAddToContext()
        {
            List<object> objects = new List<object>();
            objects.Add(this);
            return objects;
        }
    }
}
