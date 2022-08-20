using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SPCoder.Core.Utils;
using SPCoder.PowerPlatform.Utils.Nodes;
using SPCoder.Utils;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace SPCoder.PowerPlatform.Utils
{

    public class PPConnector : BaseConnector
    {
        public AuthenticationResult Token { get; set; }
        public string EnvironmentUrl { get; set; }

        DirectoryInfo Folder { get; set; }
        public PPConnector(string connectorType)
        { }

        public PPConnector()
        { }

        string RootNodeTitle = "Power platform environment: ";

        public override SPCoder.Utils.Nodes.BaseNode GetSPStructure(string envUrl)
        {
            this.EnvironmentUrl = envUrl;
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";            
            var redirectUrl = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");
            string auth = "https://login.microsoftonline.com/common";
            var context = new AuthenticationContext(auth, false);
            var platformParameters = new PlatformParameters(PromptBehavior.SelectAccount);
            Token = context.AcquireTokenAsync(this.EnvironmentUrl, clientId, redirectUrl, platformParameters, UserIdentifier.AnyUser).Result;

            return GenerateRootNode();
        }

        public override BaseNode GenerateRootNode()
        {
            //PPFlowContainerNode
            var rezFlows = GetAllFlows();
            dynamic jsond = JsonConvert.DeserializeObject(rezFlows.Result.ToString());
            //rezFlows.Result;


            BaseNode rootNode = new PPEnvironmentNode();
            rootNode.Title = RootNodeTitle + EnvironmentUrl;
            rootNode.NodeConnector = this;
            rootNode.OMType = ObjectModelType.REMOTE;
            rootNode.SPObject = jsond["value"];
            rootNode.LoadedData = true;
            GenerateFlowNodes(rootNode, jsond);
            return rootNode;
        }
        public void GenerateFlowNodes(BaseNode rootNode, dynamic jsond)
        {
            var myNode = new PPFlowContainerNode();
            rootNode.Children.Add(myNode);
            myNode.ParentNode = rootNode;
            myNode.RootNode = rootNode;
            myNode.NodeConnector = this;
            myNode.LoadedData = true;

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
            myCategoryNode.LoadedData = true;

            foreach (var flow in flows)
            {
                if (categoryNumber != flow["category"].ToString()) continue;

                BaseNode myListNode = new PPFlowNode(flow);
                myCategoryNode.Children.Add(myListNode);
                myListNode.ParentNode = myCategoryNode;
                myListNode.RootNode = rootNode;
                myListNode.NodeConnector = this;
            }
        }
        //GenerateFlowNodes
        async Task<object> GetAllFlows()
        {
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
